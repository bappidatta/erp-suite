import { useEffect, useState, useCallback } from "react";
import type { ColumnDef } from "@tanstack/react-table";
import { Search } from "lucide-react";
import { Card, CardContent } from "@app/components/ui/card";
import { Button } from "@app/components/ui/button";
import { Input } from "@app/components/ui/input";
import { Badge } from "@app/components/ui/badge";
import { getAuditLogs } from "../api/adminApi";
import type { AuditLog, PagedResult } from "../types";
import { PageLayout, PageHeader, FormField, DataTable } from "@shared/components";

const MODULES = ["Admin", "Finance", "HR", "Inventory", "MasterData", "Procurement", "Reporting", "Sales"];

const ACTION_COLORS: Record<string, string> = {
  Create: "bg-green-100 text-green-800",
  Update: "bg-blue-100 text-blue-800",
  Delete: "bg-red-100 text-red-800",
  Login: "bg-purple-100 text-purple-800",
  Logout: "bg-gray-100 text-gray-700",
};

function actionBadgeClass(action: string) {
  return ACTION_COLORS[action] ?? "bg-muted text-muted-foreground";
}

export function AuditLogPage() {
  const [result, setResult] = useState<PagedResult<AuditLog> | null>(null);
  const [loading, setLoading] = useState(true);
  const [page, setPage] = useState(1);
  const [filters, setFilters] = useState({
    module: "",
    userId: "",
    dateFrom: "",
    dateTo: "",
  });
  const [applied, setApplied] = useState(filters);

  const load = useCallback(async () => {
    setLoading(true);
    try {
      const params: Record<string, string> = { page: String(page), pageSize: "20" };
      if (applied.module) params.module = applied.module;
      if (applied.userId) params.userId = applied.userId;
      if (applied.dateFrom) params.dateFrom = applied.dateFrom;
      if (applied.dateTo) params.dateTo = applied.dateTo;
      const data = await getAuditLogs(params);
      setResult(data);
    } finally {
      setLoading(false);
    }
  }, [page, applied]);

  useEffect(() => { load(); }, [load]);

  const handleApply = () => {
    setPage(1);
    setApplied(filters);
  };

  const handleReset = () => {
    const empty = { module: "", userId: "", dateFrom: "", dateTo: "" };
    setFilters(empty);
    setApplied(empty);
    setPage(1);
  };

  const formatDate = (iso: string) =>
    new Date(iso).toLocaleString(undefined, { dateStyle: "short", timeStyle: "medium" });

  const columns: ColumnDef<AuditLog>[] = [
    {
      accessorKey: "createdAt",
      header: "Timestamp",
      cell: ({ row }) => (
        <span className="whitespace-nowrap text-xs text-muted-foreground">
          {formatDate(row.original.createdAt)}
        </span>
      ),
    },
    {
      id: "user",
      header: "User",
      enableSorting: false,
      cell: ({ row }) => {
        const log = row.original;
        return log.userName ?? (log.userId ? `User #${log.userId}` : "System");
      },
    },
    {
      accessorKey: "action",
      header: "Action",
      cell: ({ row }) => (
        <span className={`inline-flex rounded-full px-2 py-0.5 text-xs font-medium ${actionBadgeClass(row.original.action)}`}>
          {row.original.action}
        </span>
      ),
    },
    {
      accessorKey: "module",
      header: "Module",
      cell: ({ row }) => <Badge variant="outline" className="text-xs">{row.original.module}</Badge>,
    },
    {
      accessorKey: "entityId",
      header: "Entity ID",
      cell: ({ row }) => <span className="font-mono text-xs">{row.original.entityId ?? "—"}</span>,
    },
    {
      accessorKey: "ipAddress",
      header: "IP Address",
      cell: ({ row }) => <span className="text-xs text-muted-foreground">{row.original.ipAddress ?? "—"}</span>,
    },
  ];

  return (
    <PageLayout>
      <PageHeader title="Audit Log" description="Track all system actions and changes" />

      {/* Filters */}
      <Card>
        <CardContent className="pt-4">
          <div className="grid grid-cols-2 gap-4 sm:grid-cols-4">
            <FormField id="module" label="Module">
              <select
                id="module"
                className="flex h-9 w-full rounded-md border border-input bg-background px-3 py-1 text-sm shadow-sm"
                value={filters.module}
                onChange={(e) => setFilters((f) => ({ ...f, module: e.target.value }))}
              >
                <option value="">All Modules</option>
                {MODULES.map((m) => <option key={m} value={m}>{m}</option>)}
              </select>
            </FormField>
            <FormField id="userId" label="User ID">
              <Input
                id="userId"
                placeholder="e.g. 42"
                value={filters.userId}
                onChange={(e) => setFilters((f) => ({ ...f, userId: e.target.value }))}
              />
            </FormField>
            <FormField id="dateFrom" label="From">
              <Input
                id="dateFrom"
                type="date"
                value={filters.dateFrom}
                onChange={(e) => setFilters((f) => ({ ...f, dateFrom: e.target.value }))}
              />
            </FormField>
            <FormField id="dateTo" label="To">
              <Input
                id="dateTo"
                type="date"
                value={filters.dateTo}
                onChange={(e) => setFilters((f) => ({ ...f, dateTo: e.target.value }))}
              />
            </FormField>
          </div>
          <div className="mt-3 flex gap-2">
            <Button size="sm" onClick={handleApply}>
              <Search className="mr-2 h-3.5 w-3.5" />Apply
            </Button>
            <Button size="sm" variant="outline" onClick={handleReset}>Reset</Button>
          </div>
        </CardContent>
      </Card>

      <DataTable columns={columns} data={result?.items ?? []} loading={loading} emptyText="No audit logs found."
        pagination={result ? { result, onPrevious: () => setPage((p) => p - 1), onNext: () => setPage((p) => p + 1) } : undefined}
      />
    </PageLayout>
  );
}
