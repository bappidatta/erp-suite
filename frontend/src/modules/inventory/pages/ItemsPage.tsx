import { useEffect, useState, useCallback } from "react";
import type { ColumnDef, ColumnFiltersState, OnChangeFn, SortingState } from "@tanstack/react-table";
import { Plus, Trash2, Edit, CheckCircle, XCircle } from "lucide-react";
import { Button } from "@app/components/ui/button";
import { Input } from "@app/components/ui/input";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@app/components/ui/select";
import {
  Dialog, DialogContent, DialogHeader, DialogTitle,
} from "@app/components/ui/dialog";
import {
  PageHeader, DeleteDialog, FormError,
  PageLayout, DataTable, FormField, FormGrid, FormActions, StatusBadge,
  ColumnFilterInput, ColumnFilterSelect,
} from "@shared/components";
import { getItems, createItem, updateItem, deleteItem, activateItem, deactivateItem, getUoms, getCategories } from "../api/inventoryApi";
import type { Item, CreateItemRequest, UpdateItemRequest, PagedResult, UnitOfMeasure, Category } from "../types";

const STATUS_OPTIONS = [
  { value: "", label: "All" },
  { value: "true", label: "Active" },
  { value: "false", label: "Inactive" },
];

const NONE_OPTION_VALUE = "__none__";

const ITEM_TYPE_OPTIONS = [
  { value: "1", label: "Product" },
  { value: "2", label: "Service" },
  { value: "3", label: "Raw Material" },
  { value: "4", label: "Semi Finished" },
];

const VALUATION_OPTIONS = [
  { value: "1", label: "Weighted Average" },
  { value: "2", label: "FIFO" },
  { value: "3", label: "Standard Cost" },
];

const getItemTypeName = (type: number) =>
  ITEM_TYPE_OPTIONS.find((o) => o.value === String(type))?.label ?? String(type);

