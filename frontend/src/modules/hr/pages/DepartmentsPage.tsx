import { useEffect, useState, useCallback } from "react";
import type { ColumnDef, ColumnFiltersState, OnChangeFn, SortingState } from "@tanstack/react-table";
import { Plus, Trash2, Edit, CheckCircle, XCircle } from "lucide-react";
import { Button } from "@app/components/ui/button";
import { Input } from "@app/components/ui/input";
import {
  Dialog, DialogContent, DialogHeader, DialogTitle,
} from "@app/components/ui/dialog";
import {
  PageHeader, DeleteDialog, FormError,
  PageLayout, DataTable, FormField, FormActions, StatusBadge,
  ColumnFilterInput, ColumnFilterSelect,
} from "@shared/components";
import { getDepartments, createDepartment, updateDepartment, deleteDepartment, activateDepartment, deactivateDepartment } from "../api/hrApi";
import type { Department, CreateDepartmentRequest, UpdateDepartmentRequest, PagedResult } from "../types";

const STATUS_OPTIONS = [
  { value: "", label: "All" },
  { value: "true", label: "Active" },
  { value: "false", label: "Inactive" },
];

function DepartmentForm({
  department,
  onSave,
  onCancel,
}: {
  department?: Department;
  onSave: (data: CreateDepartmentRequest | UpdateDepartmentRequest) => Promise<void>;
  onCancel: () => void;
}) {
  const [form, setForm] = useState({
    code: department?.code ?? "",
    name: department?.name ?? "",
    description: department?.description ?? "",
  });
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState("");

  const set = (field: string, value: string) => setForm((f) => ({ ...f, [field]: value }));

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setSaving(true);
    setError("");
    try {
      const common = {
        name: form.name,
        description: form.description || undefined,
      };
      if (department) {
        await onSave(common as UpdateDepartmentRequest);
      } else {
        await onSave({ code: form.code, ...common } as CreateDepartmentRequest);
      }
    } catch (err) {
      setError(err instanceof Error ? err.message : "An error occurred.");
      setSaving(false);
    }
  };

  return (
    <form onSubmit={handleSubmit} className="space-y-4">
      <FormError error={error} />

      {!department && (
        <FormField id="code" label="Code">
          <Input id="code" required value={form.code} onChange={(e) => set("code", e.target.value)} />
        </FormField>
      )}

      <FormField id="name" label="Name">
        <Input id="name" required value={form.name} onChange={(e) => set("name", e.target.value)} />
      </FormField>

      <FormField id="description" label="Description">
        <Input id="description" value={form.description} onChange={(e) => set("description", e.target.value)} />
      </FormField>

      <FormActions onCancel={onCancel} saving={saving} saveLabel={department ? "Update Department" : "Create Department"} />
    </form>
  );
}

