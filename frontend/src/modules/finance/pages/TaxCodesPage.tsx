import { useEffect, useState, useCallback } from "react";
import type { ColumnDef, ColumnFiltersState, OnChangeFn, SortingState } from "@tanstack/react-table";
import { Plus, Trash2, Edit } from "lucide-react";
import { Button } from "@app/components/ui/button";
import { Input } from "@app/components/ui/input";
import { Badge } from "@app/components/ui/badge";
import {
  Dialog, DialogContent, DialogHeader, DialogTitle,
} from "@app/components/ui/dialog";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@app/components/ui/select";
import {
  PageHeader, DeleteDialog, FormError,
  PageLayout, DataTable, FormField, FormGrid, FormActions, StatusBadge,
  ColumnFilterInput,
} from "@shared/components";
import { getTaxCodes, createTaxCode, updateTaxCode, deleteTaxCode } from "../api/financeApi";
import type { TaxCode, CreateTaxCodeRequest, UpdateTaxCodeRequest, PagedResult } from "../types";

const TAX_TYPES = [
  { value: "0", label: "Percentage" },
  { value: "1", label: "Fixed" },
];

function TaxCodeForm({
  taxCode,
  onSave,
  onCancel,
}: {
  taxCode?: TaxCode;
  onSave: (data: CreateTaxCodeRequest | UpdateTaxCodeRequest) => Promise<void>;
  onCancel: () => void;
}) {
  const [form, setForm] = useState({
    code: taxCode?.code ?? "",
    name: taxCode?.name ?? "",
    rate: taxCode?.rate?.toString() ?? "0",
    type: taxCode?.type?.toString() ?? "0",
    description: taxCode?.description ?? "",
    appliesToSales: taxCode?.appliesToSales ?? true,
    appliesToPurchases: taxCode?.appliesToPurchases ?? true,
  });
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState("");

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setSaving(true);
    setError("");
    try {
      const common = {
        name: form.name,
        rate: Number(form.rate),
        type: Number(form.type),
        description: form.description || undefined,
        appliesToSales: form.appliesToSales,
        appliesToPurchases: form.appliesToPurchases,
      };
      if (taxCode) {
        await onSave(common as UpdateTaxCodeRequest);
      } else {
        await onSave({ code: form.code, ...common } as CreateTaxCodeRequest);
      }
    } catch (err) {
      setError(err instanceof Error ? err.message : "An error occurred.");
      setSaving(false);
    }
  };

  return (
    <form onSubmit={handleSubmit} className="space-y-4">
      <FormError error={error} />

      {!taxCode && (
        <FormField id="code" label="Code">
          <Input id="code" required value={form.code} onChange={(e) => setForm((f) => ({ ...f, code: e.target.value }))} />
        </FormField>
      )}

      <FormField id="name" label="Name">
        <Input id="name" required value={form.name} onChange={(e) => setForm((f) => ({ ...f, name: e.target.value }))} />
      </FormField>

      <FormGrid>
        <FormField id="rate" label="Rate">
          <Input id="rate" type="number" step="0.0001" required value={form.rate} onChange={(e) => setForm((f) => ({ ...f, rate: e.target.value }))} />
        </FormField>
        <FormField id="type" label="Type">
          <Select value={form.type} onValueChange={(v) => setForm((f) => ({ ...f, type: v ?? "0" }))}>
            <SelectTrigger><SelectValue /></SelectTrigger>
            <SelectContent>
              {TAX_TYPES.map((t) => (
                <SelectItem key={t.value} value={t.value}>{t.label}</SelectItem>
              ))}
            </SelectContent>
          </Select>
        </FormField>
      </FormGrid>

      <FormField id="description" label="Description">
        <Input id="description" value={form.description} onChange={(e) => setForm((f) => ({ ...f, description: e.target.value }))} />
      </FormField>

      <div className="flex gap-6">
        <label className="flex items-center gap-2 text-sm">
          <input type="checkbox" checked={form.appliesToSales} onChange={(e) => setForm((f) => ({ ...f, appliesToSales: e.target.checked }))} className="rounded" />
          Applies to Sales
        </label>
        <label className="flex items-center gap-2 text-sm">
          <input type="checkbox" checked={form.appliesToPurchases} onChange={(e) => setForm((f) => ({ ...f, appliesToPurchases: e.target.checked }))} className="rounded" />
          Applies to Purchases
        </label>
      </div>

      <FormActions onCancel={onCancel} saving={saving} saveLabel={taxCode ? "Update Tax Code" : "Create Tax Code"} />
    </form>
  );
}

