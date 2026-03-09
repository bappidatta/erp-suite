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
import { getCustomers, createCustomer, updateCustomer, deleteCustomer, activateCustomer, deactivateCustomer } from "../api/salesApi";
import type { Customer, CreateCustomerRequest, UpdateCustomerRequest, PagedResult } from "../types";

const STATUS_OPTIONS = [
  { value: "", label: "All" },
  { value: "true", label: "Active" },
  { value: "false", label: "Inactive" },
];

function CustomerForm({
  customer,
  onSave,
  onCancel,
}: {
  customer?: Customer;
  onSave: (data: CreateCustomerRequest | UpdateCustomerRequest) => Promise<void>;
  onCancel: () => void;
}) {
  const [form, setForm] = useState({
    code: customer?.code ?? "",
    name: customer?.name ?? "",
    contactPerson: customer?.contactPerson ?? "",
    email: customer?.email ?? "",
    phone: customer?.phone ?? "",
    website: customer?.website ?? "",
    taxId: customer?.taxId ?? "",
    addressLine1: customer?.addressLine1 ?? "",
    addressLine2: customer?.addressLine2 ?? "",
    city: customer?.city ?? "",
    state: customer?.state ?? "",
    postalCode: customer?.postalCode ?? "",
    country: customer?.country ?? "",
    creditLimit: customer?.creditLimit?.toString() ?? "0",
    currency: customer?.currency ?? "USD",
    paymentTerms: customer?.paymentTerms ?? "",
    notes: customer?.notes ?? "",
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
        creditLimit: Number(form.creditLimit) || 0,
        currency: form.currency || "USD",
        paymentTerms: form.paymentTerms || undefined,
        notes: form.notes || undefined,
      };
      if (customer) {
        await onSave(common as UpdateCustomerRequest);
      } else {
        await onSave({ code: form.code, ...common } as CreateCustomerRequest);
      }
    } catch (err) {
      setError(err instanceof Error ? err.message : "An error occurred.");
      setSaving(false);
    }
  };

  return (
    <form onSubmit={handleSubmit} className="space-y-4 max-h-[70vh] overflow-y-auto pr-2">
      <FormError error={error} />

      {!customer && (
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
        <FormField id="creditLimit" label="Credit Limit">
          <Input id="creditLimit" type="number" step="0.01" value={form.creditLimit} onChange={(e) => set("creditLimit", e.target.value)} />
        </FormField>
        <FormField id="paymentTerms" label="Payment Terms">
          <Input id="paymentTerms" value={form.paymentTerms} onChange={(e) => set("paymentTerms", e.target.value)} />
        </FormField>
      </FormGrid>

      <FormField id="notes" label="Notes">
        <Input id="notes" value={form.notes} onChange={(e) => set("notes", e.target.value)} />
      </FormField>

      <FormActions onCancel={onCancel} saving={saving} saveLabel={customer ? "Update Customer" : "Create Customer"} />
    </form>
  );
}

export function CustomersPage() {
  const [result, setResult] = useState<PagedResult<Customer> | null>(null);
  const [loading, setLoading] = useState(true);
  const [columnFilters, setColumnFilters] = useState<ColumnFiltersState>([]);
  const [sorting, setSorting] = useState<SortingState>([]);
  const [page, setPage] = useState(1);
  const [formOpen, setFormOpen] = useState(false);
  const [editingCustomer, setEditingCustomer] = useState<Customer | undefined>(undefined);
  const [deleteTarget, setDeleteTarget] = useState<Customer | null>(null);
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

  const fetchCustomers = useCallback(() => {
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
    getCustomers(params)
      .then(setResult)
      .finally(() => setLoading(false));
  }, [columnFilters, page, sorting]);

  useEffect(() => { fetchCustomers(); }, [fetchCustomers]);

  const handleSave = async (data: CreateCustomerRequest | UpdateCustomerRequest) => {
    if (editingCustomer) {
      await updateCustomer(editingCustomer.id, data as UpdateCustomerRequest);
    } else {
      await createCustomer(data as CreateCustomerRequest);
    }
    setFormOpen(false);
    setEditingCustomer(undefined);
    fetchCustomers();
  };

  const handleDelete = async () => {
    if (!deleteTarget) return;
    setDeleting(true);
    try {
      await deleteCustomer(deleteTarget.id);
      setDeleteTarget(null);
      fetchCustomers();
    } finally {
      setDeleting(false);
    }
  };

  const handleToggleActive = async (c: Customer) => {
    if (c.isActive) {
      await deactivateCustomer(c.id);
    } else {
      await activateCustomer(c.id);
    }
    fetchCustomers();
  };

  const customers = result?.items ?? [];

  const columns: ColumnDef<Customer>[] = [
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
        const c = row.original;
        return (
          <div className="flex items-center justify-end gap-1">
            <Button variant="ghost" size="sm" onClick={() => handleToggleActive(c)} title={c.isActive ? "Deactivate" : "Activate"}>
              {c.isActive ? <XCircle className="h-4 w-4 text-orange-500" /> : <CheckCircle className="h-4 w-4 text-green-600" />}
            </Button>
            <Button variant="ghost" size="sm" onClick={() => { setEditingCustomer(c); setFormOpen(true); }} title="Edit">
              <Edit className="h-4 w-4" />
            </Button>
            <Button variant="ghost" size="sm" onClick={() => setDeleteTarget(c)} title="Delete">
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
        title="Customers"
        description="Manage customer records"
        action={
          <Button onClick={() => { setEditingCustomer(undefined); setFormOpen(true); }}>
            <Plus className="mr-2 h-4 w-4" /> Add Customer
          </Button>
        }
      />

      <DataTable
        columns={columns}
        data={customers}
        loading={loading}
        emptyText="No customers found."
        filtering={{ state: columnFilters, onChange: handleFilteringChange, manual: true }}
        sorting={{ state: sorting, onChange: handleSortingChange, manual: true }}
        pagination={result ? { result, onPrevious: () => setPage((p) => p - 1), onNext: () => setPage((p) => p + 1) } : undefined}
      />

      <Dialog open={formOpen} onOpenChange={setFormOpen}>
        <DialogContent className="sm:max-w-lg">
          <DialogHeader><DialogTitle>{editingCustomer ? "Edit Customer" : "Create Customer"}</DialogTitle></DialogHeader>
          <CustomerForm customer={editingCustomer} onSave={handleSave} onCancel={() => { setFormOpen(false); setEditingCustomer(undefined); }} />
        </DialogContent>
      </Dialog>

      <DeleteDialog
        open={!!deleteTarget}
        onOpenChange={(open) => { if (!open) setDeleteTarget(null); }}
        title="Delete Customer"
        entityLabel={deleteTarget ? `${deleteTarget.name} (${deleteTarget.code})` : ""}
        onConfirm={handleDelete}
        deleting={deleting}
      />
    </PageLayout>
  );
}
