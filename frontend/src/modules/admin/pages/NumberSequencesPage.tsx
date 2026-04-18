import { useCallback, useEffect, useState } from "react";
import type { ColumnDef, ColumnFiltersState, OnChangeFn, SortingState } from "@tanstack/react-table";
import { Ban, CheckCircle, Edit, Plus, Trash2 } from "lucide-react";
import { Button } from "@app/components/ui/button";
import { Input } from "@app/components/ui/input";
import { Dialog, DialogContent, DialogHeader, DialogTitle } from "@app/components/ui/dialog";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@app/components/ui/select";
import {
  ColumnFilterInput,
  DeleteDialog,
  FormActions,
  FormError,
  FormField,
  FormGrid,
  PageHeader,
  PageLayout,
  DataTable,
  StatusBadge,
} from "@shared/components";
import {
  activateNumberSequence,
  createNumberSequence,
  deactivateNumberSequence,
  deleteNumberSequence,
  getNumberSequences,
  updateNumberSequence,
} from "../api/adminApi";
import type {
  CreateNumberSequenceRequest,
  NumberSequence,
  PagedResult,
  UpdateNumberSequenceRequest,
} from "../types";

const RESET_POLICIES = [
  { value: "0", label: "Manual" },
  { value: "1", label: "Annual" },
];

function NumberSequenceForm({
  sequence,
  onSave,
  onCancel,
}: {
  sequence?: NumberSequence;
  onSave: (data: CreateNumberSequenceRequest | UpdateNumberSequenceRequest) => Promise<void>;
  onCancel: () => void;
}) {
  const [form, setForm] = useState({
    module: sequence?.module ?? "",
    documentType: sequence?.documentType ?? "",
    prefix: sequence?.prefix ?? "",
    suffix: sequence?.suffix ?? "",
    startingNumber: String(sequence?.startingNumber ?? 1),
    nextNumber: String(sequence?.nextNumber ?? 1),
    paddingLength: String(sequence?.paddingLength ?? 5),
    incrementBy: String(sequence?.incrementBy ?? 1),
    resetPolicy: String(sequence?.resetPolicy ?? 0),
    isActive: sequence?.isActive ?? true,
  });
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState("");

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setSaving(true);
    setError("");
    try {
      const payload = {
        module: form.module,
        documentType: form.documentType,
        prefix: form.prefix,
        suffix: form.suffix || undefined,
        startingNumber: Number(form.startingNumber),
        nextNumber: Number(form.nextNumber),
        paddingLength: Number(form.paddingLength),
        incrementBy: Number(form.incrementBy),
        resetPolicy: Number(form.resetPolicy),
        isActive: form.isActive,
      };
      await onSave(payload);
    } catch (err) {
      setError(err instanceof Error ? err.message : "An error occurred.");
      setSaving(false);
    }
  };

  return (
    <form onSubmit={handleSubmit} className="space-y-4">
      <FormError error={error} />
      <FormGrid>
        <FormField id="module" label="Module">
          <Input id="module" required value={form.module} onChange={(e) => setForm((x) => ({ ...x, module: e.target.value }))} />
        </FormField>
        <FormField id="documentType" label="Document Type">
          <Input id="documentType" required value={form.documentType} onChange={(e) => setForm((x) => ({ ...x, documentType: e.target.value }))} />
        </FormField>
      </FormGrid>
      <FormGrid>
        <FormField id="prefix" label="Prefix">
          <Input id="prefix" required value={form.prefix} onChange={(e) => setForm((x) => ({ ...x, prefix: e.target.value }))} />
        </FormField>
        <FormField id="suffix" label="Suffix">
          <Input id="suffix" value={form.suffix} onChange={(e) => setForm((x) => ({ ...x, suffix: e.target.value }))} />
        </FormField>
      </FormGrid>
      <FormGrid>
        <FormField id="startingNumber" label="Starting Number">
          <Input id="startingNumber" type="number" min={1} required value={form.startingNumber} onChange={(e) => setForm((x) => ({ ...x, startingNumber: e.target.value }))} />
        </FormField>
        <FormField id="nextNumber" label="Next Number">
          <Input id="nextNumber" type="number" min={1} required value={form.nextNumber} onChange={(e) => setForm((x) => ({ ...x, nextNumber: e.target.value }))} />
        </FormField>
      </FormGrid>
      <FormGrid>
        <FormField id="paddingLength" label="Padding Length">
          <Input id="paddingLength" type="number" min={1} required value={form.paddingLength} onChange={(e) => setForm((x) => ({ ...x, paddingLength: e.target.value }))} />
        </FormField>
        <FormField id="incrementBy" label="Increment By">
          <Input id="incrementBy" type="number" min={1} required value={form.incrementBy} onChange={(e) => setForm((x) => ({ ...x, incrementBy: e.target.value }))} />
        </FormField>
      </FormGrid>
      <FormGrid>
        <FormField id="resetPolicy" label="Reset Policy">
          <Select value={form.resetPolicy} onValueChange={(value) => setForm((x) => ({ ...x, resetPolicy: value ?? "0" }))}>
            <SelectTrigger><SelectValue /></SelectTrigger>
            <SelectContent>
              {RESET_POLICIES.map((option) => (
                <SelectItem key={option.value} value={option.value}>{option.label}</SelectItem>
              ))}
            </SelectContent>
          </Select>
        </FormField>
        <label className="flex items-center gap-2 text-sm pt-8">
          <input type="checkbox" checked={form.isActive} onChange={(e) => setForm((x) => ({ ...x, isActive: e.target.checked }))} />
          Active
        </label>
      </FormGrid>
      <FormActions onCancel={onCancel} saving={saving} saveLabel={sequence ? "Update Sequence" : "Create Sequence"} />
    </form>
  );
}

