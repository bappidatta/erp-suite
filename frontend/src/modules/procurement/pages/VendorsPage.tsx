import { useEffect, useState, useCallback } from "react";
import type { ColumnDef, ColumnFiltersState, OnChangeFn, SortingState } from "@tanstack/react-table";
import { Plus, Trash2, Edit, CheckCircle, XCircle } from "lucide-react";
import { Button } from "@app/components/ui/button";
import { Input } from "@app/components/ui/input";
import {
  Dialog, DialogContent, DialogHeader, DialogTitle,
} from "@app/components/ui/dialog";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@app/components/ui/select";
import {
  PageHeader, DeleteDialog, FormError,
  PageLayout, DataTable, FormField, FormGrid, FormActions, StatusBadge,
  ColumnFilterInput, ColumnFilterSelect,
} from "@shared/components";
import { getVendors, createVendor, updateVendor, deleteVendor, activateVendor, deactivateVendor } from "../api/procurementApi";
import type { Vendor, CreateVendorRequest, UpdateVendorRequest, PagedResult } from "../types";

const STATUS_OPTIONS = [
  { value: "", label: "All" },
  { value: "true", label: "Active" },
  { value: "false", label: "Inactive" },
];

function VendorForm({
  vendor,
  onSave,
  onCancel,
}: {
  vendor?: Vendor;
  onSave: (data: CreateVendorRequest | UpdateVendorRequest) => Promise<void>;
  onCancel: () => void;
}) {
  const [form, setForm] = useState({
    code: vendor?.code ?? "",
    name: vendor?.name ?? "",
    contactPerson: vendor?.contactPerson ?? "",
    email: vendor?.email ?? "",
    phone: vendor?.phone ?? "",
    website: vendor?.website ?? "",
    taxId: vendor?.taxId ?? "",
    addressLine1: vendor?.addressLine1 ?? "",
    addressLine2: vendor?.addressLine2 ?? "",
    city: vendor?.city ?? "",
    state: vendor?.state ?? "",
    postalCode: vendor?.postalCode ?? "",
    country: vendor?.country ?? "",
    currency: vendor?.currency ?? "USD",
    paymentTerms: vendor?.paymentTerms ?? "",
    bankName: vendor?.bankName ?? "",
    bankAccountNumber: vendor?.bankAccountNumber ?? "",
    bankRoutingNumber: vendor?.bankRoutingNumber ?? "",
    bankSwiftCode: vendor?.bankSwiftCode ?? "",
    leadTimeDays: vendor?.leadTimeDays?.toString() ?? "0",
    notes: vendor?.notes ?? "",
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
        contactPerson: form.contactPerson || undefined,
        email: form.email || undefined,
        phone: form.phone || undefined,
        website: form.website || undefined,
        taxId: form.taxId || undefined,
        addressLine1: form.addressLine1 || undefined,
        addressLine2: form.addressLine2 || undefined,
        city: form.city || undefined,
        state: form.state || undefined,
        postalCode: form.postalCode || undefined,
        country: form.country || undefined,
        currency: form.currency || "USD",
        paymentTerms: form.paymentTerms || undefined,
        bankName: form.bankName || undefined,
        bankAccountNumber: form.bankAccountNumber || undefined,
        bankRoutingNumber: form.bankRoutingNumber || undefined,
        bankSwiftCode: form.bankSwiftCode || undefined,
        leadTimeDays: Number(form.leadTimeDays) || 0,
        notes: form.notes || undefined,
      };
      if (vendor) {
        await onSave(common as UpdateVendorRequest);
      } else {
        await onSave({ code: form.code, ...common } as CreateVendorRequest);
      }
    } catch (err) {
      setError(err instanceof Error ? err.message : "An error occurred.");
      setSaving(false);
    }
  };

  return (
    <form onSubmit={handleSubmit} className="space-y-4 max-h-[70vh] overflow-y-auto pr-2">
      <FormError error={error} />

      {!vendor && (
        <FormField id="code" label="Code">
          <Input id="code" required value={form.code} onChange={(e) => set("code", e.target.value)} />
        </FormField>
      )}

      <FormField id="name" label="Name">
        <Input id="name" required value={form.name} onChange={(e) => set("name", e.target.value)} />
      </FormField>

      <FormGrid>
        <FormField id="contactPerson" label="Contact Person">
          <Input id="contactPerson" value={form.contactPerson} onChange={(e) => set("contactPerson", e.target.value)} />
        </FormField>
        <FormField id="email" label="Email">
          <Input id="email" type="email" value={form.email} onChange={(e) => set("email", e.target.value)} />
        </FormField>
      </FormGrid>

      <FormGrid>
        <FormField id="phone" label="Phone">
          <Input id="phone" value={form.phone} onChange={(e) => set("phone", e.target.value)} />
        </FormField>
        <FormField id="website" label="Website">
          <Input id="website" value={form.website} onChange={(e) => set("website", e.target.value)} />
        </FormField>
      </FormGrid>

      <FormField id="taxId" label="Tax ID">
        <Input id="taxId" value={form.taxId} onChange={(e) => set("taxId", e.target.value)} />
      </FormField>

      <FormField id="addressLine1" label="Address Line 1">
        <Input id="addressLine1" value={form.addressLine1} onChange={(e) => set("addressLine1", e.target.value)} />
      </FormField>
      <FormField id="addressLine2" label="Address Line 2">
        <Input id="addressLine2" value={form.addressLine2} onChange={(e) => set("addressLine2", e.target.value)} />
      </FormField>

      <FormGrid cols={3}>
        <FormField id="city" label="City">
          <Input id="city" value={form.city} onChange={(e) => set("city", e.target.value)} />
        </FormField>
        <FormField id="state" label="State">
          <Input id="state" value={form.state} onChange={(e) => set("state", e.target.value)} />
        </FormField>
        <FormField id="postalCode" label="Postal Code">
          <Input id="postalCode" value={form.postalCode} onChange={(e) => set("postalCode", e.target.value)} />
        </FormField>
      </FormGrid>

      <FormGrid>
        <FormField id="country" label="Country">
          <Input id="country" value={form.country} onChange={(e) => set("country", e.target.value)} />
        </FormField>
        <FormField id="currency" label="Currency">
          <Input id="currency" maxLength={3} value={form.currency} onChange={(e) => set("currency", e.target.value)} />
        </FormField>
      </FormGrid>

      <FormGrid>
        <FormField id="paymentTerms" label="Payment Terms">
          <Input id="paymentTerms" value={form.paymentTerms} onChange={(e) => set("paymentTerms", e.target.value)} />
        </FormField>
        <FormField id="leadTimeDays" label="Lead Time (Days)">
          <Input id="leadTimeDays" type="number" min="0" value={form.leadTimeDays} onChange={(e) => set("leadTimeDays", e.target.value)} />
        </FormField>
      </FormGrid>

      <fieldset className="border rounded-md p-3 space-y-3">
        <legend className="text-sm font-medium px-1">Banking Details</legend>
        <FormGrid>
          <FormField id="bankName" label="Bank Name">
            <Input id="bankName" value={form.bankName} onChange={(e) => set("bankName", e.target.value)} />
          </FormField>
          <FormField id="bankAccountNumber" label="Account Number">
            <Input id="bankAccountNumber" value={form.bankAccountNumber} onChange={(e) => set("bankAccountNumber", e.target.value)} />
          </FormField>
        </FormGrid>
        <FormGrid>
          <FormField id="bankRoutingNumber" label="Routing Number">
            <Input id="bankRoutingNumber" value={form.bankRoutingNumber} onChange={(e) => set("bankRoutingNumber", e.target.value)} />
          </FormField>
          <FormField id="bankSwiftCode" label="SWIFT Code">
            <Input id="bankSwiftCode" value={form.bankSwiftCode} onChange={(e) => set("bankSwiftCode", e.target.value)} />
          </FormField>
        </FormGrid>
      </fieldset>

      <FormField id="notes" label="Notes">
        <Input id="notes" value={form.notes} onChange={(e) => set("notes", e.target.value)} />
      </FormField>

      <FormActions onCancel={onCancel} saving={saving} saveLabel={vendor ? "Update Vendor" : "Create Vendor"} />
    </form>
  );
}

