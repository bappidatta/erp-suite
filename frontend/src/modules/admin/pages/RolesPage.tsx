import { useEffect, useState, useCallback } from "react";
import { Plus, Trash2, Edit, Shield } from "lucide-react";
import { Card, CardContent } from "@app/components/ui/card";
import { Button } from "@app/components/ui/button";
import { Input } from "@app/components/ui/input";
import { Badge } from "@app/components/ui/badge";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@app/components/ui/table";
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
} from "@app/components/ui/dialog";
import { Label } from "@app/components/ui/label";
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
      {error && <div className="rounded-md bg-destructive/10 p-3 text-sm text-destructive">{error}</div>}
      <div className="space-y-1">
        <Label htmlFor="name">Role Name</Label>
        <Input id="name" required value={name} onChange={(e) => setName(e.target.value)} />
      </div>
      <div className="space-y-1">
        <Label htmlFor="description">Description</Label>
        <Input id="description" value={description} onChange={(e) => setDescription(e.target.value)} />
      </div>
      <div className="flex justify-end gap-2 pt-2">
        <Button type="button" variant="outline" onClick={onCancel} disabled={saving}>Cancel</Button>
        <Button type="submit" disabled={saving}>{saving ? "Saving…" : role ? "Update Role" : "Create Role"}</Button>
      </div>
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

  return (
    <div className="space-y-4">
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-2xl font-bold tracking-tight">Roles</h1>
          <p className="text-muted-foreground">Manage roles and permissions</p>
        </div>
        <Button onClick={() => { setEditingRole(undefined); setFormOpen(true); }}>
          <Plus className="mr-2 h-4 w-4" /> Add Role
        </Button>
      </div>

      <Card>
        <CardContent className="p-0">
          <Table>
            <TableHeader>
              <TableRow>
                <TableHead>Name</TableHead>
                <TableHead>Description</TableHead>
                <TableHead>Users</TableHead>
                <TableHead>Permissions</TableHead>
                <TableHead>Type</TableHead>
                <TableHead className="text-right">Actions</TableHead>
              </TableRow>
            </TableHeader>
            <TableBody>
              {loading ? (
                <TableRow>
                  <TableCell colSpan={6} className="py-10 text-center text-muted-foreground">Loading…</TableCell>
                </TableRow>
              ) : roles.length === 0 ? (
                <TableRow>
                  <TableCell colSpan={6} className="py-10 text-center text-muted-foreground">No roles found.</TableCell>
                </TableRow>
              ) : (
                roles.map((role) => (
                  <TableRow key={role.id}>
                    <TableCell className="font-medium">{role.name}</TableCell>
                    <TableCell className="text-muted-foreground">{role.description ?? "—"}</TableCell>
                    <TableCell>{role.userCount}</TableCell>
                    <TableCell>{role.permissionCount}</TableCell>
                    <TableCell>
                      {role.isSystem ? <Badge variant="secondary">System</Badge> : <Badge variant="outline">Custom</Badge>}
                    </TableCell>
                    <TableCell className="text-right">
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
                    </TableCell>
                  </TableRow>
                ))
              )}
            </TableBody>
          </Table>
        </CardContent>
      </Card>

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
      <Dialog open={!!deleteTarget} onOpenChange={(open) => { if (!open) setDeleteTarget(null); }}>
        <DialogContent className="sm:max-w-sm">
          <DialogHeader>
            <DialogTitle>Delete Role</DialogTitle>
          </DialogHeader>
          <p className="text-sm text-muted-foreground">
            Are you sure you want to delete the role <strong>{deleteTarget?.name}</strong>?
          </p>
          <div className="flex justify-end gap-2 pt-2">
            <Button variant="outline" onClick={() => setDeleteTarget(null)} disabled={deleting}>Cancel</Button>
            <Button variant="destructive" onClick={handleDelete} disabled={deleting}>
              {deleting ? "Deleting…" : "Delete"}
            </Button>
          </div>
        </DialogContent>
      </Dialog>
    </div>
  );
}
