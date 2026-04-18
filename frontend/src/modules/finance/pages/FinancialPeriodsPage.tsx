import { useCallback, useEffect, useState } from "react";
import type { ColumnDef } from "@tanstack/react-table";
import { CheckCircle, Edit, Lock, Plus, Trash2, Unlock } from "lucide-react";
import { Button } from "@app/components/ui/button";
import { Input } from "@app/components/ui/input";
import { Dialog, DialogContent, DialogHeader, DialogTitle } from "@app/components/ui/dialog";
import {
  DataTable,
  DeleteDialog,
  FormActions,
  FormError,
  FormField,
  FormGrid,
  PageHeader,
  PageLayout,
  StatusBadge,
} from "@shared/components";
import {
  closeFinancialPeriod,
  createFinancialPeriod,
  deleteFinancialPeriod,
  getFinancialPeriods,
  reopenFinancialPeriod,
  updateFinancialPeriod,
} from "../api/financeApi";
import type { CreateFinancialPeriodRequest, FinancialPeriod, PagedResult, UpdateFinancialPeriodRequest } from "../types";

function FinancialPeriodForm({
  period,
  onSave,
  onCancel,
}: {
  period?: FinancialPeriod;
  onSave: (data: CreateFinancialPeriodRequest | UpdateFinancialPeriodRequest) => Promise<void>;
  onCancel: () => void;
}) {
  const [form, setForm] = useState({
    name: period?.name ?? "",
    startDate: period?.startDate.slice(0, 10) ?? "",
    endDate: period?.endDate.slice(0, 10) ?? "",
  });
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState("");

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setSaving(true);
    setError("");
    try {
      await onSave(form);
    } catch (err) {
      setError(err instanceof Error ? err.message : "An error occurred.");
      setSaving(false);
    }
  };

  return (
    <form onSubmit={handleSubmit} className="space-y-4">
      <FormError error={error} />
      <FormField id="name" label="Name">
        <Input id="name" required value={form.name} onChange={(e) => setForm((current) => ({ ...current, name: e.target.value }))} />
      </FormField>
      <FormGrid>
        <FormField id="startDate" label="Start Date">
          <Input id="startDate" type="date" required value={form.startDate} onChange={(e) => setForm((current) => ({ ...current, startDate: e.target.value }))} />
        </FormField>
        <FormField id="endDate" label="End Date">
          <Input id="endDate" type="date" required value={form.endDate} onChange={(e) => setForm((current) => ({ ...current, endDate: e.target.value }))} />
        </FormField>
      </FormGrid>
      <FormActions onCancel={onCancel} saving={saving} saveLabel={period ? "Update Period" : "Create Period"} />
    </form>
  );
}

export function FinancialPeriodsPage() {
  const [result, setResult] = useState<PagedResult<FinancialPeriod> | null>(null);
  const [loading, setLoading] = useState(true);
  const [formOpen, setFormOpen] = useState(false);
  const [editingPeriod, setEditingPeriod] = useState<FinancialPeriod | undefined>();
  const [deleteTarget, setDeleteTarget] = useState<FinancialPeriod | null>(null);
  const [deleting, setDeleting] = useState(false);

  const fetchPeriods = useCallback(() => {
    setLoading(true);
    getFinancialPeriods({ page: "1", pageSize: "100" }).then(setResult).finally(() => setLoading(false));
  }, []);

  useEffect(() => { fetchPeriods(); }, [fetchPeriods]);

  const handleSave = async (data: CreateFinancialPeriodRequest | UpdateFinancialPeriodRequest) => {
    if (editingPeriod) {
      await updateFinancialPeriod(editingPeriod.id, data as UpdateFinancialPeriodRequest);
    } else {
      await createFinancialPeriod(data as CreateFinancialPeriodRequest);
    }
    setFormOpen(false);
    setEditingPeriod(undefined);
    fetchPeriods();
  };

  const handleDelete = async () => {
    if (!deleteTarget) return;
    setDeleting(true);
    try {
      await deleteFinancialPeriod(deleteTarget.id);
      setDeleteTarget(null);
      fetchPeriods();
    } finally {
      setDeleting(false);
    }
  };

  const columns: ColumnDef<FinancialPeriod>[] = [
    { accessorKey: "name", header: "Name" },
    { accessorKey: "startDate", header: "Start", cell: ({ row }) => row.original.startDate.slice(0, 10) },
    { accessorKey: "endDate", header: "End", cell: ({ row }) => row.original.endDate.slice(0, 10) },
    {
      accessorKey: "statusName",
      header: "Status",
      cell: ({ row }) => <StatusBadge isActive={row.original.status === 0} activeLabel={row.original.statusName} inactiveLabel={row.original.statusName} />,
    },
    { accessorKey: "closedBy", header: "Closed By", cell: ({ row }) => row.original.closedBy ?? "—" },
    {
      id: "actions",
      header: "Actions",
      meta: { className: "text-right", headerClassName: "text-right" },
      cell: ({ row }) => {
        const period = row.original;
        const isOpen = period.status === 0;
        return (
          <div className="flex items-center justify-end gap-1">
            {isOpen && (
              <>
                <Button variant="ghost" size="sm" onClick={() => { setEditingPeriod(period); setFormOpen(true); }}><Edit className="h-4 w-4" /></Button>
                <Button variant="ghost" size="sm" onClick={() => closeFinancialPeriod(period.id).then(fetchPeriods)}><Lock className="h-4 w-4 text-amber-600" /></Button>
                <Button variant="ghost" size="sm" onClick={() => setDeleteTarget(period)}><Trash2 className="h-4 w-4 text-destructive" /></Button>
              </>
            )}
            {!isOpen && (
              <Button variant="ghost" size="sm" onClick={() => reopenFinancialPeriod(period.id).then(fetchPeriods)}><Unlock className="h-4 w-4 text-emerald-600" /></Button>
            )}
          </div>
        );
      },
    },
  ];

  return (
    <PageLayout>
      <PageHeader
        title="Financial Periods"
        description="Open, close, and maintain accounting periods."
        action={
          <Button onClick={() => { setEditingPeriod(undefined); setFormOpen(true); }}>
            <Plus className="mr-2 h-4 w-4" /> New Period
          </Button>
        }
      />
      <DataTable columns={columns} data={result?.items ?? []} loading={loading} />

      <Dialog open={formOpen} onOpenChange={setFormOpen}>
        <DialogContent className="sm:max-w-xl">
          <DialogHeader>
            <DialogTitle>{editingPeriod ? "Edit Financial Period" : "New Financial Period"}</DialogTitle>
          </DialogHeader>
          <FinancialPeriodForm period={editingPeriod} onSave={handleSave} onCancel={() => setFormOpen(false)} />
        </DialogContent>
      </Dialog>

      <DeleteDialog
        open={Boolean(deleteTarget)}
        onOpenChange={(open) => { if (!open) setDeleteTarget(null); }}
        entityLabel={deleteTarget?.name ?? "period"}
        onConfirm={handleDelete}
        deleting={deleting}
      />
    </PageLayout>
  );
}
