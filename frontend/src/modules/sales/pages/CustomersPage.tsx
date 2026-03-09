import { useEffect, useState, useCallback } from "react";
import { Plus, Search, Trash2, Edit, CheckCircle, XCircle } from "lucide-react";
import { Card, CardContent } from "@app/components/ui/card";
import { Button } from "@app/components/ui/button";
import { Input } from "@app/components/ui/input";
import { Badge } from "@app/components/ui/badge";
import {
  Table, TableBody, TableCell, TableHead, TableHeader, TableRow,
} from "@app/components/ui/table";
import {
  Dialog, DialogContent, DialogHeader, DialogTitle,
} from "@app/components/ui/dialog";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@app/components/ui/select";
import { Label } from "@app/components/ui/label";
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
      {error && <div className="rounded-md bg-destructive/10 p-3 text-sm text-destructive">{error}</div>}

      {!customer && (
        <div className="space-y-1">
          <Label htmlFor="code">Code</Label>
          <Input id="code" required value={form.code} onChange={(e) => set("code", e.target.value)} />
        </div>
      )}

      <div className="space-y-1">
        <Label htmlFor="name">Name</Label>
        <Input id="name" required value={form.name} onChange={(e) => set("name", e.target.value)} />
      </div>

      <div className="grid grid-cols-2 gap-3">
        <div className="space-y-1">
          <Label htmlFor="contactPerson">Contact Person</Label>
          <Input id="contactPerson" value={form.contactPerson} onChange={(e) => set("contactPerson", e.target.value)} />
        </div>
        <div className="space-y-1">
          <Label htmlFor="email">Email</Label>
          <Input id="email" type="email" value={form.email} onChange={(e) => set("email", e.target.value)} />
        </div>
      </div>

      <div className="grid grid-cols-2 gap-3">
        <div className="space-y-1">
          <Label htmlFor="phone">Phone</Label>
          <Input id="phone" value={form.phone} onChange={(e) => set("phone", e.target.value)} />
        </div>
        <div className="space-y-1">
          <Label htmlFor="website">Website</Label>
          <Input id="website" value={form.website} onChange={(e) => set("website", e.target.value)} />
        </div>
      </div>

      <div className="space-y-1">
        <Label htmlFor="taxId">Tax ID</Label>
        <Input id="taxId" value={form.taxId} onChange={(e) => set("taxId", e.target.value)} />
      </div>

      <div className="space-y-1">
        <Label htmlFor="addressLine1">Address Line 1</Label>
        <Input id="addressLine1" value={form.addressLine1} onChange={(e) => set("addressLine1", e.target.value)} />
      </div>
      <div className="space-y-1">
        <Label htmlFor="addressLine2">Address Line 2</Label>
        <Input id="addressLine2" value={form.addressLine2} onChange={(e) => set("addressLine2", e.target.value)} />
      </div>

      <div className="grid grid-cols-3 gap-3">
        <div className="space-y-1">
          <Label htmlFor="city">City</Label>
          <Input id="city" value={form.city} onChange={(e) => set("city", e.target.value)} />
        </div>
        <div className="space-y-1">
          <Label htmlFor="state">State</Label>
          <Input id="state" value={form.state} onChange={(e) => set("state", e.target.value)} />
        </div>
        <div className="space-y-1">
          <Label htmlFor="postalCode">Postal Code</Label>
          <Input id="postalCode" value={form.postalCode} onChange={(e) => set("postalCode", e.target.value)} />
        </div>
      </div>

      <div className="grid grid-cols-2 gap-3">
        <div className="space-y-1">
          <Label htmlFor="country">Country</Label>
          <Input id="country" value={form.country} onChange={(e) => set("country", e.target.value)} />
        </div>
        <div className="space-y-1">
          <Label htmlFor="currency">Currency</Label>
          <Input id="currency" maxLength={3} value={form.currency} onChange={(e) => set("currency", e.target.value)} />
        </div>
      </div>

      <div className="grid grid-cols-2 gap-3">
        <div className="space-y-1">
          <Label htmlFor="creditLimit">Credit Limit</Label>
          <Input id="creditLimit" type="number" step="0.01" value={form.creditLimit} onChange={(e) => set("creditLimit", e.target.value)} />
        </div>
        <div className="space-y-1">
          <Label htmlFor="paymentTerms">Payment Terms</Label>
          <Input id="paymentTerms" value={form.paymentTerms} onChange={(e) => set("paymentTerms", e.target.value)} />
        </div>
      </div>

      <div className="space-y-1">
        <Label htmlFor="notes">Notes</Label>
        <Input id="notes" value={form.notes} onChange={(e) => set("notes", e.target.value)} />
      </div>

      <div className="flex justify-end gap-2 pt-2">
        <Button type="button" variant="outline" onClick={onCancel} disabled={saving}>Cancel</Button>
        <Button type="submit" disabled={saving}>
          {saving ? "Saving…" : customer ? "Update Customer" : "Create Customer"}
        </Button>
      </div>
    </form>
  );
}

