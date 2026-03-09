import { useEffect, useState, useCallback } from "react";
import type { ColumnDef, ColumnFiltersState, OnChangeFn, SortingState } from "@tanstack/react-table";
import { Plus, Trash2, Edit, CheckCircle } from "lucide-react";
import { Button } from "@app/components/ui/button";
import { Input } from "@app/components/ui/input";
import { Badge } from "@app/components/ui/badge";
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
} from "@app/components/ui/dialog";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@app/components/ui/select";
import {
  PageHeader, DeleteDialog, FormError,
  PageLayout, DataTable, FormField, FormGrid, FormActions,
  ColumnFilterInput, ColumnFilterSelect,
} from "@shared/components";
import { getUsers, getRoles, createUser, updateUser, deleteUser, activateUser } from "../api/adminApi";
import type { User, Role, CreateUserRequest, UpdateUserRequest, PagedResult } from "../types";

const STATUS_LABELS: Record<number, { label: string; variant: "default" | "secondary" | "destructive" | "outline" }> = {
  1: { label: "Active", variant: "default" },
  2: { label: "Inactive", variant: "secondary" },
  3: { label: "Locked", variant: "destructive" },
  4: { label: "Suspended", variant: "outline" },
};

const STATUS_OPTIONS = [
  { value: "", label: "All Statuses" },
  { value: "1", label: "Active" },
  { value: "2", label: "Inactive" },
  { value: "3", label: "Locked" },
  { value: "4", label: "Suspended" },
];

function UserForm({
  user,
  roles,
  onSave,
  onCancel,
}: {
  user?: User;
  roles: Role[];
  onSave: (data: CreateUserRequest | UpdateUserRequest) => Promise<void>;
  onCancel: () => void;
}) {
  const [form, setForm] = useState({
    email: user?.email ?? "",
    password: "",
    firstName: user?.firstName ?? "",
    lastName: user?.lastName ?? "",
    phone: user?.phone ?? "",
    roleId: user?.roleId?.toString() ?? "",
    mustChangePassword: false,
  });
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState("");

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setSaving(true);
    setError("");
    try {
      if (user) {
        await onSave({
          firstName: form.firstName,
          lastName: form.lastName,
          phone: form.phone || undefined,
          roleId: Number(form.roleId),
        } as UpdateUserRequest);
      } else {
        await onSave({
          email: form.email,
          password: form.password,
          firstName: form.firstName,
          lastName: form.lastName,
          phone: form.phone || undefined,
          roleId: Number(form.roleId),
          mustChangePassword: form.mustChangePassword,
        } as CreateUserRequest);
      }
    } catch (err) {
      setError(err instanceof Error ? err.message : "An error occurred.");
      setSaving(false);
    }
  };

  return (
    <form onSubmit={handleSubmit} className="space-y-4">
      <FormError error={error} />

      {!user && (
        <FormField id="email" label="Email">
          <Input
            id="email"
            type="email"
            required
            value={form.email}
            onChange={(e) => setForm((f) => ({ ...f, email: e.target.value }))}
          />
        </FormField>
      )}

      {!user && (
        <FormField id="password" label="Password">
          <Input
            id="password"
            type="password"
            required
            minLength={8}
            value={form.password}
            onChange={(e) => setForm((f) => ({ ...f, password: e.target.value }))}
          />
        </FormField>
      )}

      <FormGrid>
        <FormField id="firstName" label="First Name">
          <Input
            id="firstName"
            required
            value={form.firstName}
            onChange={(e) => setForm((f) => ({ ...f, firstName: e.target.value }))}
          />
        </FormField>
        <FormField id="lastName" label="Last Name">
          <Input
            id="lastName"
            required
            value={form.lastName}
            onChange={(e) => setForm((f) => ({ ...f, lastName: e.target.value }))}
          />
        </FormField>
      </FormGrid>

      <FormField id="phone" label="Phone">
        <Input
          id="phone"
          value={form.phone}
          onChange={(e) => setForm((f) => ({ ...f, phone: e.target.value }))}
        />
      </FormField>

      <FormField id="role" label="Role">
        <Select value={form.roleId} onValueChange={(v) => setForm((f) => ({ ...f, roleId: v ?? "" }))}>  
          <SelectTrigger>
            <SelectValue placeholder="Select a role" />
          </SelectTrigger>
          <SelectContent>
            {roles.map((r) => (
              <SelectItem key={r.id} value={r.id.toString()}>
                {r.name}
              </SelectItem>
            ))}
          </SelectContent>
        </Select>
      </FormField>

      <FormActions onCancel={onCancel} saving={saving} disabled={!form.roleId} saveLabel={user ? "Update User" : "Create User"} />
    </form>
  );
}

