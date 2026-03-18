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
import { getUoms, createUom, updateUom, deleteUom, activateUom, deactivateUom } from "../api/inventoryApi";
import type { UnitOfMeasure, CreateUomRequest, UpdateUomRequest, PagedResult } from "../types";

const STATUS_OPTIONS = [
  { value: "", label: "All" },
  { value: "true", label: "Active" },
  { value: "false", label: "Inactive" },
];

function UomForm({
  uom,
  onSave,
  onCancel,
}: {
  uom?: UnitOfMeasure;
  onSave: (data: CreateUomRequest | UpdateUomRequest) => Promise<void>;
  onCancel: () => void;
}) {
  const [form, setForm] = useState({
    code: uom?.code ?? "",
    name: uom?.name ?? "",
    description: uom?.description ?? "",
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
      if (uom) {
        await onSave(common as UpdateUomRequest);
      } else {
        await onSave({ code: form.code, ...common } as CreateUomRequest);
      }
    } catch (err) {
      setError(err instanceof Error ? err.message : "An error occurred.");
      setSaving(false);
    }
  };

  return (
    <form onSubmit={handleSubmit} className="space-y-4">
      <FormError error={error} />

      {!uom && (
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

      <FormActions onCancel={onCancel} saving={saving} saveLabel={uom ? "Update UOM" : "Create UOM"} />
    </form>
  );
}

export function UomsPage() {
  const [result, setResult] = useState<PagedResult<UnitOfMeasure> | null>(null);
  const [loading, setLoading] = useState(true);
  const [columnFilters, setColumnFilters] = useState<ColumnFiltersState>([]);
  const [sorting, setSorting] = useState<SortingState>([]);
  const [page, setPage] = useState(1);
  const [formOpen, setFormOpen] = useState(false);
  const [editingUom, setEditingUom] = useState<UnitOfMeasure | undefined>(undefined);
  const [deleteTarget, setDeleteTarget] = useState<UnitOfMeasure | null>(null);
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

  const fetchUoms = useCallback(() => {
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
    getUoms(params)
      .then(setResult)
      .finally(() => setLoading(false));
  }, [columnFilters, page, sorting]);

  useEffect(() => { fetchUoms(); }, [fetchUoms]);

  const handleSave = async (data: CreateUomRequest | UpdateUomRequest) => {
    if (editingUom) {
      await updateUom(editingUom.id, data as UpdateUomRequest);
    } else {
      await createUom(data as CreateUomRequest);
    }
    setFormOpen(false);
    setEditingUom(undefined);
    fetchUoms();
  };

  const handleDelete = async () => {
    if (!deleteTarget) return;
    setDeleting(true);
    try {
      await deleteUom(deleteTarget.id);
      setDeleteTarget(null);
      fetchUoms();
    } finally {
      setDeleting(false);
    }
  };

  const handleToggleActive = async (u: UnitOfMeasure) => {
    if (u.isActive) {
      await deactivateUom(u.id);
    } else {
      await activateUom(u.id);
    }
    fetchUoms();
  };

  const columns: ColumnDef<UnitOfMeasure>[] = [
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
        const u = row.original;
        return (
          <div className="flex items-center justify-end gap-1">
            <Button variant="ghost" size="sm" onClick={() => handleToggleActive(u)} title={u.isActive ? "Deactivate" : "Activate"}>
              {u.isActive ? <XCircle className="h-4 w-4 text-orange-500" /> : <CheckCircle className="h-4 w-4 text-green-600" />}
            </Button>
            <Button variant="ghost" size="sm" onClick={() => { setEditingUom(u); setFormOpen(true); }} title="Edit">
              <Edit className="h-4 w-4" />
            </Button>
            <Button variant="ghost" size="sm" onClick={() => setDeleteTarget(u)} title="Delete">
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
        title="Units of Measure"
        description="Manage units of measure"
        action={
          <Button onClick={() => { setEditingUom(undefined); setFormOpen(true); }}>
            <Plus className="mr-2 h-4 w-4" /> Add UOM
          </Button>
        }
      />

      <DataTable
        columns={columns}
        data={result?.items ?? []}
        loading={loading}
        emptyText="No units of measure found."
        filtering={{ state: columnFilters, onChange: handleFilteringChange, manual: true }}
        sorting={{ state: sorting, onChange: handleSortingChange, manual: true }}
        pagination={result ? { result, onPrevious: () => setPage((p) => p - 1), onNext: () => setPage((p) => p + 1) } : undefined}
      />

      <Dialog open={formOpen} onOpenChange={setFormOpen}>
        <DialogContent className="sm:max-w-md">
          <DialogHeader><DialogTitle>{editingUom ? "Edit Unit of Measure" : "Create Unit of Measure"}</DialogTitle></DialogHeader>
          <UomForm uom={editingUom} onSave={handleSave} onCancel={() => { setFormOpen(false); setEditingUom(undefined); }} />
        </DialogContent>
      </Dialog>

      <DeleteDialog
        open={!!deleteTarget}
        onOpenChange={(open) => { if (!open) setDeleteTarget(null); }}
        title="Delete Unit of Measure"
        entityLabel={deleteTarget ? `${deleteTarget.name} (${deleteTarget.code})` : ""}
        onConfirm={handleDelete}
        deleting={deleting}
      />
    </PageLayout>
  );
}