export function NumberSequencesPage() {
  const [result, setResult] = useState<PagedResult<NumberSequence> | null>(null);
  const [loading, setLoading] = useState(true);
  const [columnFilters, setColumnFilters] = useState<ColumnFiltersState>([]);
  const [sorting, setSorting] = useState<SortingState>([]);
  const [page, setPage] = useState(1);
  const [formOpen, setFormOpen] = useState(false);
  const [editingSequence, setEditingSequence] = useState<NumberSequence | undefined>();
  const [deleteTarget, setDeleteTarget] = useState<NumberSequence | null>(null);
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

  const fetchSequences = useCallback(() => {
    const search = String(columnFilters.find((filter) => filter.id === "searchTerm")?.value ?? "");
    const params: Record<string, string> = { page: String(page), pageSize: "20" };
    if (search) params.searchTerm = search;
    if (sorting[0]) {
      params.sortBy = sorting[0].id;
      params.sortDescending = String(sorting[0].desc);
    }
    setLoading(true);
    getNumberSequences(params).then(setResult).finally(() => setLoading(false));
  }, [columnFilters, page, sorting]);

  useEffect(() => { fetchSequences(); }, [fetchSequences]);

  const handleSave = async (data: CreateNumberSequenceRequest | UpdateNumberSequenceRequest) => {
    if (editingSequence) {
      await updateNumberSequence(editingSequence.id, data as UpdateNumberSequenceRequest);
    } else {
      await createNumberSequence(data as CreateNumberSequenceRequest);
    }
    setFormOpen(false);
    setEditingSequence(undefined);
    fetchSequences();
  };

  const handleDelete = async () => {
    if (!deleteTarget) return;
    setDeleting(true);
    try {
      await deleteNumberSequence(deleteTarget.id);
      setDeleteTarget(null);
      fetchSequences();
    } finally {
      setDeleting(false);
    }
  };

  const handleActivate = async (sequence: NumberSequence) => {
    await activateNumberSequence(sequence.id);
    fetchSequences();
  };

  const handleDeactivate = async (sequence: NumberSequence) => {
    await deactivateNumberSequence(sequence.id);
    fetchSequences();
  };

  const columns: ColumnDef<NumberSequence>[] = [
    {
      accessorKey: "module",
      header: "Module",
      meta: {
        filterId: "searchTerm",
        filterComponent: ({ value, onChange }) => (
          <ColumnFilterInput value={value} onChange={onChange} placeholder="Search sequences…" />
        ),
      },
    },
    { accessorKey: "documentType", header: "Document Type" },
    { accessorKey: "prefix", header: "Prefix" },
    { accessorKey: "preview", header: "Preview", cell: ({ row }) => <span className="font-medium">{row.original.preview}</span> },
    { accessorKey: "resetPolicyName", header: "Reset Policy", enableSorting: false },
    {
      accessorKey: "isActive",
      header: "Status",
      enableSorting: false,
      cell: ({ row }) => <StatusBadge isActive={row.original.isActive} />,
    },
    {
      id: "actions",
      header: "Actions",
      enableSorting: false,
      meta: { className: "text-right", headerClassName: "text-right" },
      cell: ({ row }) => {
        const sequence = row.original;
        return (
          <div className="flex items-center justify-end gap-1">
            <Button variant="ghost" size="sm" onClick={() => { setEditingSequence(sequence); setFormOpen(true); }} title="Edit">
              <Edit className="h-4 w-4" />
            </Button>
            {sequence.isActive ? (
              <Button variant="ghost" size="sm" onClick={() => handleDeactivate(sequence)} title="Deactivate">
                <Ban className="h-4 w-4 text-amber-600" />
              </Button>
            ) : (
              <Button variant="ghost" size="sm" onClick={() => handleActivate(sequence)} title="Activate">
                <CheckCircle className="h-4 w-4 text-emerald-600" />
              </Button>
            )}
            <Button variant="ghost" size="sm" onClick={() => setDeleteTarget(sequence)} title="Delete">
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
        title="Number Sequences"
        description="Configure document numbering patterns across modules."
        action={
          <Button onClick={() => { setEditingSequence(undefined); setFormOpen(true); }}>
            <Plus className="mr-2 h-4 w-4" /> Add Sequence
          </Button>
        }
      />
      <DataTable
        columns={columns}
        data={result?.items ?? []}
        loading={loading}
        filtering={{ state: columnFilters, onChange: handleFilteringChange }}
        sorting={{ state: sorting, onChange: handleSortingChange }}
        pagination={result ? {
          result,
          onPrevious: () => setPage((current) => Math.max(1, current - 1)),
          onNext: () => setPage((current) => current + 1),
        } : undefined}
      />

      <Dialog open={formOpen} onOpenChange={setFormOpen}>
        <DialogContent className="sm:max-w-2xl">
          <DialogHeader>
            <DialogTitle>{editingSequence ? "Edit Number Sequence" : "New Number Sequence"}</DialogTitle>
          </DialogHeader>
          <NumberSequenceForm sequence={editingSequence} onSave={handleSave} onCancel={() => setFormOpen(false)} />
        </DialogContent>
      </Dialog>

      <DeleteDialog
        open={Boolean(deleteTarget)}
        onOpenChange={(open) => { if (!open) setDeleteTarget(null); }}
        entityLabel={deleteTarget ? `${deleteTarget.module} / ${deleteTarget.documentType}` : "sequence"}
        onConfirm={handleDelete}
        deleting={deleting}
      />
    </PageLayout>
  );
}
