import { useEffect, useState, useCallback } from "react";
import type { ColumnDef, ColumnFiltersState, OnChangeFn, SortingState } from "@tanstack/react-table";
import { Plus, Search, Trash2, Edit, ChevronRight, ChevronDown, FolderTree } from "lucide-react";
import { Button } from "@app/components/ui/button";
import { Input } from "@app/components/ui/input";
import { Badge } from "@app/components/ui/badge";
import {
  Card, CardContent,
} from "@app/components/ui/card";
import {
  Table, TableBody, TableCell, TableHead, TableHeader, TableRow,
} from "@app/components/ui/table";
import {
  Dialog, DialogContent, DialogHeader, DialogTitle,
} from "@app/components/ui/dialog";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@app/components/ui/select";
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@app/components/ui/tabs";
import {
  PageHeader, DeleteDialog, FormError,
  PageLayout, DataTable, FormField, FormGrid, FormActions, StatusBadge,
  ColumnFilterInput, ColumnFilterSelect,
} from "@shared/components";
import { getAccounts, getAccountTree, createAccount, updateAccount, deleteAccount } from "../api/financeApi";
import type { Account, AccountTreeNode, CreateAccountRequest, UpdateAccountRequest, PagedResult } from "../types";

const ACCOUNT_TYPES = [
  { value: "0", label: "Asset" },
  { value: "1", label: "Liability" },
  { value: "2", label: "Equity" },
  { value: "3", label: "Revenue" },
  { value: "4", label: "Expense" },
];

const TYPE_OPTIONS = [
  { value: "", label: "All Types" },
  ...ACCOUNT_TYPES,
];

function AccountForm({
  account,
  accounts,
  onSave,
  onCancel,
}: {
  account?: Account;
  accounts: Account[];
  onSave: (data: CreateAccountRequest | UpdateAccountRequest) => Promise<void>;
  onCancel: () => void;
}) {
  const [form, setForm] = useState({
    code: account?.code ?? "",
    name: account?.name ?? "",
    type: account?.type?.toString() ?? "0",
    description: account?.description ?? "",
    parentId: account?.parentId?.toString() ?? "",
    isHeader: account?.isHeader ?? false,
  });
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState("");

  const parentOptions = accounts.filter((a) => a.id !== account?.id && a.isHeader);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setSaving(true);
    setError("");
    try {
      const common = {
        name: form.name,
        type: Number(form.type),
        description: form.description || undefined,
        parentId: form.parentId ? Number(form.parentId) : undefined,
        isHeader: form.isHeader,
      };
      if (account) {
        await onSave(common as UpdateAccountRequest);
      } else {
        await onSave({ code: form.code, ...common } as CreateAccountRequest);
      }
    } catch (err) {
      setError(err instanceof Error ? err.message : "An error occurred.");
      setSaving(false);
    }
  };

  return (
    <form onSubmit={handleSubmit} className="space-y-4">
      <FormError error={error} />

      {!account && (
        <FormField id="code" label="Code">
          <Input id="code" required value={form.code} onChange={(e) => setForm((f) => ({ ...f, code: e.target.value }))} />
        </FormField>
      )}

      <FormField id="name" label="Name">
        <Input id="name" required value={form.name} onChange={(e) => setForm((f) => ({ ...f, name: e.target.value }))} />
      </FormField>

      <FormGrid>
        <FormField id="type" label="Account Type">
          <Select value={form.type} onValueChange={(v) => setForm((f) => ({ ...f, type: v ?? "0" }))}>
            <SelectTrigger><SelectValue /></SelectTrigger>
            <SelectContent>
              {ACCOUNT_TYPES.map((t) => (
                <SelectItem key={t.value} value={t.value}>{t.label}</SelectItem>
              ))}
            </SelectContent>
          </Select>
        </FormField>
        <FormField id="parentId" label="Parent Account">
          <Select value={form.parentId} onValueChange={(v) => setForm((f) => ({ ...f, parentId: v ?? "" }))}>
            <SelectTrigger><SelectValue placeholder="None (Top-level)" /></SelectTrigger>
            <SelectContent>
              <SelectItem value="">None (Top-level)</SelectItem>
              {parentOptions.map((a) => (
                <SelectItem key={a.id} value={a.id.toString()}>{a.code} — {a.name}</SelectItem>
              ))}
            </SelectContent>
          </Select>
        </FormField>
      </FormGrid>

      <FormField id="description" label="Description">
        <Input id="description" value={form.description} onChange={(e) => setForm((f) => ({ ...f, description: e.target.value }))} />
      </FormField>

      <label className="flex items-center gap-2 text-sm">
        <input type="checkbox" checked={form.isHeader} onChange={(e) => setForm((f) => ({ ...f, isHeader: e.target.checked }))} className="rounded" />
        Header Account (group node, not postable)
      </label>

      <FormActions onCancel={onCancel} saving={saving} saveLabel={account ? "Update Account" : "Create Account"} />
    </form>
  );
}

