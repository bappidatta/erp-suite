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
  PageLayout, DataTable, FormField, FormGrid, FormActions, StatusBadge,
  ColumnFilterInput, ColumnFilterSelect,
} from "@shared/components";
import { getWarehouses, createWarehouse, updateWarehouse, deleteWarehouse, activateWarehouse, deactivateWarehouse } from "../api/inventoryApi";
import type { Warehouse, CreateWarehouseRequest, UpdateWarehouseRequest, PagedResult } from "../types";

const STATUS_OPTIONS = [
  { value: "", label: "All" },
  { value: "true", label: "Active" },
  { value: "false", label: "Inactive" },
];

function WarehouseForm({
  warehouse,
  onSave,
  onCancel,
}: {
  warehouse?: Warehouse;
  onSave: (data: CreateWarehouseRequest | UpdateWarehouseRequest) => Promise<void>;
  onCancel: () => void;
}) {
  const [form, setForm] = useState({
    code: warehouse?.code ?? "",
    name: warehouse?.name ?? "",
    location: warehouse?.location ?? "",
    address: warehouse?.address ?? "",
    contactPerson: warehouse?.contactPerson ?? "",
    phone: warehouse?.phone ?? "",
    notes: warehouse?.notes ?? "",
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
        location: form.location || undefined,
        address: form.address || undefined,
        contactPerson: form.contactPerson || undefined,
        phone: form.phone || undefined,
        notes: form.notes || undefined,
      };
      if (warehouse) {
        await onSave(common as UpdateWarehouseRequest);
      } else {
        await onSave({ code: form.code, ...common } as CreateWarehouseRequest);
      }
    } catch (err) {
      setError(err instanceof Error ? err.message : "An error occurred.");
      setSaving(false);
    }
  };

  return (
    <form onSubmit={handleSubmit} className="space-y-4 max-h-[70vh] overflow-y-auto pr-2">
      <FormError error={error} />

      {!warehouse && (
        <FormField id="code" label="Code">
          <Input id="code" required value={form.code} onChange={(e) => set("code", e.target.value)} />
        </FormField>
      )}

      <FormField id="name" label="Name">
        <Input id="name" required value={form.name} onChange={(e) => set("name", e.target.value)} />
      </FormField>

      <FormField id="location" label="Location">
        <Input id="location" value={form.location} onChange={(e) => set("location", e.target.value)} />
      </FormField>

      <FormField id="address" label="Address">
        <Input id="address" value={form.address} onChange={(e) => set("address", e.target.value)} />
      </FormField>

      <FormGrid>
        <FormField id="contactPerson" label="Contact Person">
          <Input id="contactPerson" value={form.contactPerson} onChange={(e) => set("contactPerson", e.target.value)} />
        </FormField>
        <FormField id="phone" label="Phone">
          <Input id="phone" value={form.phone} onChange={(e) => set("phone", e.target.value)} />
        </FormField>
      </FormGrid>

      <FormField id="notes" label="Notes">
        <Input id="notes" value={form.notes} onChange={(e) => set("notes", e.target.value)} />
      </FormField>

      <FormActions onCancel={onCancel} saving={saving} saveLabel={warehouse ? "Update Warehouse" : "Create Warehouse"} />
    </form>
  );
}