export function DepartmentsPage() {
  const [result, setResult] = useState<PagedResult<Department> | null>(null);
  const [loading, setLoading] = useState(true);
  const [columnFilters, setColumnFilters] = useState<ColumnFiltersState>([]);
  const [sorting, setSorting] = useState<SortingState>([]);
  const [page, setPage] = useState(1);
  const [formOpen, setFormOpen] = useState(false);
  const [editingDepartment, setEditingDepartment] = useState<Department | undefined>(undefined);
  const [deleteTarget, setDeleteTarget] = useState<Department | null>(null);
  const [deleting, setDeleting] = useState(false);

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

  const fetchDepartments = useCallback(() => {
    const search = String(columnFilters.find((f) => f.id === "searchTerm")?.value ?? "");
    const statusFilter = String(columnFilters.find((f) => f.id === "isActive")?.value ?? "");
    const params: Record<string, string> = { page: String(page), pageSize: "20" };
    if (search) params.searchTerm = search;
    if (statusFilter) params.isActive = statusFilter;
    if (sorting[0]) {
      params.sortBy = sorting[0].id;
      params.sortDescending = String(sorting[0].desc);
    }
    setLoading(true);
    getDepartments(params)
      .then(setResult)
      .finally(() => setLoading(false));
  }, [columnFilters, page, sorting]);

  useEffect(() => { fetchDepartments(); }, [fetchDepartments]);

  const handleSave = async (data: CreateDepartmentRequest | UpdateDepartmentRequest) => {
    if (editingDepartment) {
      await updateDepartment(editingDepartment.id, data as UpdateDepartmentRequest);
    } else {
      await createDepartment(data as CreateDepartmentRequest);
    }
    setFormOpen(false);
    setEditingDepartment(undefined);
    fetchDepartments();
  };

  const handleDelete = async () => {
    if (!deleteTarget) return;
    setDeleting(true);
    try {
      await deleteDepartment(deleteTarget.id);
      setDeleteTarget(null);
      fetchDepartments();
    } finally {
      setDeleting(false);
    }
  };

  const handleToggleActive = async (d: Department) => {
    if (d.isActive) {
      await deactivateDepartment(d.id);
    } else {
      await activateDepartment(d.id);
    }
    fetchDepartments();
  };

  const columns: ColumnDef<Department>[] = [
    {
      accessorKey: "code",
      header: "Code",
      cell: ({ row }) => <span className="font-medium">{row.original.code}</span>,
      meta: {
        filterId: "searchTerm",
        filterComponent: ({ value, onChange }) => (
          <ColumnFilterInput value={value} onChange={onChange} placeholder="Search…" />
        ),
      },
    },
    { accessorKey: "name", header: "Name" },
    {
      accessorKey: "description",
      header: "Description",
      cell: ({ row }) => <span className="text-muted-foreground">{row.original.description ?? "—"}</span>,
      enableSorting: false,
    },
    {
      accessorKey: "isActive",
      header: "Status",
      cell: ({ row }) => <StatusBadge isActive={row.original.isActive} />,
      enableSorting: false,
      meta: {
        filterComponent: ({ value, onChange }) => (
          <ColumnFilterSelect value={value} onChange={onChange} options={STATUS_OPTIONS} placeholder="All" />
        ),
      },
    },
    {
      id: "actions",
      header: "Actions",
      enableSorting: false,
      meta: { className: "text-right", headerClassName: "text-right" },
      cell: ({ row }) => {
        const d = row.original;
        return (
          <div className="flex items-center justify-end gap-1">
            <Button variant="ghost" size="sm" onClick={() => handleToggleActive(d)} title={d.isActive ? "Deactivate" : "Activate"}>
              {d.isActive ? <XCircle className="h-4 w-4 text-orange-500" /> : <CheckCircle className="h-4 w-4 text-green-600" />}
            </Button>
            <Button variant="ghost" size="sm" onClick={() => { setEditingDepartment(d); setFormOpen(true); }} title="Edit">
              <Edit className="h-4 w-4" />
            </Button>
            <Button variant="ghost" size="sm" onClick={() => setDeleteTarget(d)} title="Delete">
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
        title="Departments"
        description="Manage organizational departments"
        action={
          <Button onClick={() => { setEditingDepartment(undefined); setFormOpen(true); }}>
            <Plus className="mr-2 h-4 w-4" /> Add Department
          </Button>
        }
      />

      <DataTable
        columns={columns}
        data={result?.items ?? []}
        loading={loading}
        emptyText="No departments found."
        filtering={{ state: columnFilters, onChange: handleFilteringChange, manual: true }}
        sorting={{ state: sorting, onChange: handleSortingChange, manual: true }}
        pagination={result ? { result, onPrevious: () => setPage((p) => p - 1), onNext: () => setPage((p) => p + 1) } : undefined}
      />

      <Dialog open={formOpen} onOpenChange={setFormOpen}>
        <DialogContent className="sm:max-w-md">
          <DialogHeader><DialogTitle>{editingDepartment ? "Edit Department" : "Create Department"}</DialogTitle></DialogHeader>
          <DepartmentForm department={editingDepartment} onSave={handleSave} onCancel={() => { setFormOpen(false); setEditingDepartment(undefined); }} />
        </DialogContent>
      </Dialog>

      <DeleteDialog
        open={!!deleteTarget}
        onOpenChange={(open) => { if (!open) setDeleteTarget(null); }}
        title="Delete Department"
        entityLabel={deleteTarget ? `${deleteTarget.name} (${deleteTarget.code})` : ""}
        onConfirm={handleDelete}
        deleting={deleting}
      />
    </PageLayout>
  );
}