export function UsersPage() {
  const [result, setResult] = useState<PagedResult<User> | null>(null);
  const [roles, setRoles] = useState<Role[]>([]);
  const [loading, setLoading] = useState(true);
  const [columnFilters, setColumnFilters] = useState<ColumnFiltersState>([]);
  const [sorting, setSorting] = useState<SortingState>([]);
  const [page, setPage] = useState(1);
  const [formOpen, setFormOpen] = useState(false);
  const [editingUser, setEditingUser] = useState<User | undefined>(undefined);
  const [deleteTarget, setDeleteTarget] = useState<User | null>(null);
  const [deleting, setDeleting] = useState(false);

  const mapSortField = (columnId: string) => {
    switch (columnId) {
      case "email":
        return "email";
      case "fullName":
        return "name";
      default:
        return columnId;
    }
  };

  const handleSortingChange: OnChangeFn<SortingState> = (updater) => {
    setSorting((current) => {
      const next = typeof updater === "function" ? updater(current) : updater;
      setPage(1);
      return next.slice(0, 1);
    });
  };

  const handleFilteringChange: OnChangeFn<ColumnFiltersState> = (updater) => {
    setColumnFilters((current) => {
      const next = typeof updater === "function" ? updater(current) : updater;
      setPage(1);
      return next;
    });
  };

  const fetchUsers = useCallback(() => {
    const search = String(columnFilters.find((filter) => filter.id === "searchTerm")?.value ?? "");
    const statusFilter = String(columnFilters.find((filter) => filter.id === "status")?.value ?? "");
    const params: Record<string, string> = { page: String(page), pageSize: "20" };
    if (search) params.searchTerm = search;
    if (statusFilter) params.status = statusFilter;
    if (sorting[0]) {
      params.sortBy = mapSortField(sorting[0].id);
      params.sortDescending = String(sorting[0].desc);
    }
    setLoading(true);
    getUsers(params)
      .then(setResult)
      .finally(() => setLoading(false));
  }, [columnFilters, page, sorting]);

  useEffect(() => {
    getRoles().then(setRoles).catch(() => {});
  }, []);

  useEffect(() => {
    fetchUsers();
  }, [fetchUsers]);

  const handleSaveUser = async (data: CreateUserRequest | UpdateUserRequest) => {
    if (editingUser) {
      await updateUser(editingUser.id, data as UpdateUserRequest);
    } else {
      await createUser(data as CreateUserRequest);
    }
    setFormOpen(false);
    setEditingUser(undefined);
    fetchUsers();
  };

  const handleDelete = async () => {
    if (!deleteTarget) return;
    setDeleting(true);
    try {
      await deleteUser(deleteTarget.id);
      setDeleteTarget(null);
      fetchUsers();
    } finally {
      setDeleting(false);
    }
  };

  const handleActivate = async (user: User) => {
    await activateUser(user.id);
    fetchUsers();
  };

  const users = result?.items ?? [];

  const columns: ColumnDef<User>[] = [
    {
      accessorKey: "fullName",
      header: "Name",
      cell: ({ row }) => <span className="font-medium">{row.original.fullName}</span>,
      meta: {
        filterId: "searchTerm",
        filterComponent: ({ value, onChange }) => (
          <ColumnFilterInput
            value={value}
            onChange={onChange}
            placeholder="Search…"
          />
        ),
      },
    },
    {
      accessorKey: "email",
      header: "Email",
      cell: ({ row }) => <span className="text-muted-foreground">{row.original.email}</span>,
    },
    { accessorKey: "roleName", header: "Role" },
    {
      accessorKey: "status",
      header: "Status",
      cell: ({ row }) => {
        const statusInfo = STATUS_LABELS[row.original.status] ?? { label: row.original.statusName, variant: "outline" as const };
        return <Badge variant={statusInfo.variant}>{statusInfo.label}</Badge>;
      },
      enableSorting: false,
      meta: {
        filterComponent: ({ value, onChange }) => (
          <ColumnFilterSelect
            value={value}
            onChange={onChange}
            options={STATUS_OPTIONS}
            placeholder="All Statuses"
          />
        ),
      },
    },
    {
      accessorKey: "lastLoginAt",
      header: "Last Login",
      cell: ({ row }) => (
        <span className="text-muted-foreground">
          {row.original.lastLoginAt ? new Date(row.original.lastLoginAt).toLocaleDateString() : "Never"}
        </span>
      ),
      enableSorting: false,
    },
    {
      id: "actions",
      header: "Actions",
      enableSorting: false,
      meta: { className: "text-right", headerClassName: "text-right" },
      cell: ({ row }) => {
        const user = row.original;
        return (
          <div className="flex items-center justify-end gap-1">
            {user.status !== 1 && (
              <Button variant="ghost" size="sm" onClick={() => handleActivate(user)} title="Activate">
                <CheckCircle className="h-4 w-4 text-green-600" />
              </Button>
            )}
            <Button variant="ghost" size="sm" onClick={() => { setEditingUser(user); setFormOpen(true); }} title="Edit">
              <Edit className="h-4 w-4" />
            </Button>
            <Button variant="ghost" size="sm" onClick={() => setDeleteTarget(user)} title="Delete">
              <Trash2 className="h-4 w-4 text-destructive" />
            </Button>
          </div>
        );
      },
    },
  ];

  return (
    <PageLayout>
      <PageHeader
        title="Users"
        description="Manage system users"
        action={
          <Button onClick={() => { setEditingUser(undefined); setFormOpen(true); }}>
            <Plus className="mr-2 h-4 w-4" /> Add User
          </Button>
        }
      />

      <DataTable
        columns={columns}
        data={users}
        loading={loading}
        emptyText="No users found."
        filtering={{ state: columnFilters, onChange: handleFilteringChange, manual: true }}
        sorting={{ state: sorting, onChange: handleSortingChange, manual: true }}
        pagination={result ? { result, onPrevious: () => setPage((p) => p - 1), onNext: () => setPage((p) => p + 1) } : undefined}
      />

      {/* Create/Edit Dialog */}
      <Dialog open={formOpen} onOpenChange={setFormOpen}>
        <DialogContent className="sm:max-w-md">
          <DialogHeader>
            <DialogTitle>{editingUser ? "Edit User" : "Create User"}</DialogTitle>
          </DialogHeader>
          <UserForm
            user={editingUser}
            roles={roles}
            onSave={handleSaveUser}
            onCancel={() => { setFormOpen(false); setEditingUser(undefined); }}
          />
        </DialogContent>
      </Dialog>

      {/* Delete Dialog */}
      <DeleteDialog
        open={!!deleteTarget}
        onOpenChange={(open) => { if (!open) setDeleteTarget(null); }}
        title="Delete User"
        entityLabel={deleteTarget?.fullName ?? ""}
        onConfirm={handleDelete}
        deleting={deleting}
      />
    </PageLayout>
  );
}
