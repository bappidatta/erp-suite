import { useCallback, useEffect, useState } from "react";
import type { ColumnDef } from "@tanstack/react-table";
import { Search } from "lucide-react";
import { Button } from "@app/components/ui/button";
import { Input } from "@app/components/ui/input";
import { DataTable, FormField, FormGrid, PageHeader, PageLayout } from "@shared/components";
import { getTrialBalance } from "../api/financeApi";
import type { TrialBalanceRow } from "../types";

export function TrialBalancePage() {
  const [rows, setRows] = useState<TrialBalanceRow[]>([]);
  const [loading, setLoading] = useState(true);
  const [fromDate, setFromDate] = useState("");
  const [toDate, setToDate] = useState("");

  const fetchRows = useCallback(() => {
    const params: Record<string, string> = {};
    if (fromDate) params.fromDate = fromDate;
    if (toDate) params.toDate = toDate;
    setLoading(true);
    getTrialBalance(params).then(setRows).finally(() => setLoading(false));
  }, [fromDate, toDate]);

  useEffect(() => { fetchRows(); }, [fetchRows]);

  const columns: ColumnDef<TrialBalanceRow>[] = [
    { accessorKey: "accountCode", header: "Account Code" },
    { accessorKey: "accountName", header: "Account Name" },
    { accessorKey: "totalDebit", header: "Debit", cell: ({ row }) => row.original.totalDebit.toFixed(2) },
    { accessorKey: "totalCredit", header: "Credit", cell: ({ row }) => row.original.totalCredit.toFixed(2) },
    { accessorKey: "netBalance", header: "Net Balance", cell: ({ row }) => row.original.netBalance.toFixed(2) },
  ];

  return (
    <PageLayout>
      <PageHeader title="Trial Balance" description="Review debits and credits by account." />
      <FormGrid>
        <FormField id="tbFromDate" label="From Date">
          <Input id="tbFromDate" type="date" value={fromDate} onChange={(e) => setFromDate(e.target.value)} />
        </FormField>
        <FormField id="tbToDate" label="To Date">
          <Input id="tbToDate" type="date" value={toDate} onChange={(e) => setToDate(e.target.value)} />
        </FormField>
      </FormGrid>
      <div className="flex justify-end">
        <Button onClick={fetchRows}><Search className="mr-2 h-4 w-4" /> Refresh</Button>
      </div>
      <DataTable columns={columns} data={rows} loading={loading} emptyText="No trial balance rows found." />
    </PageLayout>
  );
}
