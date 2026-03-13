# Page Component Template

## Full CRUD Page Structure

```tsx
import { useCallback, useEffect, useState } from "react";
import type { ColumnDef, ColumnFiltersState, SortingState } from "@tanstack/react-table";
import { MoreHorizontal, Pencil, Plus, Power, PowerOff, Trash2 } from "lucide-react";
import { Button } from "@app/components/ui/button";
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
} from "@app/components/ui/dialog";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from "@app/components/ui/dropdown-menu";
import { DataTable } from "@shared/components/DataTable";
import { DeleteDialog } from "@shared/components/DeleteDialog";
import { PageHeader } from "@shared/components/PageHeader";
import { PageLayout } from "@shared/components/PageLayout";
import { StatusBadge } from "@shared/components/StatusBadge";
import { ColumnFilterInput, ColumnFilterSelect } from "@shared/components/ColumnFilters";
import type { {Entity}, PagedResult } from "../types";
import {
  get{Entities},
  create{Entity},
  update{Entity},
  delete{Entity},
  activate{Entity},
  deactivate{Entity},
} from "../api/{module}Api";

// ── Inline Form Component ──────────────────────────────────────
// (See form-component.md reference for the full pattern)
function {Entity}Form({ ... }) { /* ... */ }

// ── Page Component ─────────────────────────────────────────────
export function {Entities}Page() {
  // Data state
  const [result, setResult] = useState<PagedResult<{Entity}> | null>(null);
  const [loading, setLoading] = useState(true);

  // Table state
  const [columnFilters, setColumnFilters] = useState<ColumnFiltersState>([]);
  const [sorting, setSorting] = useState<SortingState>([]);
  const [page, setPage] = useState(1);

  // Dialog state
  const [formOpen, setFormOpen] = useState(false);
  const [editing{Entity}, setEditing{Entity}] = useState<{Entity} | undefined>();
  const [deleteTarget, setDeleteTarget] = useState<{Entity} | null>(null);
  const [deleting, setDeleting] = useState(false);

  // ── Fetch ────────────────────────────────────────────────────
  const fetch{Entities} = useCallback(() => {
    const params: Record<string, string> = {
      page: String(page),
      pageSize: "20",
    };

    // Map column filters to API params
    for (const f of columnFilters) {
      if (f.value) params[f.id] = String(f.value);
    }

    // Map sorting
    if (sorting.length > 0) {
      params.sortBy = sorting[0].id;
      params.sortDescending = String(sorting[0].desc);
    }

    setLoading(true);
    get{Entities}(params)
      .then(setResult)
      .catch(() => setResult(null))
      .finally(() => setLoading(false));
  }, [columnFilters, page, sorting]);

  useEffect(() => {
    fetch{Entities}();
  }, [fetch{Entities}]);

  // Reset page when filters change
  useEffect(() => {
    setPage(1);
  }, [columnFilters]);

  // ── Handlers ─────────────────────────────────────────────────
  const openCreate = () => {
    setEditing{Entity}(undefined);
    setFormOpen(true);
  };

  const openEdit = ({entity}: {Entity}) => {
    setEditing{Entity}({entity});
    setFormOpen(true);
  };

  const handleSave = async (data: /* Create or Update request */) => {
    if (editing{Entity}) {
      await update{Entity}(editing{Entity}.id, data);
    } else {
      await create{Entity}(data);
    }
    setFormOpen(false);
    fetch{Entities}();
  };

  const handleDelete = async () => {
    if (!deleteTarget) return;
    setDeleting(true);
    try {
      await delete{Entity}(deleteTarget.id);
      setDeleteTarget(null);
      fetch{Entities}();
    } finally {
      setDeleting(false);
    }
  };

  const handleToggleActive = async ({entity}: {Entity}) => {
    if ({entity}.isActive) {
      await deactivate{Entity}({entity}.id);
    } else {
      await activate{Entity}({entity}.id);
    }
    fetch{Entities}();
  };

  // ── Column Definitions ───────────────────────────────────────
  const {entities} = result?.items ?? [];

  const columns: ColumnDef<{Entity}>[] = [
    {
      accessorKey: "code",
      header: "Code",
      meta: {
        filterId: "searchTerm",
        filterComponent: ColumnFilterInput,
      },
    },
    {
      accessorKey: "name",
      header: "Name",
    },
    // ... entity-specific columns ...
    {
      accessorKey: "isActive",
      header: "Status",
      cell: ({ row }) => <StatusBadge isActive={row.original.isActive} />,
      meta: {
        filterId: "isActive",
        filterComponent: (props) => (
          <ColumnFilterSelect
            {...props}
            options={[
              { label: "Active", value: "true" },
              { label: "Inactive", value: "false" },
            ]}
          />
        ),
      },
    },
    {
      id: "actions",
      header: "",
      cell: ({ row }) => {
        const {entity} = row.original;
        return (
          <DropdownMenu>
            <DropdownMenuTrigger asChild>
              <Button variant="ghost" size="icon" className="h-8 w-8">
                <MoreHorizontal className="h-4 w-4" />
              </Button>
            </DropdownMenuTrigger>
            <DropdownMenuContent align="end">
              <DropdownMenuItem onClick={() => openEdit({entity})}>
                <Pencil className="h-4 w-4 mr-2" /> Edit
              </DropdownMenuItem>
              <DropdownMenuItem onClick={() => handleToggleActive({entity})}>
                {{entity}.isActive ? (
                  <><PowerOff className="h-4 w-4 mr-2" /> Deactivate</>
                ) : (
                  <><Power className="h-4 w-4 mr-2" /> Activate</>
                )}
              </DropdownMenuItem>
              <DropdownMenuItem
                className="text-destructive"
                onClick={() => setDeleteTarget({entity})}
              >
                <Trash2 className="h-4 w-4 mr-2" /> Delete
              </DropdownMenuItem>
            </DropdownMenuContent>
          </DropdownMenu>
        );
      },
      meta: { className: "w-12", headerClassName: "w-12" },
    },
  ];

  // ── Render ───────────────────────────────────────────────────
  return (
    <PageLayout>
      <PageHeader
        title="{Entities}"
        description="Manage {entities}"
        action={
          <Button onClick={openCreate}>
            <Plus className="h-4 w-4 mr-2" /> Add {Entity}
          </Button>
        }
      />

      <DataTable
        columns={columns}
        data={{entities}}
        loading={loading}
        emptyText="No {entities} found."
        filtering={{
          state: columnFilters,
          onChange: setColumnFilters,
          manual: true,
        }}
        sorting={{
          state: sorting,
          onChange: setSorting,
          manual: true,
        }}
        pagination={
          result
            ? {
                result,
                onPrevious: () => setPage((p) => p - 1),
                onNext: () => setPage((p) => p + 1),
              }
            : undefined
        }
      />

      {/* Create/Edit Dialog */}
      <Dialog open={formOpen} onOpenChange={setFormOpen}>
        <DialogContent className="sm:max-w-lg">
          <DialogHeader>
            <DialogTitle>
              {editing{Entity} ? "Edit {Entity}" : "Add {Entity}"}
            </DialogTitle>
          </DialogHeader>
          <{Entity}Form
            {entity}={editing{Entity}}
            onSave={handleSave}
            onCancel={() => setFormOpen(false)}
          />
        </DialogContent>
      </Dialog>

      {/* Delete Confirmation */}
      <DeleteDialog
        open={!!deleteTarget}
        onOpenChange={() => setDeleteTarget(null)}
        entityLabel={deleteTarget?.name ?? ""}
        onConfirm={handleDelete}
        deleting={deleting}
      />
    </PageLayout>
  );
}
```

## Key Patterns

- **Server-side filtering**: Column filters map directly to API query params via `filterId` in column meta
- **Server-side sorting**: Single-column sort, mapped to `sortBy` + `sortDescending` params
- **Server-side pagination**: Page state managed locally, passed to API, `PagedResult` drives pagination UI
- **Optimistic refetch**: After any mutation (create/update/delete/activate), call `fetch{Entities}()` to refresh
- **Dialog state**: Single `formOpen` boolean + optional `editing{Entity}` controls create vs. edit mode