function TreeNode({ node, onEdit, onDelete, depth = 0 }: {
  node: AccountTreeNode;
  onEdit: (id: number) => void;
  onDelete: (node: AccountTreeNode) => void;
  depth?: number;
}) {
  const [expanded, setExpanded] = useState(depth < 2);
  const hasChildren = node.children.length > 0;

  return (
    <>
      <TableRow className={node.isHeader ? "bg-muted/30" : ""}>
        <TableCell style={{ paddingLeft: `${depth * 24 + 12}px` }}>
          <div className="flex items-center gap-1">
            {hasChildren ? (
              <button onClick={() => setExpanded(!expanded)} className="p-0.5 rounded hover:bg-muted">
                {expanded ? <ChevronDown className="h-4 w-4" /> : <ChevronRight className="h-4 w-4" />}
              </button>
            ) : (
              <span className="w-5" />
            )}
            <span className="font-medium">{node.code}</span>
          </div>
        </TableCell>
        <TableCell className={node.isHeader ? "font-semibold" : ""}>{node.name}</TableCell>
        <TableCell><Badge variant="outline">{node.typeName}</Badge></TableCell>
        <TableCell>{node.isHeader ? "Header" : "Postable"}</TableCell>
        <TableCell className="text-right">
          <div className="flex items-center justify-end gap-1">
            <Button variant="ghost" size="sm" onClick={() => onEdit(node.id)} title="Edit"><Edit className="h-4 w-4" /></Button>
            {!hasChildren && (
              <Button variant="ghost" size="sm" onClick={() => onDelete(node)} title="Delete"><Trash2 className="h-4 w-4 text-destructive" /></Button>
            )}
          </div>
        </TableCell>
      </TableRow>
      {expanded && node.children.map((child) => (
        <TreeNode key={child.id} node={child} onEdit={onEdit} onDelete={onDelete} depth={depth + 1} />
      ))}
    </>
  );
}