export function VendorsPage() {
  const [result, setResult] = useState<PagedResult<Vendor> | null>(null);
  const [loading, setLoading] = useState(true);
  const [columnFilters, setColumnFilters] = useState<ColumnFiltersState>([]);
  const [sorting, setSorting] = useState<SortingState>([]);
  const [page, setPage] = useState(1);
  const [formOpen, setFormOpen] = useState(false);
  const [editingVendor, setEditingVendor] = useState<Vendor | undefined>(undefined);
  const [deleteTarget, setDeleteTarget] = useState<Vendor | null>(null);
  const [deleting, setDeleting] = useState(false);

  const mapSortField = (columnId: string) => {
    switch (columnId) {
      case "code":
        return "code";
      case "name":
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

  const fetchVendors = useCallback(() => {
    const search = String(columnFilters.find((filter) => filter.id === "searchTerm")?.value ?? "");
    const statusFilter = String(columnFilters.find((filter) => filter.id === "isActive")?.value ?? "");
    const params: Record<string, string> = { page: String(page), pageSize: "20" };
    if (search) params.searchTerm = search;
    if (statusFilter) params.isActive = statusFilter;
    if (sorting[0]) {
      params.sortBy = mapSortField(sorting[0].id);
      params.sortDescending = String(sorting[0].desc);
    }
    setLoading(true);
    getVendors(params)
      .then(setResult)
      .finally(() => setLoading(false));
  }, [columnFilters, page, sorting]);

  useEffect(() => { fetchVendors(); }, [fetchVendors]);

  const handleSave = async (data: CreateVendorRequest | UpdateVendorRequest) => {
    if (editingVendor) {
      await updateVendor(editingVendor.id, data as UpdateVendorRequest);
    } else {
      await createVendor(data as CreateVendorRequest);
    }
    setFormOpen(false);
    setEditingVendor(undefined);
    fetchVendors();
  };

  const handleDelete = async () => {
    if (!deleteTarget) return;
    setDeleting(true);
    try {
      await deleteVendor(deleteTarget.id);
      setDeleteTarget(null);
      fetchVendors();
    } finally {
      setDeleting(false);
    }
  };

  const handleToggleActive = async (v: Vendor) => {
    if (v.isActive) {
      await deactivateVendor(v.id);
    } else {
      await activateVendor(v.id);
    }
    fetchVendors();
  };

  const vendors = result?.items ?? [];

  const columns: ColumnDef<Vendor>[] = [
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
      accessorKey: "contactPerson",
      header: "Contact",
      cell: ({ row }) => <span className="text-muted-foreground">{row.original.contactPerson ?? "—"}</span>,
      enableSorting: false,
    },
    {
      accessorKey: "city",
      header: "City",
      cell: ({ row }) => <span className="text-muted-foreground">{row.original.city ?? "—"}</span>,
      enableSorting: false,
    },
    {
      accessorKey: "leadTimeDays",
      header: "Lead Time",
      cell: ({ row }) => <span className="text-muted-foreground">{row.original.leadTimeDays}d</span>,
      enableSorting: false,
    },
    {
      accessorKey: "isActive",
      header: "Status",
      cell: ({ row }) => <StatusBadge isActive={row.original.isActive} />,
      enableSorting: false,
      meta: {
        filterComponent: ({ value, onChange }) => (
          <ColumnFilterSelect
            value={value}
            onChange={onChange}
            options={STATUS_OPTIONS}
            placeholder="All"
          />
        ),
      },
    },
    {
      id: "actions",
      header: "Actions",
      enableSorting: false,
      meta: { className: "text-right", headerClassName: "text-right" },
      cell: ({ row }) => {
        const v = row.original;
        return (
          <div className="flex items-center justify-end gap-1">
            <Button variant="ghost" size="sm" onClick={() => handleToggleActive(v)} title={v.isActive ? "Deactivate" : "Activate"}>
              {v.isActive ? <XCircle className="h-4 w-4 text-orange-500" /> : <CheckCircle className="h-4 w-4 text-green-600" />}
            </Button>
            <Button variant="ghost" size="sm" onClick={() => { setEditingVendor(v); setFormOpen(true); }} title="Edit">
              <Edit className="h-4 w-4" />
            </Button>
            <Button variant="ghost" size="sm" onClick={() => setDeleteTarget(v)} title="Delete">
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
        title="Vendors"
        description="Manage vendor records"
        action={
          <Button onClick={() => { setEditingVendor(undefined); setFormOpen(true); }}>
            <Plus className="mr-2 h-4 w-4" /> Add Vendor
          </Button>
        }
      />

      <DataTable
        columns={columns}
        data={vendors}
        loading={loading}
        emptyText="No vendors found."
        filtering={{ state: columnFilters, onChange: handleFilteringChange, manual: true }}
        sorting={{ state: sorting, onChange: handleSortingChange, manual: true }}
        pagination={result ? { result, onPrevious: () => setPage((p) => p - 1), onNext: () => setPage((p) => p + 1) } : undefined}
      />

      <Dialog open={formOpen} onOpenChange={setFormOpen}>
        <DialogContent className="sm:max-w-lg">
          <DialogHeader><DialogTitle>{editingVendor ? "Edit Vendor" : "Create Vendor"}</DialogTitle></DialogHeader>
          <VendorForm vendor={editingVendor} onSave={handleSave} onCancel={() => { setFormOpen(false); setEditingVendor(undefined); }} />
        </DialogContent>
      </Dialog>

      <DeleteDialog
        open={!!deleteTarget}
        onOpenChange={(open) => { if (!open) setDeleteTarget(null); }}
        title="Delete Vendor"
        entityLabel={deleteTarget ? `${deleteTarget.name} (${deleteTarget.code})` : ""}
        onConfirm={handleDelete}
        deleting={deleting}
      />
    </PageLayout>
  );
}
