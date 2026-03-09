import { useEffect, useState, useCallback } from "react";
import { Plus, Search, Trash2, Edit } from "lucide-react";
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
      {error && <div className="rounded-md bg-destructive/10 p-3 text-sm text-destructive">{error}</div>}

      {!taxCode && (
        <div className="space-y-1">
          <Label htmlFor="code">Code</Label>
          <Input id="code" required value={form.code} onChange={(e) => setForm((f) => ({ ...f, code: e.target.value }))} />
        </div>
      )}

      <div className="space-y-1">
        <Label htmlFor="name">Name</Label>
        <Input id="name" required value={form.name} onChange={(e) => setForm((f) => ({ ...f, name: e.target.value }))} />
      </div>

      <div className="grid grid-cols-2 gap-3">
        <div className="space-y-1">
          <Label htmlFor="rate">Rate</Label>
          <Input id="rate" type="number" step="0.0001" required value={form.rate} onChange={(e) => setForm((f) => ({ ...f, rate: e.target.value }))} />
        </div>
        <div className="space-y-1">
          <Label htmlFor="type">Type</Label>
          <Select value={form.type} onValueChange={(v) => setForm((f) => ({ ...f, type: v ?? "0" }))}>
            <SelectTrigger><SelectValue /></SelectTrigger>
            <SelectContent>
              {TAX_TYPES.map((t) => (
                <SelectItem key={t.value} value={t.value}>{t.label}</SelectItem>
              ))}
            </SelectContent>
          </Select>
        </div>
      </div>

      <div className="space-y-1">
        <Label htmlFor="description">Description</Label>
        <Input id="description" value={form.description} onChange={(e) => setForm((f) => ({ ...f, description: e.target.value }))} />
      </div>

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

      <div className="flex justify-end gap-2 pt-2">
        <Button type="button" variant="outline" onClick={onCancel} disabled={saving}>Cancel</Button>
        <Button type="submit" disabled={saving}>
          {saving ? "Saving…" : taxCode ? "Update Tax Code" : "Create Tax Code"}
        </Button>
      </div>
    </form>
  );
}

export function TaxCodesPage() {
  const [result, setResult] = useState<PagedResult<TaxCode> | null>(null);
  const [loading, setLoading] = useState(true);
  const [search, setSearch] = useState("");
  const [page, setPage] = useState(1);
  const [formOpen, setFormOpen] = useState(false);
  const [editingTaxCode, setEditingTaxCode] = useState<TaxCode | undefined>(undefined);
  const [deleteTarget, setDeleteTarget] = useState<TaxCode | null>(null);
  const [deleting, setDeleting] = useState(false);

  const fetchTaxCodes = useCallback(() => {
    const params: Record<string, string> = { page: String(page), pageSize: "20" };
    if (search) params.searchTerm = search;
    setLoading(true);
    getTaxCodes(params)
      .then(setResult)
      .finally(() => setLoading(false));
  }, [page, search]);

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

  return (
    <div className="space-y-4">
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-2xl font-bold tracking-tight">Tax Codes</h1>
          <p className="text-muted-foreground">Manage tax codes for sales and purchases</p>
        </div>
        <Button onClick={() => { setEditingTaxCode(undefined); setFormOpen(true); }}>
          <Plus className="mr-2 h-4 w-4" /> Add Tax Code
        </Button>
      </div>

      <div className="flex flex-wrap gap-3">
        <div className="relative flex-1 min-w-[200px]">
          <Search className="absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-muted-foreground" />
          <Input placeholder="Search by name or code…" className="pl-9" value={search}
            onChange={(e) => { setSearch(e.target.value); setPage(1); }} />
        </div>
      </div>

      <Card>
        <CardContent className="p-0">
          <Table>
            <TableHeader>
              <TableRow>
                <TableHead>Code</TableHead>
                <TableHead>Name</TableHead>
                <TableHead>Rate</TableHead>
                <TableHead>Type</TableHead>
                <TableHead>Sales</TableHead>
                <TableHead>Purchases</TableHead>
                <TableHead>Status</TableHead>
                <TableHead className="text-right">Actions</TableHead>
              </TableRow>
            </TableHeader>
            <TableBody>
              {loading ? (
                <TableRow><TableCell colSpan={8} className="py-10 text-center text-muted-foreground">Loading…</TableCell></TableRow>
              ) : taxCodes.length === 0 ? (
                <TableRow><TableCell colSpan={8} className="py-10 text-center text-muted-foreground">No tax codes found.</TableCell></TableRow>
              ) : (
                taxCodes.map((tc) => (
                  <TableRow key={tc.id}>
                    <TableCell className="font-medium">{tc.code}</TableCell>
                    <TableCell>{tc.name}</TableCell>
                    <TableCell>{tc.type === 0 ? `${tc.rate}%` : `$${tc.rate.toFixed(2)}`}</TableCell>
                    <TableCell><Badge variant="outline">{tc.typeName}</Badge></TableCell>
                    <TableCell>{tc.appliesToSales ? "Yes" : "No"}</TableCell>
                    <TableCell>{tc.appliesToPurchases ? "Yes" : "No"}</TableCell>
                    <TableCell>
                      <Badge variant={tc.isActive ? "default" : "secondary"}>{tc.isActive ? "Active" : "Inactive"}</Badge>
                    </TableCell>
                    <TableCell className="text-right">
                      <div className="flex items-center justify-end gap-1">
                        <Button variant="ghost" size="sm" onClick={() => { setEditingTaxCode(tc); setFormOpen(true); }} title="Edit">
                          <Edit className="h-4 w-4" />
                        </Button>
                        <Button variant="ghost" size="sm" onClick={() => setDeleteTarget(tc)} title="Delete">
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
        <DialogContent className="sm:max-w-md">
          <DialogHeader><DialogTitle>{editingTaxCode ? "Edit Tax Code" : "Create Tax Code"}</DialogTitle></DialogHeader>
          <TaxCodeForm taxCode={editingTaxCode} onSave={handleSave} onCancel={() => { setFormOpen(false); setEditingTaxCode(undefined); }} />
        </DialogContent>
      </Dialog>

      <Dialog open={!!deleteTarget} onOpenChange={(open) => { if (!open) setDeleteTarget(null); }}>
        <DialogContent className="sm:max-w-sm">
          <DialogHeader><DialogTitle>Delete Tax Code</DialogTitle></DialogHeader>
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