export function WarehousesPage() {
  const [result, setResult] = useState<PagedResult<Warehouse> | null>(null);
  const [loading, setLoading] = useState(true);
  const [columnFilters, setColumnFilters] = useState<ColumnFiltersState>([]);
  const [sorting, setSorting] = useState<SortingState>([]);
  const [page, setPage] = useState(1);
  const [formOpen, setFormOpen] = useState(false);
  const [editingWarehouse, setEditingWarehouse] = useState<Warehouse | undefined>(undefined);
  const [deleteTarget, setDeleteTarget] = useState<Warehouse | null>(null);
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

  const fetchWarehouses = useCallback(() => {
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
    getWarehouses(params)
      .then(setResult)
      .finally(() => setLoading(false));
  }, [columnFilters, page, sorting]);

  useEffect(() => { fetchWarehouses(); }, [fetchWarehouses]);

  const handleSave = async (data: CreateWarehouseRequest | UpdateWarehouseRequest) => {
    if (editingWarehouse) {
      await updateWarehouse(editingWarehouse.id, data as UpdateWarehouseRequest);
    } else {
      await createWarehouse(data as CreateWarehouseRequest);
    }
    setFormOpen(false);
    setEditingWarehouse(undefined);
    fetchWarehouses();
  };

  const handleDelete = async () => {
    if (!deleteTarget) return;
    setDeleting(true);
    try {
      await deleteWarehouse(deleteTarget.id);
      setDeleteTarget(null);
      fetchWarehouses();
    } finally {
      setDeleting(false);
    }
  };

  const handleToggleActive = async (w: Warehouse) => {
    if (w.isActive) {
      await deactivateWarehouse(w.id);
    } else {
      await activateWarehouse(w.id);
    }
    fetchWarehouses();
  };

  const columns: ColumnDef<Warehouse>[] = [
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
      accessorKey: "location",
      header: "Location",
      cell: ({ row }) => <span className="text-muted-foreground">{row.original.location ?? "—"}</span>,
      enableSorting: false,
    },
    {
      accessorKey: "contactPerson",
      header: "Contact",
      cell: ({ row }) => <span className="text-muted-foreground">{row.original.contactPerson ?? "—"}</span>,
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
        const w = row.original;
        return (
          <div className="flex items-center justify-end gap-1">
            <Button variant="ghost" size="sm" onClick={() => handleToggleActive(w)} title={w.isActive ? "Deactivate" : "Activate"}>
              {w.isActive ? <XCircle className="h-4 w-4 text-orange-500" /> : <CheckCircle className="h-4 w-4 text-green-600" />}
            </Button>
            <Button variant="ghost" size="sm" onClick={() => { setEditingWarehouse(w); setFormOpen(true); }} title="Edit">
              <Edit className="h-4 w-4" />
            </Button>
            <Button variant="ghost" size="sm" onClick={() => setDeleteTarget(w)} title="Delete">
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
        title="Warehouses"
        description="Manage warehouse locations"
        action={
          <Button onClick={() => { setEditingWarehouse(undefined); setFormOpen(true); }}>
            <Plus className="mr-2 h-4 w-4" /> Add Warehouse
          </Button>
        }
      />

      <DataTable
        columns={columns}
        data={result?.items ?? []}
        loading={loading}
        emptyText="No warehouses found."
        filtering={{ state: columnFilters, onChange: handleFilteringChange, manual: true }}
        sorting={{ state: sorting, onChange: handleSortingChange, manual: true }}
        pagination={result ? { result, onPrevious: () => setPage((p) => p - 1), onNext: () => setPage((p) => p + 1) } : undefined}
      />

      <Dialog open={formOpen} onOpenChange={setFormOpen}>
        <DialogContent className="sm:max-w-lg">
          <DialogHeader><DialogTitle>{editingWarehouse ? "Edit Warehouse" : "Create Warehouse"}</DialogTitle></DialogHeader>
          <WarehouseForm warehouse={editingWarehouse} onSave={handleSave} onCancel={() => { setFormOpen(false); setEditingWarehouse(undefined); }} />
        </DialogContent>
      </Dialog>

      <DeleteDialog
        open={!!deleteTarget}
        onOpenChange={(open) => { if (!open) setDeleteTarget(null); }}
        title="Delete Warehouse"
        entityLabel={deleteTarget ? `${deleteTarget.name} (${deleteTarget.code})` : ""}
        onConfirm={handleDelete}
        deleting={deleting}
      />
    </PageLayout>
  );
}
