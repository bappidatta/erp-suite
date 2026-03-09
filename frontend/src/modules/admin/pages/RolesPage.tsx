import { useEffect, useState, useCallback } from "react";
import type { ColumnDef } from "@tanstack/react-table";
import { Plus, Trash2, Edit, Shield } from "lucide-react";
import { Button } from "@app/components/ui/button";
import { Input } from "@app/components/ui/input";
import { Badge } from "@app/components/ui/badge";
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
} from "@app/components/ui/dialog";
import {
  PageHeader, DeleteDialog, FormError,
  PageLayout, DataTable, FormField, FormActions,
} from "@shared/components";
import { getRoles, createRole, updateRole, deleteRole, getAllPermissions, assignPermissions } from "../api/adminApi";
import type { Role, Permission, CreateRoleRequest, RoleDetail, UpdateRoleRequest } from "../types";
import { getRoleById } from "../api/adminApi";

function RoleForm({
  role,
  onSave,
  onCancel,
}: {
  role?: Role;
  onSave: (data: CreateRoleRequest | UpdateRoleRequest) => Promise<void>;
  onCancel: () => void;
}) {
  const [name, setName] = useState(role?.name ?? "");
  const [description, setDescription] = useState(role?.description ?? "");
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState("");

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setSaving(true);
    setError("");
    try {
      await onSave({ name, description: description || undefined });
    } catch (err) {
      setError(err instanceof Error ? err.message : "An error occurred.");
      setSaving(false);
    }
  };

  return (
    <form onSubmit={handleSubmit} className="space-y-4">
      <FormError error={error} />
      <FormField id="name" label="Role Name">
        <Input id="name" required value={name} onChange={(e) => setName(e.target.value)} />
      </FormField>
      <FormField id="description" label="Description">
        <Input id="description" value={description} onChange={(e) => setDescription(e.target.value)} />
      </FormField>
      <FormActions onCancel={onCancel} saving={saving} saveLabel={role ? "Update Role" : "Create Role"} />
    </form>
  );
}

function PermissionsDialog({
  role,
  permissions,
  onClose,
}: {
  role: RoleDetail;
  permissions: Permission[];
  onClose: () => void;
}) {
  const [selected, setSelected] = useState<Set<number>>(new Set(role.permissions.map((p) => p.id)));
  const [saving, setSaving] = useState(false);

  const byModule = permissions.reduce<Record<string, Permission[]>>((acc, p) => {
    acc[p.module] = [...(acc[p.module] ?? []), p];
    return acc;
  }, {});

  const toggle = (id: number) =>
    setSelected((prev) => {
      const next = new Set(prev);
      if (next.has(id)) next.delete(id);
      else next.add(id);
      return next;
    });

  const toggleAll = (module: string) => {
    const ids = byModule[module].map((p) => p.id);
    const allSelected = ids.every((id) => selected.has(id));
    setSelected((prev) => {
      const next = new Set(prev);
      ids.forEach((id) => (allSelected ? next.delete(id) : next.add(id)));
      return next;
    });
  };

  const handleSave = async () => {
    setSaving(true);
    try {
      await assignPermissions(role.id, Array.from(selected));
      onClose();
    } finally {
      setSaving(false);
    }
  };

  return (
    <div className="space-y-4">
      <div className="max-h-96 overflow-y-auto space-y-4">
        {Object.entries(byModule).map(([module, perms]) => {
          const allSelected = perms.every((p) => selected.has(p.id));
          return (
            <div key={module}>
              <div className="flex items-center gap-2 mb-2">
                <input
                  type="checkbox"
                  id={`module-${module}`}
                  checked={allSelected}
                  onChange={() => toggleAll(module)}
                  className="h-4 w-4"
                />
                <label htmlFor={`module-${module}`} className="font-semibold text-sm">{module}</label>
              </div>
              <div className="ml-6 grid grid-cols-2 gap-1">
                {perms.map((p) => (
                  <div key={p.id} className="flex items-center gap-2">
                    <input
                      type="checkbox"
                      id={`perm-${p.id}`}
                      checked={selected.has(p.id)}
                      onChange={() => toggle(p.id)}
                      className="h-4 w-4"
                    />
                    <label htmlFor={`perm-${p.id}`} className="text-sm">{p.action}</label>
                  </div>
                ))}
              </div>
            </div>
          );
        })}
        {Object.keys(byModule).length === 0 && (
          <p className="text-sm text-muted-foreground">No permissions defined yet.</p>
        )}
      </div>
      <div className="flex justify-end gap-2">
        <Button variant="outline" onClick={onClose} disabled={saving}>Cancel</Button>
        <Button onClick={handleSave} disabled={saving}>{saving ? "Saving…" : "Save Permissions"}</Button>
      </div>
    </div>
  );
}

