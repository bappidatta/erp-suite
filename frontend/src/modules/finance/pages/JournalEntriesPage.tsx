import { useCallback, useEffect, useMemo, useState } from "react";
import type { ColumnDef, ColumnFiltersState, OnChangeFn, SortingState } from "@tanstack/react-table";
import { CheckCircle, Edit, Plus, Trash2 } from "lucide-react";
import { Button } from "@app/components/ui/button";
import { Input } from "@app/components/ui/input";
import { Dialog, DialogContent, DialogHeader, DialogTitle } from "@app/components/ui/dialog";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@app/components/ui/select";
import {
  ColumnFilterInput,
  ColumnFilterSelect,
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
  createJournalEntry,
  deleteJournalEntry,
  getAccounts,
  getJournalEntries,
  postJournalEntry,
  updateJournalEntry,
} from "../api/financeApi";
import type {
  Account,
  CreateJournalEntryRequest,
  JournalEntry,
  JournalEntryLineRequest,
  PagedResult,
  UpdateJournalEntryRequest,
} from "../types";

const JOURNAL_STATUS_OPTIONS = [
  { value: "", label: "All Statuses" },
  { value: "0", label: "Draft" },
  { value: "1", label: "Posted" },
];

function JournalEntryForm({
  journalEntry,
  accounts,
  onSave,
  onCancel,
}: {
  journalEntry?: JournalEntry;
  accounts: Account[];
  onSave: (data: CreateJournalEntryRequest | UpdateJournalEntryRequest) => Promise<void>;
  onCancel: () => void;
}) {
  const [entryDate, setEntryDate] = useState(journalEntry?.entryDate.slice(0, 10) ?? new Date().toISOString().slice(0, 10));
  const [description, setDescription] = useState(journalEntry?.description ?? "");
  const [reference, setReference] = useState(journalEntry?.reference ?? "");
  const [lines, setLines] = useState<JournalEntryLineRequest[]>(
    journalEntry?.lines.map((line) => ({
      lineNumber: line.lineNumber,
      accountId: line.accountId,
      debitAmount: line.debitAmount,
      creditAmount: line.creditAmount,
      description: line.description,
    })) ?? [
      { lineNumber: 1, accountId: 0, debitAmount: 0, creditAmount: 0, description: "" },
      { lineNumber: 2, accountId: 0, debitAmount: 0, creditAmount: 0, description: "" },
    ],
  );
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState("");

  const totalDebit = useMemo(() => lines.reduce((sum, line) => sum + line.debitAmount, 0), [lines]);
  const totalCredit = useMemo(() => lines.reduce((sum, line) => sum + line.creditAmount, 0), [lines]);

  const updateLine = (index: number, patch: Partial<JournalEntryLineRequest>) => {
    setLines((current) => current.map((line, lineIndex) => (
      lineIndex === index ? { ...line, ...patch } : line
    )));
  };

  const addLine = () => {
    setLines((current) => [...current, { lineNumber: current.length + 1, accountId: 0, debitAmount: 0, creditAmount: 0, description: "" }]);
  };

  const removeLine = (index: number) => {
    setLines((current) => current.filter((_, lineIndex) => lineIndex !== index).map((line, idx) => ({ ...line, lineNumber: idx + 1 })));
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setSaving(true);
    setError("");
    try {
      const payload = {
        entryDate,
        description,
        reference: reference || undefined,
        lines: lines.map((line, index) => ({ ...line, lineNumber: index + 1 })),
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
        <FormField id="entryDate" label="Entry Date">
          <Input id="entryDate" type="date" required value={entryDate} onChange={(e) => setEntryDate(e.target.value)} />
        </FormField>
        <FormField id="reference" label="Reference">
          <Input id="reference" value={reference} onChange={(e) => setReference(e.target.value)} />
        </FormField>
      </FormGrid>
      <FormField id="description" label="Description">
        <Input id="description" required value={description} onChange={(e) => setDescription(e.target.value)} />
      </FormField>

      <div className="space-y-3">
        <div className="flex items-center justify-between">
          <h3 className="text-sm font-medium">Lines</h3>
          <Button type="button" variant="outline" size="sm" onClick={addLine}>
            <Plus className="mr-2 h-4 w-4" /> Add Line
          </Button>
        </div>
        {lines.map((line, index) => (
          <div key={line.lineNumber} className="grid gap-3 rounded-md border p-3 md:grid-cols-[2fr_1fr_1fr_2fr_auto]">
            <Select value={line.accountId ? String(line.accountId) : ""} onValueChange={(value) => updateLine(index, { accountId: Number(value) })}>
              <SelectTrigger><SelectValue placeholder="Select account" /></SelectTrigger>
              <SelectContent>
                {accounts.map((account) => (
                  <SelectItem key={account.id} value={String(account.id)}>{account.code} — {account.name}</SelectItem>
                ))}
              </SelectContent>
            </Select>
            <Input type="number" step="0.01" min="0" placeholder="Debit" value={line.debitAmount || ""} onChange={(e) => updateLine(index, { debitAmount: Number(e.target.value || "0") })} />
            <Input type="number" step="0.01" min="0" placeholder="Credit" value={line.creditAmount || ""} onChange={(e) => updateLine(index, { creditAmount: Number(e.target.value || "0") })} />
            <Input placeholder="Line description" value={line.description ?? ""} onChange={(e) => updateLine(index, { description: e.target.value })} />
            <Button type="button" variant="ghost" size="sm" disabled={lines.length <= 2} onClick={() => removeLine(index)}>
              <Trash2 className="h-4 w-4 text-destructive" />
            </Button>
          </div>
        ))}
      </div>

      <div className="flex justify-end gap-6 text-sm font-medium">
        <span>Total Debit: {totalDebit.toFixed(2)}</span>
        <span>Total Credit: {totalCredit.toFixed(2)}</span>
      </div>

      <FormActions onCancel={onCancel} saving={saving} saveLabel={journalEntry ? "Update Journal Entry" : "Create Journal Entry"} />
    </form>
  );
}

export function JournalEntriesPage() {
  const [result, setResult] = useState<PagedResult<JournalEntry> | null>(null);
  const [accounts, setAccounts] = useState<Account[]>([]);
  const [loading, setLoading] = useState(true);
  const [columnFilters, setColumnFilters] = useState<ColumnFiltersState>([]);
  const [sorting, setSorting] = useState<SortingState>([]);
  const [page, setPage] = useState(1);
  const [formOpen, setFormOpen] = useState(false);
  const [editingJournalEntry, setEditingJournalEntry] = useState<JournalEntry | undefined>();
  const [deleteTarget, setDeleteTarget] = useState<JournalEntry | null>(null);
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

  const fetchAccounts = useCallback(() => {
    getAccounts({ page: "1", pageSize: "500", isHeader: "false" }).then((response) => setAccounts(response.items)).catch(() => {});
  }, []);

  const fetchJournalEntries = useCallback(() => {
    const search = String(columnFilters.find((filter) => filter.id === "searchTerm")?.value ?? "");
    const status = String(columnFilters.find((filter) => filter.id === "status")?.value ?? "");
    const params: Record<string, string> = { page: String(page), pageSize: "20" };
    if (search) params.searchTerm = search;
    if (status) params.status = status;
    if (sorting[0]) {
      params.sortBy = sorting[0].id;
      params.sortDescending = String(sorting[0].desc);
    }
    setLoading(true);
    getJournalEntries(params).then(setResult).finally(() => setLoading(false));
  }, [columnFilters, page, sorting]);

  useEffect(() => { fetchAccounts(); }, [fetchAccounts]);
  useEffect(() => { fetchJournalEntries(); }, [fetchJournalEntries]);

  const handleSave = async (data: CreateJournalEntryRequest | UpdateJournalEntryRequest) => {
    if (editingJournalEntry) {
      await updateJournalEntry(editingJournalEntry.id, data as UpdateJournalEntryRequest);
    } else {
      await createJournalEntry(data as CreateJournalEntryRequest);
    }
    setFormOpen(false);
    setEditingJournalEntry(undefined);
    fetchJournalEntries();
  };

  const handleDelete = async () => {
    if (!deleteTarget) return;
    setDeleting(true);
    try {
      await deleteJournalEntry(deleteTarget.id);
      setDeleteTarget(null);
      fetchJournalEntries();
    } finally {
      setDeleting(false);
    }
  };

  const handlePost = async (journalEntry: JournalEntry) => {
    await postJournalEntry(journalEntry.id);
    fetchJournalEntries();
  };

  const columns: ColumnDef<JournalEntry>[] = [
    {
      accessorKey: "number",
      header: "Number",
      meta: {
        filterId: "searchTerm",
        filterComponent: ({ value, onChange }) => <ColumnFilterInput value={value} onChange={onChange} placeholder="Search journals…" />,
      },
    },
    { accessorKey: "entryDate", header: "Date", cell: ({ row }) => row.original.entryDate.slice(0, 10) },
    { accessorKey: "description", header: "Description" },
    { accessorKey: "totalDebit", header: "Debit", cell: ({ row }) => row.original.totalDebit.toFixed(2) },
    { accessorKey: "totalCredit", header: "Credit", cell: ({ row }) => row.original.totalCredit.toFixed(2) },
    {
      accessorKey: "statusName",
      header: "Status",
      enableSorting: false,
      meta: {
        filterId: "status",
        filterComponent: ({ value, onChange }) => <ColumnFilterSelect value={value} onChange={onChange} options={JOURNAL_STATUS_OPTIONS} placeholder="All Statuses" />,
      },
      cell: ({ row }) => <StatusBadge isActive={row.original.status === 1} activeLabel={row.original.statusName} inactiveLabel={row.original.statusName} />,
    },
    {
      id: "actions",
      header: "Actions",
      enableSorting: false,
      meta: { className: "text-right", headerClassName: "text-right" },
      cell: ({ row }) => {
        const journalEntry = row.original;
        const isDraft = journalEntry.status === 0;
        return (
          <div className="flex items-center justify-end gap-1">
            {isDraft && (
              <>
                <Button variant="ghost" size="sm" onClick={() => { setEditingJournalEntry(journalEntry); setFormOpen(true); }}>
                  <Edit className="h-4 w-4" />
                </Button>
                <Button variant="ghost" size="sm" onClick={() => handlePost(journalEntry)}>
                  <CheckCircle className="h-4 w-4 text-emerald-600" />
                </Button>
                <Button variant="ghost" size="sm" onClick={() => setDeleteTarget(journalEntry)}>
                  <Trash2 className="h-4 w-4 text-destructive" />
                </Button>
              </>
            )}
          </div>
        );
      },
    },
  ];

  return (
    <PageLayout>
      <PageHeader
        title="Journal Entries"
        description="Capture and post balanced general ledger journals."
        action={
          <Button onClick={() => { setEditingJournalEntry(undefined); setFormOpen(true); }}>
            <Plus className="mr-2 h-4 w-4" /> New Journal
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
        <DialogContent className="max-w-5xl">
          <DialogHeader>
            <DialogTitle>{editingJournalEntry ? "Edit Journal Entry" : "New Journal Entry"}</DialogTitle>
          </DialogHeader>
          <JournalEntryForm journalEntry={editingJournalEntry} accounts={accounts} onSave={handleSave} onCancel={() => setFormOpen(false)} />
        </DialogContent>
      </Dialog>

      <DeleteDialog
        open={Boolean(deleteTarget)}
        onOpenChange={(open) => { if (!open) setDeleteTarget(null); }}
        entityLabel={deleteTarget?.number ?? "journal entry"}
        onConfirm={handleDelete}
        deleting={deleting}
      />
    </PageLayout>
  );
}