export function ChartOfAccountsPage() {
  const [tab, setTab] = useState("tree");
  const [tree, setTree] = useState<AccountTreeNode[]>([]);
  const [result, setResult] = useState<PagedResult<Account> | null>(null);
  const [allAccounts, setAllAccounts] = useState<Account[]>([]);
  const [loading, setLoading] = useState(true);
  const [columnFilters, setColumnFilters] = useState<ColumnFiltersState>([]);
  const [sorting, setSorting] = useState<SortingState>([]);
  const [page, setPage] = useState(1);
  const [formOpen, setFormOpen] = useState(false);
  const [editingAccount, setEditingAccount] = useState<Account | undefined>(undefined);
  const [deleteTarget, setDeleteTarget] = useState<AccountTreeNode | Account | null>(null);
  const [deleting, setDeleting] = useState(false);

  const mapSortField = (columnId: string) => {
    switch (columnId) {
      case "code":
        return "code";
      case "name":
        return "name";
      case "typeName":
        return "type";
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

  const fetchTree = useCallback(() => {
    setLoading(true);
    getAccountTree()
      .then(setTree)
      .finally(() => setLoading(false));
  }, []);

  const fetchList = useCallback(() => {
    const search = String(columnFilters.find((filter) => filter.id === "searchTerm")?.value ?? "");
    const typeFilter = String(columnFilters.find((filter) => filter.id === "type")?.value ?? "");
    const params: Record<string, string> = { page: String(page), pageSize: "50" };
    if (search) params.searchTerm = search;
    if (typeFilter) params.type = typeFilter;
    if (sorting[0]) {
      params.sortBy = mapSortField(sorting[0].id);
      params.sortDescending = String(sorting[0].desc);
    }
    setLoading(true);
    getAccounts(params)
      .then(setResult)
      .finally(() => setLoading(false));
  }, [columnFilters, page, sorting]);

  const fetchAllAccounts = useCallback(() => {
    getAccounts({ page: "1", pageSize: "500" }).then((r) => {
      if (r) setAllAccounts(r.items);
    });
  }, []);

  useEffect(() => { fetchAllAccounts(); }, [fetchAllAccounts]);
  useEffect(() => { if (tab === "tree") fetchTree(); else fetchList(); }, [tab, fetchTree, fetchList]);

  const handleEdit = async (id: number) => {
    const acct = allAccounts.find((a) => a.id === id);
    if (acct) {
      setEditingAccount(acct);
      setFormOpen(true);
    }
  };

  const handleSave = async (data: CreateAccountRequest | UpdateAccountRequest) => {
    if (editingAccount) {
      await updateAccount(editingAccount.id, data as UpdateAccountRequest);
    } else {
      await createAccount(data as CreateAccountRequest);
    }
    setFormOpen(false);
    setEditingAccount(undefined);
    fetchAllAccounts();
    if (tab === "tree") fetchTree(); else fetchList();
  };

  const handleDelete = async () => {
    if (!deleteTarget) return;
    setDeleting(true);
    try {
      await deleteAccount(deleteTarget.id);
      setDeleteTarget(null);
      fetchAllAccounts();
      if (tab === "tree") fetchTree(); else fetchList();
    } finally {
      setDeleting(false);
    }
  };

  const accounts = result?.items ?? [];

  const listColumns: ColumnDef<Account>[] = [
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
      accessorKey: "typeName",
      header: "Type",
      cell: ({ row }) => <Badge variant="outline">{row.original.typeName}</Badge>,
      enableSorting: false,
      meta: {
        filterId: "type",
        filterComponent: ({ value, onChange }) => (
          <ColumnFilterSelect
            value={value}
            onChange={onChange}
            options={TYPE_OPTIONS}
            placeholder="All Types"
          />
        ),
      },
    },
    {
      accessorKey: "parentName",
      header: "Parent",
      cell: ({ row }) => <span className="text-muted-foreground">{row.original.parentName ?? "—"}</span>,
    },
    {
      accessorKey: "isHeader",
      header: "Kind",
      cell: ({ row }) => row.original.isHeader ? "Header" : "Postable",
      enableSorting: false,
    },
    {
      accessorKey: "isActive",
      header: "Status",
      cell: ({ row }) => <StatusBadge isActive={row.original.isActive} />,
      enableSorting: false,
    },
    {
      id: "actions",
      header: "Actions",
      enableSorting: false,
      meta: { className: "text-right", headerClassName: "text-right" },
      cell: ({ row }) => (
        <div className="flex items-center justify-end gap-1">
          <Button variant="ghost" size="sm" onClick={() => handleEdit(row.original.id)} title="Edit">
            <Edit className="h-4 w-4" />
          </Button>
          <Button variant="ghost" size="sm" onClick={() => setDeleteTarget(row.original)} title="Delete">
            <Trash2 className="h-4 w-4 text-destructive" />
          </Button>
        </div>
      ),
    },
  ];

  return (
    <PageLayout>
      <PageHeader
        title="Chart of Accounts"
        description="Manage general ledger accounts"
        action={
          <Button onClick={() => { setEditingAccount(undefined); setFormOpen(true); }}>
            <Plus className="mr-2 h-4 w-4" /> Add Account
          </Button>
        }
      />

      <Tabs value={tab} onValueChange={setTab}>
        <TabsList>
          <TabsTrigger value="tree"><FolderTree className="mr-1 h-4 w-4" /> Tree View</TabsTrigger>
          <TabsTrigger value="list"><Search className="mr-1 h-4 w-4" /> List View</TabsTrigger>
        </TabsList>

        <TabsContent value="tree" className="space-y-4">
          <Card>
            <CardContent className="p-0">
              <Table>
                <TableHeader>
                  <TableRow>
                    <TableHead>Code</TableHead>
                    <TableHead>Name</TableHead>
                    <TableHead>Type</TableHead>
                    <TableHead>Kind</TableHead>
                    <TableHead className="text-right">Actions</TableHead>
                  </TableRow>
                </TableHeader>
                <TableBody>
                    {loading ? (
                      <TableRow>
                        <TableCell colSpan={5} className="py-10 text-center text-muted-foreground">Loading…</TableCell>
                      </TableRow>
                    ) : tree.length === 0 ? (
                      <TableRow>
                        <TableCell colSpan={5} className="py-10 text-center text-muted-foreground">No accounts found.</TableCell>
                      </TableRow>
                    ) : tree.map((node) => (
                      <TreeNode key={node.id} node={node} onEdit={handleEdit} onDelete={setDeleteTarget} />
                    ))}
                </TableBody>
              </Table>
            </CardContent>
          </Card>
        </TabsContent>

        <TabsContent value="list" className="space-y-4">
          <DataTable
            columns={listColumns}
            data={accounts}
            loading={loading}
            emptyText="No accounts found."
            filtering={{ state: columnFilters, onChange: handleFilteringChange, manual: true }}
            sorting={{ state: sorting, onChange: handleSortingChange, manual: true }}
            pagination={result ? { result, onPrevious: () => setPage((p) => p - 1), onNext: () => setPage((p) => p + 1) } : undefined}
          />
        </TabsContent>
      </Tabs>

      <Dialog open={formOpen} onOpenChange={setFormOpen}>
        <DialogContent className="sm:max-w-md">
          <DialogHeader><DialogTitle>{editingAccount ? "Edit Account" : "Create Account"}</DialogTitle></DialogHeader>
          <AccountForm account={editingAccount} accounts={allAccounts} onSave={handleSave} onCancel={() => { setFormOpen(false); setEditingAccount(undefined); }} />
        </DialogContent>
      </Dialog>

      <DeleteDialog
        open={!!deleteTarget}
        onOpenChange={(open) => { if (!open) setDeleteTarget(null); }}
        title="Delete Account"
        entityLabel={deleteTarget ? `${deleteTarget.name} (${deleteTarget.code})` : ""}
        onConfirm={handleDelete}
        deleting={deleting}
      />
    </PageLayout>
  );
}
