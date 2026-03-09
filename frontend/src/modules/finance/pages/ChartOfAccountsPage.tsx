import { useEffect, useState, useCallback } from "react";
import { Plus, Search, Trash2, Edit, ChevronRight, ChevronDown, FolderTree } from "lucide-react";
import { Card, CardContent, CardHeader, CardTitle } from "@app/components/ui/card";
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
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@app/components/ui/tabs";
import { getAccounts, getAccountTree, createAccount, updateAccount, deleteAccount } from "../api/financeApi";
import type { Account, AccountTreeNode, CreateAccountRequest, UpdateAccountRequest, PagedResult } from "../types";

const ACCOUNT_TYPES = [
  { value: "0", label: "Asset" },
  { value: "1", label: "Liability" },
  { value: "2", label: "Equity" },
  { value: "3", label: "Revenue" },
  { value: "4", label: "Expense" },
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
      {error && <div className="rounded-md bg-destructive/10 p-3 text-sm text-destructive">{error}</div>}

      {!account && (
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
          <Label htmlFor="type">Account Type</Label>
          <Select value={form.type} onValueChange={(v) => setForm((f) => ({ ...f, type: v ?? "0" }))}>
            <SelectTrigger><SelectValue /></SelectTrigger>
            <SelectContent>
              {ACCOUNT_TYPES.map((t) => (
                <SelectItem key={t.value} value={t.value}>{t.label}</SelectItem>
              ))}
            </SelectContent>
          </Select>
        </div>
        <div className="space-y-1">
          <Label htmlFor="parentId">Parent Account</Label>
          <Select value={form.parentId} onValueChange={(v) => setForm((f) => ({ ...f, parentId: v ?? "" }))}>
            <SelectTrigger><SelectValue placeholder="None (Top-level)" /></SelectTrigger>
            <SelectContent>
              <SelectItem value="">None (Top-level)</SelectItem>
              {parentOptions.map((a) => (
                <SelectItem key={a.id} value={a.id.toString()}>{a.code} — {a.name}</SelectItem>
              ))}
            </SelectContent>
          </Select>
        </div>
      </div>

      <div className="space-y-1">
        <Label htmlFor="description">Description</Label>
        <Input id="description" value={form.description} onChange={(e) => setForm((f) => ({ ...f, description: e.target.value }))} />
      </div>

      <label className="flex items-center gap-2 text-sm">
        <input type="checkbox" checked={form.isHeader} onChange={(e) => setForm((f) => ({ ...f, isHeader: e.target.checked }))} className="rounded" />
        Header Account (group node, not postable)
      </label>

      <div className="flex justify-end gap-2 pt-2">
        <Button type="button" variant="outline" onClick={onCancel} disabled={saving}>Cancel</Button>
        <Button type="submit" disabled={saving}>
          {saving ? "Saving…" : account ? "Update Account" : "Create Account"}
        </Button>
      </div>
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
  const [search, setSearch] = useState("");
  const [typeFilter, setTypeFilter] = useState("");
  const [page, setPage] = useState(1);
  const [formOpen, setFormOpen] = useState(false);
  const [editingAccount, setEditingAccount] = useState<Account | undefined>(undefined);
  const [deleteTarget, setDeleteTarget] = useState<AccountTreeNode | Account | null>(null);
  const [deleting, setDeleting] = useState(false);

  const fetchTree = useCallback(() => {
    setLoading(true);
    getAccountTree()
      .then(setTree)
      .finally(() => setLoading(false));
  }, []);

  const fetchList = useCallback(() => {
    const params: Record<string, string> = { page: String(page), pageSize: "50" };
    if (search) params.searchTerm = search;
    if (typeFilter) params.type = typeFilter;
    setLoading(true);
    getAccounts(params)
      .then(setResult)
      .finally(() => setLoading(false));
  }, [page, search, typeFilter]);

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

  return (
    <div className="space-y-4">
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-2xl font-bold tracking-tight">Chart of Accounts</h1>
          <p className="text-muted-foreground">Manage general ledger accounts</p>
        </div>
        <Button onClick={() => { setEditingAccount(undefined); setFormOpen(true); }}>
          <Plus className="mr-2 h-4 w-4" /> Add Account
        </Button>
      </div>

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
                    <TableRow><TableCell colSpan={5} className="py-10 text-center text-muted-foreground">Loading…</TableCell></TableRow>
                  ) : tree.length === 0 ? (
                    <TableRow><TableCell colSpan={5} className="py-10 text-center text-muted-foreground">No accounts found.</TableCell></TableRow>
                  ) : (
                    tree.map((node) => (
                      <TreeNode key={node.id} node={node} onEdit={handleEdit} onDelete={setDeleteTarget} />
                    ))
                  )}
                </TableBody>
              </Table>
            </CardContent>
          </Card>
        </TabsContent>

        <TabsContent value="list" className="space-y-4">
          <div className="flex flex-wrap gap-3">
            <div className="relative flex-1 min-w-[200px]">
              <Search className="absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-muted-foreground" />
              <Input placeholder="Search by name or code…" className="pl-9" value={search}
                onChange={(e) => { setSearch(e.target.value); setPage(1); }} />
            </div>
            <Select value={typeFilter} onValueChange={(v) => { setTypeFilter(v ?? ""); setPage(1); }}>
              <SelectTrigger className="w-36"><SelectValue placeholder="All Types" /></SelectTrigger>
              <SelectContent>
                <SelectItem value="">All Types</SelectItem>
                {ACCOUNT_TYPES.map((t) => (
                  <SelectItem key={t.value} value={t.value}>{t.label}</SelectItem>
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
                    <TableHead>Type</TableHead>
                    <TableHead>Parent</TableHead>
                    <TableHead>Kind</TableHead>
                    <TableHead>Status</TableHead>
                    <TableHead className="text-right">Actions</TableHead>
                  </TableRow>
                </TableHeader>
                <TableBody>
                  {loading ? (
                    <TableRow><TableCell colSpan={7} className="py-10 text-center text-muted-foreground">Loading…</TableCell></TableRow>
                  ) : accounts.length === 0 ? (
                    <TableRow><TableCell colSpan={7} className="py-10 text-center text-muted-foreground">No accounts found.</TableCell></TableRow>
                  ) : (
                    accounts.map((a) => (
                      <TableRow key={a.id}>
                        <TableCell className="font-medium">{a.code}</TableCell>
                        <TableCell>{a.name}</TableCell>
                        <TableCell><Badge variant="outline">{a.typeName}</Badge></TableCell>
                        <TableCell className="text-muted-foreground">{a.parentName ?? "—"}</TableCell>
                        <TableCell>{a.isHeader ? "Header" : "Postable"}</TableCell>
                        <TableCell>
                          <Badge variant={a.isActive ? "default" : "secondary"}>{a.isActive ? "Active" : "Inactive"}</Badge>
                        </TableCell>
                        <TableCell className="text-right">
                          <div className="flex items-center justify-end gap-1">
                            <Button variant="ghost" size="sm" onClick={() => handleEdit(a.id)} title="Edit">
                              <Edit className="h-4 w-4" />
                            </Button>
                            <Button variant="ghost" size="sm" onClick={() => setDeleteTarget(a)} title="Delete">
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
        </TabsContent>
      </Tabs>

      <Dialog open={formOpen} onOpenChange={setFormOpen}>
        <DialogContent className="sm:max-w-md">
          <DialogHeader><DialogTitle>{editingAccount ? "Edit Account" : "Create Account"}</DialogTitle></DialogHeader>
          <AccountForm account={editingAccount} accounts={allAccounts} onSave={handleSave} onCancel={() => { setFormOpen(false); setEditingAccount(undefined); }} />
        </DialogContent>
      </Dialog>

      <Dialog open={!!deleteTarget} onOpenChange={(open) => { if (!open) setDeleteTarget(null); }}>
        <DialogContent className="sm:max-w-sm">
          <DialogHeader><DialogTitle>Delete Account</DialogTitle></DialogHeader>
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