export function RolesPage() {
  const [roles, setRoles] = useState<Role[]>([]);
  const [allPermissions, setAllPermissions] = useState<Permission[]>([]);
  const [loading, setLoading] = useState(true);
  const [formOpen, setFormOpen] = useState(false);
  const [editingRole, setEditingRole] = useState<Role | undefined>(undefined);
  const [permissionsRole, setPermissionsRole] = useState<RoleDetail | null>(null);
  const [deleteTarget, setDeleteTarget] = useState<Role | null>(null);
  const [deleting, setDeleting] = useState(false);

  const fetchRoles = useCallback(() => {
    setLoading(true);
    getRoles().then(setRoles).finally(() => setLoading(false));
  }, []);

  useEffect(() => {
    fetchRoles();
    getAllPermissions().then(setAllPermissions).catch(() => {});
  }, [fetchRoles]);

  const handleSaveRole = async (data: CreateRoleRequest | UpdateRoleRequest) => {
    if (editingRole) {
      await updateRole(editingRole.id, data as UpdateRoleRequest);
    } else {
      await createRole(data as CreateRoleRequest);
    }
    setFormOpen(false);
    setEditingRole(undefined);
    fetchRoles();
  };

  const handleDelete = async () => {
    if (!deleteTarget) return;
    setDeleting(true);
    try {
      await deleteRole(deleteTarget.id);
      setDeleteTarget(null);
      fetchRoles();
    } finally {
      setDeleting(false);
    }
  };

  const openPermissions = async (role: Role) => {
    const detail = await getRoleById(role.id);
    setPermissionsRole(detail);
  };

  const columns: ColumnDef<Role>[] = [
    {
      accessorKey: "name",
      header: "Name",
      cell: ({ row }) => <span className="font-medium">{row.original.name}</span>,
    },
    {
      accessorKey: "description",
      header: "Description",
      cell: ({ row }) => <span className="text-muted-foreground">{row.original.description ?? "—"}</span>,
    },
    { accessorKey: "userCount", header: "Users" },
    { accessorKey: "permissionCount", header: "Permissions" },
    {
      accessorKey: "isSystem",
      header: "Type",
      cell: ({ row }) =>
        row.original.isSystem
          ? <Badge variant="secondary">System</Badge>
          : <Badge variant="outline">Custom</Badge>,
      enableSorting: false,
    },
    {
      id: "actions",
      header: "Actions",
      enableSorting: false,
      meta: { className: "text-right", headerClassName: "text-right" },
      cell: ({ row }) => {
        const role = row.original;
        return (
          <div className="flex items-center justify-end gap-1">
            <Button variant="ghost" size="sm" onClick={() => openPermissions(role)} title="Set Permissions">
              <Shield className="h-4 w-4" />
            </Button>
            {!role.isSystem && (
              <>
                <Button variant="ghost" size="sm" onClick={() => { setEditingRole(role); setFormOpen(true); }} title="Edit">
                  <Edit className="h-4 w-4" />
                </Button>
                <Button variant="ghost" size="sm" onClick={() => setDeleteTarget(role)} title="Delete">
                  <Trash2 className="h-4 w-4 text-destructive" />
                </Button>
              </>
            )}
          </div>
        );
      },
    },
  ];

  return (
    <PageLayout>
      <PageHeader
        title="Roles"
        description="Manage roles and permissions"
        action={
          <Button onClick={() => { setEditingRole(undefined); setFormOpen(true); }}>
            <Plus className="mr-2 h-4 w-4" /> Add Role
          </Button>
        }
      />

      <DataTable columns={columns} data={roles} loading={loading} emptyText="No roles found." />

      {/* Create/Edit Dialog */}
      <Dialog open={formOpen} onOpenChange={setFormOpen}>
        <DialogContent className="sm:max-w-md">
          <DialogHeader>
            <DialogTitle>{editingRole ? "Edit Role" : "Create Role"}</DialogTitle>
          </DialogHeader>
          <RoleForm
            role={editingRole}
            onSave={handleSaveRole}
            onCancel={() => { setFormOpen(false); setEditingRole(undefined); }}
          />
        </DialogContent>
      </Dialog>

      {/* Permissions Dialog */}
      <Dialog open={!!permissionsRole} onOpenChange={(open) => { if (!open) setPermissionsRole(null); }}>
        <DialogContent className="sm:max-w-lg">
          <DialogHeader>
            <DialogTitle>Permissions — {permissionsRole?.name}</DialogTitle>
          </DialogHeader>
          {permissionsRole && (
            <PermissionsDialog
              role={permissionsRole}
              permissions={allPermissions}
              onClose={() => { setPermissionsRole(null); fetchRoles(); }}
            />
          )}
        </DialogContent>
      </Dialog>

      {/* Delete Dialog */}
      <DeleteDialog
        open={!!deleteTarget}
        onOpenChange={(open) => { if (!open) setDeleteTarget(null); }}
        title="Delete Role"
        entityLabel={deleteTarget?.name ?? ""}
        onConfirm={handleDelete}
        deleting={deleting}
      />
    </PageLayout>
  );
}
