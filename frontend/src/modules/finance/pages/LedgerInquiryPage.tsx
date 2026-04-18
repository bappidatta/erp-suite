import { useCallback, useEffect, useState } from "react";
import type { ColumnDef } from "@tanstack/react-table";
import { Search } from "lucide-react";
import { Button } from "@app/components/ui/button";
import { Input } from "@app/components/ui/input";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@app/components/ui/select";
import { DataTable, FormField, FormGrid, PageHeader, PageLayout } from "@shared/components";
import { getAccounts, getLedgerEntries } from "../api/financeApi";
import type { Account, LedgerEntry } from "../types";

export function LedgerInquiryPage() {
  const [accounts, setAccounts] = useState<Account[]>([]);
  const [accountId, setAccountId] = useState("");
  const [fromDate, setFromDate] = useState("");
  const [toDate, setToDate] = useState("");
  const [rows, setRows] = useState<LedgerEntry[]>([]);
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    getAccounts({ page: "1", pageSize: "500", isHeader: "false" }).then((response) => setAccounts(response.items)).catch(() => {});
  }, []);

  const fetchRows = useCallback(() => {
    if (!accountId) {
      setRows([]);
      return;
    }
    const params: Record<string, string> = { accountId };
    if (fromDate) params.fromDate = fromDate;
    if (toDate) params.toDate = toDate;
    setLoading(true);
    getLedgerEntries(params).then(setRows).finally(() => setLoading(false));
  }, [accountId, fromDate, toDate]);

  const columns: ColumnDef<LedgerEntry>[] = [
    { accessorKey: "journalNumber", header: "Journal #" },
    { accessorKey: "entryDate", header: "Date", cell: ({ row }) => row.original.entryDate.slice(0, 10) },
    { accessorKey: "description", header: "Description" },
    { accessorKey: "reference", header: "Reference", cell: ({ row }) => row.original.reference ?? "—" },
    { accessorKey: "debitAmount", header: "Debit", cell: ({ row }) => row.original.debitAmount.toFixed(2) },
    { accessorKey: "creditAmount", header: "Credit", cell: ({ row }) => row.original.creditAmount.toFixed(2) },
    { accessorKey: "runningBalance", header: "Running Balance", cell: ({ row }) => row.original.runningBalance.toFixed(2) },
  ];

  return (
    <PageLayout>
      <PageHeader title="Ledger Inquiry" description="Drill into posted activity for a single account." />
      <FormGrid>
        <FormField id="ledgerAccount" label="Account">
          <Select value={accountId} onValueChange={(value) => setAccountId(value ?? "")}>
            <SelectTrigger><SelectValue placeholder="Select account" /></SelectTrigger>
            <SelectContent>
              {accounts.map((account) => (
                <SelectItem key={account.id} value={String(account.id)}>{account.code} — {account.name}</SelectItem>
              ))}
            </SelectContent>
          </Select>
        </FormField>
        <FormField id="ledgerFromDate" label="From Date">
          <Input id="ledgerFromDate" type="date" value={fromDate} onChange={(e) => setFromDate(e.target.value)} />
        </FormField>
        <FormField id="ledgerToDate" label="To Date">
          <Input id="ledgerToDate" type="date" value={toDate} onChange={(e) => setToDate(e.target.value)} />
        </FormField>
      </FormGrid>
      <div className="flex justify-end">
        <Button onClick={fetchRows} disabled={!accountId}><Search className="mr-2 h-4 w-4" /> Search</Button>
      </div>
      <DataTable columns={columns} data={rows} loading={loading} emptyText="Select an account to load ledger activity." />
    </PageLayout>
  );
}