export function CustomersPage() {
  const [result, setResult] = useState<PagedResult<Customer> | null>(null);
  const [loading, setLoading] = useState(true);
  const [search, setSearch] = useState("");
  const [statusFilter, setStatusFilter] = useState("");
  const [page, setPage] = useState(1);
  const [formOpen, setFormOpen] = useState(false);
  const [editingCustomer, setEditingCustomer] = useState<Customer | undefined>(undefined);
  const [deleteTarget, setDeleteTarget] = useState<Customer | null>(null);
  const [deleting, setDeleting] = useState(false);

  const fetchCustomers = useCallback(() => {
    const params: Record<string, string> = { page: String(page), pageSize: "20" };
    if (search) params.searchTerm = search;
    if (statusFilter) params.isActive = statusFilter;
    setLoading(true);
    getCustomers(params)
      .then(setResult)
      .finally(() => setLoading(false));
  }, [page, search, statusFilter]);

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

  return (
    <div className="space-y-4">
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-2xl font-bold tracking-tight">Customers</h1>
          <p className="text-muted-foreground">Manage customer records</p>
        </div>
        <Button onClick={() => { setEditingCustomer(undefined); setFormOpen(true); }}>
          <Plus className="mr-2 h-4 w-4" /> Add Customer
        </Button>
      </div>

      <div className="flex flex-wrap gap-3">
        <div className="relative flex-1 min-w-[200px]">
          <Search className="absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-muted-foreground" />
          <Input placeholder="Search by name or code…" className="pl-9" value={search}
            onChange={(e) => { setSearch(e.target.value); setPage(1); }} />
        </div>
        <Select value={statusFilter} onValueChange={(v) => { setStatusFilter(v ?? ""); setPage(1); }}>
          <SelectTrigger className="w-36"><SelectValue placeholder="All" /></SelectTrigger>
          <SelectContent>
            {STATUS_OPTIONS.map((o) => (
              <SelectItem key={o.value} value={o.value}>{o.label}</SelectItem>
            ))}
          </SelectContent>
        </Select>
      </div>

      <Card>
        <CardContent className="p-0">
          <Table>
            <TableHeader>
              <TableRow>
                <TableHead>Code</TableHead>
                <TableHead>Name</TableHead>
                <TableHead>Contact</TableHead>
                <TableHead>City</TableHead>
                <TableHead>Status</TableHead>
                <TableHead className="text-right">Actions</TableHead>
              </TableRow>
            </TableHeader>
            <TableBody>
              {loading ? (
                <TableRow><TableCell colSpan={6} className="py-10 text-center text-muted-foreground">Loading…</TableCell></TableRow>
              ) : customers.length === 0 ? (
                <TableRow><TableCell colSpan={6} className="py-10 text-center text-muted-foreground">No customers found.</TableCell></TableRow>
              ) : (
                customers.map((c) => (
                  <TableRow key={c.id}>
                    <TableCell className="font-medium">{c.code}</TableCell>
                    <TableCell>{c.name}</TableCell>
                    <TableCell className="text-muted-foreground">{c.contactPerson ?? "—"}</TableCell>
                    <TableCell className="text-muted-foreground">{c.city ?? "—"}</TableCell>
                    <TableCell>
                      <Badge variant={c.isActive ? "default" : "secondary"}>{c.isActive ? "Active" : "Inactive"}</Badge>
                    </TableCell>
                    <TableCell className="text-right">
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
                    </TableCell>
                  </TableRow>
                ))
              )}
            </TableBody>
          </Table>
        </CardContent>
      </Card>

      {result && result.totalPages > 1 && (
        <div className="flex items-center justify-between text-sm">
          <span className="text-muted-foreground">
            Showing {(result.page - 1) * result.pageSize + 1}–{Math.min(result.page * result.pageSize, result.totalCount)} of {result.totalCount}
          </span>
          <div className="flex gap-2">
            <Button variant="outline" size="sm" disabled={!result.hasPreviousPage} onClick={() => setPage((p) => p - 1)}>Previous</Button>
            <Button variant="outline" size="sm" disabled={!result.hasNextPage} onClick={() => setPage((p) => p + 1)}>Next</Button>
          </div>
        </div>
      )}

      <Dialog open={formOpen} onOpenChange={setFormOpen}>
        <DialogContent className="sm:max-w-lg">
          <DialogHeader><DialogTitle>{editingCustomer ? "Edit Customer" : "Create Customer"}</DialogTitle></DialogHeader>
          <CustomerForm customer={editingCustomer} onSave={handleSave} onCancel={() => { setFormOpen(false); setEditingCustomer(undefined); }} />
        </DialogContent>
      </Dialog>

      <Dialog open={!!deleteTarget} onOpenChange={(open) => { if (!open) setDeleteTarget(null); }}>
        <DialogContent className="sm:max-w-sm">
          <DialogHeader><DialogTitle>Delete Customer</DialogTitle></DialogHeader>
          <p className="text-sm text-muted-foreground">
            Are you sure you want to delete <strong>{deleteTarget?.name}</strong> ({deleteTarget?.code})? This action cannot be undone.
          </p>
          <div className="flex justify-end gap-2 pt-2">
            <Button variant="outline" onClick={() => setDeleteTarget(null)} disabled={deleting}>Cancel</Button>
            <Button variant="destructive" onClick={handleDelete} disabled={deleting}>{deleting ? "Deleting…" : "Delete"}</Button>
          </div>
        </DialogContent>
      </Dialog>
    </div>
  );
}