export function TaxCodesPage() {
  const [result, setResult] = useState<PagedResult<TaxCode> | null>(null);
  const [loading, setLoading] = useState(true);
  const [columnFilters, setColumnFilters] = useState<ColumnFiltersState>([]);
  const [sorting, setSorting] = useState<SortingState>([]);
  const [page, setPage] = useState(1);
  const [formOpen, setFormOpen] = useState(false);
  const [editingTaxCode, setEditingTaxCode] = useState<TaxCode | undefined>(undefined);
  const [deleteTarget, setDeleteTarget] = useState<TaxCode | null>(null);
  const [deleting, setDeleting] = useState(false);

  const mapSortField = (columnId: string) => {
    switch (columnId) {
      case "code":
        return "code";
      case "name":
        return "name";
      case "rate":
        return "rate";
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

  const fetchTaxCodes = useCallback(() => {
    const search = String(columnFilters.find((filter) => filter.id === "searchTerm")?.value ?? "");
    const params: Record<string, string> = { page: String(page), pageSize: "20" };
    if (search) params.searchTerm = search;
    if (sorting[0]) {
      params.sortBy = mapSortField(sorting[0].id);
      params.sortDescending = String(sorting[0].desc);
    }
    setLoading(true);
    getTaxCodes(params)
      .then(setResult)
      .finally(() => setLoading(false));
  }, [columnFilters, page, sorting]);

  useEffect(() => { fetchTaxCodes(); }, [fetchTaxCodes]);

  const handleSave = async (data: CreateTaxCodeRequest | UpdateTaxCodeRequest) => {
    if (editingTaxCode) {
      await updateTaxCode(editingTaxCode.id, data as UpdateTaxCodeRequest);
    } else {
      await createTaxCode(data as CreateTaxCodeRequest);
    }
    setFormOpen(false);
    setEditingTaxCode(undefined);
    fetchTaxCodes();
  };

  const handleDelete = async () => {
    if (!deleteTarget) return;
    setDeleting(true);
    try {
      await deleteTaxCode(deleteTarget.id);
      setDeleteTarget(null);
      fetchTaxCodes();
    } finally {
      setDeleting(false);
    }
  };

  const taxCodes = result?.items ?? [];

  const columns: ColumnDef<TaxCode>[] = [
    {
      accessorKey: "code",
      header: "Code",
      cell: ({ row }) => <span className="font-medium">{row.original.code}</span>,
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
    { accessorKey: "name", header: "Name" },
    {
      accessorKey: "rate",
      header: "Rate",
      cell: ({ row }) => {
        const tc = row.original;
        return tc.type === 0 ? `${tc.rate}%` : `$${tc.rate.toFixed(2)}`;
      },
    },
    {
      accessorKey: "typeName",
      header: "Type",
      cell: ({ row }) => <Badge variant="outline">{row.original.typeName}</Badge>,
      enableSorting: false,
    },
    {
      accessorKey: "appliesToSales",
      header: "Sales",
      cell: ({ row }) => row.original.appliesToSales ? "Yes" : "No",
      enableSorting: false,
    },
    {
      accessorKey: "appliesToPurchases",
      header: "Purchases",
      cell: ({ row }) => row.original.appliesToPurchases ? "Yes" : "No",
      enableSorting: false,
    },
    {
      accessorKey: "isActive",
      header: "Status",
      cell: ({ row }) => <StatusBadge isActive={row.original.isActive} />,
      enableSorting: false,
    },
    {
      id: "actions",
      header: "Actions",
      enableSorting: false,
      meta: { className: "text-right", headerClassName: "text-right" },
      cell: ({ row }) => {
        const tc = row.original;
        return (
          <div className="flex items-center justify-end gap-1">
            <Button variant="ghost" size="sm" onClick={() => { setEditingTaxCode(tc); setFormOpen(true); }} title="Edit">
              <Edit className="h-4 w-4" />
            </Button>
            <Button variant="ghost" size="sm" onClick={() => setDeleteTarget(tc)} title="Delete">
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
        title="Tax Codes"
        description="Manage tax codes for sales and purchases"
        action={
          <Button onClick={() => { setEditingTaxCode(undefined); setFormOpen(true); }}>
            <Plus className="mr-2 h-4 w-4" /> Add Tax Code
          </Button>
        }
      />

      <DataTable
        columns={columns}
        data={taxCodes}
        loading={loading}
        emptyText="No tax codes found."
        filtering={{ state: columnFilters, onChange: handleFilteringChange, manual: true }}
        sorting={{ state: sorting, onChange: handleSortingChange, manual: true }}
        pagination={result ? { result, onPrevious: () => setPage((p) => p - 1), onNext: () => setPage((p) => p + 1) } : undefined}
      />

      <Dialog open={formOpen} onOpenChange={setFormOpen}>
        <DialogContent className="sm:max-w-md">
          <DialogHeader><DialogTitle>{editingTaxCode ? "Edit Tax Code" : "Create Tax Code"}</DialogTitle></DialogHeader>
          <TaxCodeForm taxCode={editingTaxCode} onSave={handleSave} onCancel={() => { setFormOpen(false); setEditingTaxCode(undefined); }} />
        </DialogContent>
      </Dialog>

      <DeleteDialog
        open={!!deleteTarget}
        onOpenChange={(open) => { if (!open) setDeleteTarget(null); }}
        title="Delete Tax Code"
        entityLabel={deleteTarget ? `${deleteTarget.name} (${deleteTarget.code})` : ""}
        onConfirm={handleDelete}
        deleting={deleting}
      />
    </PageLayout>
  );
}