function ItemForm({
  item,
  uoms,
  categories,
  onSave,
  onCancel,
}: {
  item?: Item;
  uoms: UnitOfMeasure[];
  categories: Category[];
  onSave: (data: CreateItemRequest | UpdateItemRequest) => Promise<void>;
  onCancel: () => void;
}) {
  const [form, setForm] = useState({
    code: item?.code ?? "",
    name: item?.name ?? "",
    description: item?.description ?? "",
    uomId: item?.uomId?.toString() ?? "",
    categoryId: item?.categoryId?.toString() ?? "",
    type: item?.type?.toString() ?? "1",
    valuationMethod: item?.valuationMethod?.toString() ?? "1",
    standardCost: item?.standardCost?.toString() ?? "0",
    salePrice: item?.salePrice?.toString() ?? "0",
    reorderLevel: item?.reorderLevel?.toString() ?? "0",
    barcode: item?.barcode ?? "",
    notes: item?.notes ?? "",
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
        uomId: Number(form.uomId),
        categoryId: form.categoryId ? Number(form.categoryId) : undefined,
        type: Number(form.type),
        valuationMethod: Number(form.valuationMethod),
        standardCost: Number(form.standardCost) || 0,
        salePrice: Number(form.salePrice) || 0,
        reorderLevel: Number(form.reorderLevel) || 0,
        barcode: form.barcode || undefined,
        notes: form.notes || undefined,
      };
      if (item) {
        await onSave(common as UpdateItemRequest);
      } else {
        await onSave({ code: form.code, ...common } as CreateItemRequest);
      }
    } catch (err) {
      setError(err instanceof Error ? err.message : "An error occurred.");
      setSaving(false);
    }
  };

  return (
    <form onSubmit={handleSubmit} className="space-y-4 max-h-[70vh] overflow-y-auto pr-2">
      <FormError error={error} />

      {!item && (
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

      <FormGrid>
        <FormField id="uomId" label="Unit of Measure">
          <Select value={form.uomId} onValueChange={(v) => set("uomId", v ?? "")}>
            <SelectTrigger id="uomId"><SelectValue placeholder="Select UOM" /></SelectTrigger>
            <SelectContent>
              {uoms.map((u) => (
                <SelectItem key={u.id} value={u.id.toString()}>{u.name} ({u.code})</SelectItem>
              ))}
            </SelectContent>
          </Select>
        </FormField>
        <FormField id="categoryId" label="Category">
          <Select value={form.categoryId} onValueChange={(v) => set("categoryId", v === NONE_OPTION_VALUE ? "" : (v ?? ""))}>
            <SelectTrigger id="categoryId"><SelectValue placeholder="None" /></SelectTrigger>
            <SelectContent>
              <SelectItem value={NONE_OPTION_VALUE}>None</SelectItem>
              {categories.map((c) => (
                <SelectItem key={c.id} value={c.id.toString()}>{c.name}</SelectItem>
              ))}
            </SelectContent>
          </Select>
        </FormField>
      </FormGrid>

      <FormGrid>
        <FormField id="type" label="Item Type">
          <Select value={form.type} onValueChange={(v) => set("type", v ?? "1")}>
            <SelectTrigger id="type"><SelectValue /></SelectTrigger>
            <SelectContent>
              {ITEM_TYPE_OPTIONS.map((o) => (
                <SelectItem key={o.value} value={o.value}>{o.label}</SelectItem>
              ))}
            </SelectContent>
          </Select>
        </FormField>
        <FormField id="valuationMethod" label="Valuation Method">
          <Select value={form.valuationMethod} onValueChange={(v) => set("valuationMethod", v ?? "1")}>
            <SelectTrigger id="valuationMethod"><SelectValue /></SelectTrigger>
            <SelectContent>
              {VALUATION_OPTIONS.map((o) => (
                <SelectItem key={o.value} value={o.value}>{o.label}</SelectItem>
              ))}
            </SelectContent>
          </Select>
        </FormField>
      </FormGrid>

      <FormGrid cols={3}>
        <FormField id="standardCost" label="Standard Cost">
          <Input id="standardCost" type="number" step="0.0001" value={form.standardCost} onChange={(e) => set("standardCost", e.target.value)} />
        </FormField>
        <FormField id="salePrice" label="Sale Price">
          <Input id="salePrice" type="number" step="0.0001" value={form.salePrice} onChange={(e) => set("salePrice", e.target.value)} />
        </FormField>
        <FormField id="reorderLevel" label="Reorder Level">
          <Input id="reorderLevel" type="number" step="0.0001" value={form.reorderLevel} onChange={(e) => set("reorderLevel", e.target.value)} />
        </FormField>
      </FormGrid>

      <FormField id="barcode" label="Barcode">
        <Input id="barcode" value={form.barcode} onChange={(e) => set("barcode", e.target.value)} />
      </FormField>

      <FormField id="notes" label="Notes">
        <Input id="notes" value={form.notes} onChange={(e) => set("notes", e.target.value)} />
      </FormField>

      <FormActions onCancel={onCancel} saving={saving} disabled={!form.uomId} saveLabel={item ? "Update Item" : "Create Item"} />
    </form>
  );
}

export function ItemsPage() {
  const [result, setResult] = useState<PagedResult<Item> | null>(null);
  const [uoms, setUoms] = useState<UnitOfMeasure[]>([]);
  const [categories, setCategories] = useState<Category[]>([]);
  const [loading, setLoading] = useState(true);
  const [columnFilters, setColumnFilters] = useState<ColumnFiltersState>([]);
  const [sorting, setSorting] = useState<SortingState>([]);
  const [page, setPage] = useState(1);
  const [formOpen, setFormOpen] = useState(false);
  const [editingItem, setEditingItem] = useState<Item | undefined>(undefined);
  const [deleteTarget, setDeleteTarget] = useState<Item | null>(null);
  const [deleting, setDeleting] = useState(false);

  useEffect(() => {
    getUoms({ pageSize: "500" }).then((r) => setUoms(r.items)).catch((err) => console.warn("Failed to load UOMs:", err));
    getCategories({ pageSize: "500" }).then((r) => setCategories(r.items)).catch((err) => console.warn("Failed to load categories:", err));
  }, []);

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

  const fetchItems = useCallback(() => {
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
    getItems(params)
      .then(setResult)
      .finally(() => setLoading(false));
  }, [columnFilters, page, sorting]);

  useEffect(() => { fetchItems(); }, [fetchItems]);

  const handleSave = async (data: CreateItemRequest | UpdateItemRequest) => {
    if (editingItem) {
      await updateItem(editingItem.id, data as UpdateItemRequest);
    } else {
      await createItem(data as CreateItemRequest);
    }
    setFormOpen(false);
    setEditingItem(undefined);
    fetchItems();
  };

  const handleDelete = async () => {
    if (!deleteTarget) return;
    setDeleting(true);
    try {
      await deleteItem(deleteTarget.id);
      setDeleteTarget(null);
      fetchItems();
    } finally {
      setDeleting(false);
    }
  };

  const handleToggleActive = async (i: Item) => {
    if (i.isActive) {
      await deactivateItem(i.id);
    } else {
      await activateItem(i.id);
    }
    fetchItems();
  };

  const columns: ColumnDef<Item>[] = [
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
      accessorKey: "uomCode",
      header: "UOM",
      cell: ({ row }) => <span className="text-muted-foreground">{row.original.uomCode}</span>,
      enableSorting: false,
    },
    {
      accessorKey: "type",
      header: "Type",
      cell: ({ row }) => <span className="text-muted-foreground">{getItemTypeName(row.original.type)}</span>,
      enableSorting: false,
    },
    {
      accessorKey: "salePrice",
      header: "Sale Price",
      cell: ({ row }) => <span className="text-right block">{row.original.salePrice.toFixed(2)}</span>,
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
        const i = row.original;
        return (
          <div className="flex items-center justify-end gap-1">
            <Button variant="ghost" size="sm" onClick={() => handleToggleActive(i)} title={i.isActive ? "Deactivate" : "Activate"}>
              {i.isActive ? <XCircle className="h-4 w-4 text-orange-500" /> : <CheckCircle className="h-4 w-4 text-green-600" />}
            </Button>
            <Button variant="ghost" size="sm" onClick={() => { setEditingItem(i); setFormOpen(true); }} title="Edit">
              <Edit className="h-4 w-4" />
            </Button>
            <Button variant="ghost" size="sm" onClick={() => setDeleteTarget(i)} title="Delete">
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
        title="Items"
        description="Manage inventory items and products"
        action={
          <Button onClick={() => { setEditingItem(undefined); setFormOpen(true); }}>
            <Plus className="mr-2 h-4 w-4" /> Add Item
          </Button>
        }
      />

      <DataTable
        columns={columns}
        data={result?.items ?? []}
        loading={loading}
        emptyText="No items found."
        filtering={{ state: columnFilters, onChange: handleFilteringChange, manual: true }}
        sorting={{ state: sorting, onChange: handleSortingChange, manual: true }}
        pagination={result ? { result, onPrevious: () => setPage((p) => p - 1), onNext: () => setPage((p) => p + 1) } : undefined}
      />

      <Dialog open={formOpen} onOpenChange={setFormOpen}>
        <DialogContent className="sm:max-w-lg">
          <DialogHeader><DialogTitle>{editingItem ? "Edit Item" : "Create Item"}</DialogTitle></DialogHeader>
          <ItemForm item={editingItem} uoms={uoms} categories={categories} onSave={handleSave} onCancel={() => { setFormOpen(false); setEditingItem(undefined); }} />
        </DialogContent>
      </Dialog>

      <DeleteDialog
        open={!!deleteTarget}
        onOpenChange={(open) => { if (!open) setDeleteTarget(null); }}
        title="Delete Item"
        entityLabel={deleteTarget ? `${deleteTarget.name} (${deleteTarget.code})` : ""}
        onConfirm={handleDelete}
        deleting={deleting}
      />
    </PageLayout>
  );
}
