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
      {error && <div className="rounded-md bg-destructive/10 p-3 text-sm text-destructive">{error}</div>}

      {!vendor && (
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
          <Label htmlFor="paymentTerms">Payment Terms</Label>
          <Input id="paymentTerms" value={form.paymentTerms} onChange={(e) => set("paymentTerms", e.target.value)} />
        </div>
        <div className="space-y-1">
          <Label htmlFor="leadTimeDays">Lead Time (Days)</Label>
          <Input id="leadTimeDays" type="number" min="0" value={form.leadTimeDays} onChange={(e) => set("leadTimeDays", e.target.value)} />
        </div>
      </div>

      <fieldset className="border rounded-md p-3 space-y-3">
        <legend className="text-sm font-medium px-1">Banking Details</legend>
        <div className="grid grid-cols-2 gap-3">
          <div className="space-y-1">
            <Label htmlFor="bankName">Bank Name</Label>
            <Input id="bankName" value={form.bankName} onChange={(e) => set("bankName", e.target.value)} />
          </div>
          <div className="space-y-1">
            <Label htmlFor="bankAccountNumber">Account Number</Label>
            <Input id="bankAccountNumber" value={form.bankAccountNumber} onChange={(e) => set("bankAccountNumber", e.target.value)} />
          </div>
        </div>
        <div className="grid grid-cols-2 gap-3">
          <div className="space-y-1">
            <Label htmlFor="bankRoutingNumber">Routing Number</Label>
            <Input id="bankRoutingNumber" value={form.bankRoutingNumber} onChange={(e) => set("bankRoutingNumber", e.target.value)} />
          </div>
          <div className="space-y-1">
            <Label htmlFor="bankSwiftCode">SWIFT Code</Label>
            <Input id="bankSwiftCode" value={form.bankSwiftCode} onChange={(e) => set("bankSwiftCode", e.target.value)} />
          </div>
        </div>
      </fieldset>

      <div className="space-y-1">
        <Label htmlFor="notes">Notes</Label>
        <Input id="notes" value={form.notes} onChange={(e) => set("notes", e.target.value)} />
      </div>

      <div className="flex justify-end gap-2 pt-2">
        <Button type="button" variant="outline" onClick={onCancel} disabled={saving}>Cancel</Button>
        <Button type="submit" disabled={saving}>
          {saving ? "Saving…" : vendor ? "Update Vendor" : "Create Vendor"}
        </Button>
      </div>
    </form>
  );
}

export function VendorsPage() {
  const [result, setResult] = useState<PagedResult<Vendor> | null>(null);
  const [loading, setLoading] = useState(true);
  const [search, setSearch] = useState("");
  const [statusFilter, setStatusFilter] = useState("");
  const [page, setPage] = useState(1);
  const [formOpen, setFormOpen] = useState(false);
  const [editingVendor, setEditingVendor] = useState<Vendor | undefined>(undefined);
  const [deleteTarget, setDeleteTarget] = useState<Vendor | null>(null);
  const [deleting, setDeleting] = useState(false);

  const fetchVendors = useCallback(() => {
    const params: Record<string, string> = { page: String(page), pageSize: "20" };
    if (search) params.searchTerm = search;
    if (statusFilter) params.isActive = statusFilter;
    setLoading(true);
    getVendors(params)
      .then(setResult)
      .finally(() => setLoading(false));
  }, [page, search, statusFilter]);

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

  return (
    <div className="space-y-4">
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-2xl font-bold tracking-tight">Vendors</h1>
          <p className="text-muted-foreground">Manage vendor records</p>
        </div>
        <Button onClick={() => { setEditingVendor(undefined); setFormOpen(true); }}>
          <Plus className="mr-2 h-4 w-4" /> Add Vendor
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
                <TableHead>Lead Time</TableHead>
                <TableHead>Status</TableHead>
                <TableHead className="text-right">Actions</TableHead>
              </TableRow>
            </TableHeader>
            <TableBody>
              {loading ? (
                <TableRow><TableCell colSpan={7} className="py-10 text-center text-muted-foreground">Loading…</TableCell></TableRow>
              ) : vendors.length === 0 ? (
                <TableRow><TableCell colSpan={7} className="py-10 text-center text-muted-foreground">No vendors found.</TableCell></TableRow>
              ) : (
                vendors.map((v) => (
                  <TableRow key={v.id}>
                    <TableCell className="font-medium">{v.code}</TableCell>
                    <TableCell>{v.name}</TableCell>
                    <TableCell className="text-muted-foreground">{v.contactPerson ?? "—"}</TableCell>
                    <TableCell className="text-muted-foreground">{v.city ?? "—"}</TableCell>
                    <TableCell className="text-muted-foreground">{v.leadTimeDays}d</TableCell>
                    <TableCell>
                      <Badge variant={v.isActive ? "default" : "secondary"}>{v.isActive ? "Active" : "Inactive"}</Badge>
                    </TableCell>
                    <TableCell className="text-right">
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
          <DialogHeader><DialogTitle>{editingVendor ? "Edit Vendor" : "Create Vendor"}</DialogTitle></DialogHeader>
          <VendorForm vendor={editingVendor} onSave={handleSave} onCancel={() => { setFormOpen(false); setEditingVendor(undefined); }} />
        </DialogContent>
      </Dialog>

      <Dialog open={!!deleteTarget} onOpenChange={(open) => { if (!open) setDeleteTarget(null); }}>
        <DialogContent className="sm:max-w-sm">
          <DialogHeader><DialogTitle>Delete Vendor</DialogTitle></DialogHeader>
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
