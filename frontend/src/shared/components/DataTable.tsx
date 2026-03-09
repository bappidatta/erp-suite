import { useState, type ReactNode } from "react";
import type { ColumnDef } from "@tanstack/react-table";
import {
  getFilteredRowModel,
  flexRender,
  getCoreRowModel,
  getSortedRowModel,
  useReactTable,
  type ColumnFiltersState,
  type OnChangeFn,
  type SortingState,
} from "@tanstack/react-table";
import { ArrowDown, ArrowUp, ArrowUpDown } from "lucide-react";
import { Button } from "@app/components/ui/button";
import { Card, CardContent } from "@app/components/ui/card";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@app/components/ui/table";

declare module "@tanstack/react-table" {
  // eslint-disable-next-line @typescript-eslint/no-unused-vars
  interface ColumnMeta<TData, TValue> {
    className?: string;
    headerClassName?: string;
    filterComponent?: (props: {
      value: string;
      onChange: (value: string) => void;
    }) => ReactNode;
    filterId?: string; // Custom filter ID (defaults to column.id)
  }
}

interface PagedMeta {
  page: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}

interface DataTableProps<TData> {
  columns: ColumnDef<TData>[];
  data: TData[];
  loading?: boolean;
  emptyText?: string;
  filters?: ReactNode | ((api: {
    getFilterValue: (id: string) => string;
    setFilterValue: (id: string, value: string) => void;
    clearFilters: () => void;
  }) => ReactNode);
  filtering?: {
    state: ColumnFiltersState;
    onChange: OnChangeFn<ColumnFiltersState>;
    manual?: boolean;
  };
  sorting?: {
    state: SortingState;
    onChange: OnChangeFn<SortingState>;
    manual?: boolean;
  };
  pagination?: {
    result: PagedMeta;
    onPrevious: () => void;
    onNext: () => void;
  };
}

export function DataTable<TData>({
  columns,
  data,
  loading = false,
  emptyText = "No results found.",
  filters,
  filtering,
  sorting,
  pagination,
}: DataTableProps<TData>) {
  const [internalColumnFilters, setInternalColumnFilters] = useState<ColumnFiltersState>([]);
  const [internalSorting, setInternalSorting] = useState<SortingState>([]);
  const columnFilters = filtering?.state ?? internalColumnFilters;
  const onColumnFiltersChange = filtering?.onChange ?? setInternalColumnFilters;
  const manualFiltering = filtering?.manual ?? Boolean(filtering);
  const sortingState = sorting?.state ?? internalSorting;
  const onSortingChange = sorting?.onChange ?? setInternalSorting;
  const manualSorting = sorting?.manual ?? Boolean(sorting);

  const filterApi = {
    getFilterValue: (id: string) => String(columnFilters.find((item) => item.id === id)?.value ?? ""),
    setFilterValue: (id: string, value: string) => {
      onColumnFiltersChange((current) => {
        const next = [...current];
        const index = next.findIndex((item) => item.id === id);
        if (!value) {
          if (index >= 0) next.splice(index, 1);
          return next;
        }
        if (index >= 0) {
          next[index] = { id, value };
        } else {
          next.push({ id, value });
        }
        return next;
      });
    },
    clearFilters: () => onColumnFiltersChange([]),
  };

  const table = useReactTable({
    data,
    columns,
    state: { sorting: sortingState, columnFilters },
    onColumnFiltersChange,
    onSortingChange,
    getCoreRowModel: getCoreRowModel(),
    getFilteredRowModel: manualFiltering ? undefined : getFilteredRowModel(),
    getSortedRowModel: manualSorting ? undefined : getSortedRowModel(),
    manualFiltering,
    manualSorting,
  });

  return (
    <div className="space-y-4">
      {filters && (
        <div className="flex flex-wrap gap-3">
          {typeof filters === "function" ? filters(filterApi) : filters}
        </div>
      )}
      <Card>
        <CardContent className="p-0">
          <Table>
            <TableHeader>
              {table.getHeaderGroups().map((headerGroup) => (
                <TableRow key={headerGroup.id}>
                  {headerGroup.headers.map((header) => (
                    <TableHead
                      key={header.id}
                      className={header.column.columnDef.meta?.headerClassName}
                      onClick={header.column.getCanSort() ? header.column.getToggleSortingHandler() : undefined}
                      style={{ cursor: header.column.getCanSort() ? "pointer" : "default" }}
                    >
                      {header.isPlaceholder ? null : (
                        <div className={header.column.getCanSort() ? "flex items-center gap-1 select-none" : undefined}>
                          {flexRender(header.column.columnDef.header, header.getContext())}
                          {header.column.getCanSort() && (
                            header.column.getIsSorted() === "asc" ? (
                              <ArrowUp className="h-3.5 w-3.5" />
                            ) : header.column.getIsSorted() === "desc" ? (
                              <ArrowDown className="h-3.5 w-3.5" />
                            ) : (
                              <ArrowUpDown className="h-3.5 w-3.5 opacity-40" />
                            )
                          )}
                        </div>
                      )}
                    </TableHead>
                  ))}
                </TableRow>
              ))}
              {/* Filter Row */}
              {table.getHeaderGroups().some((headerGroup) =>
                headerGroup.headers.some((header) => header.column.columnDef.meta?.filterComponent)
              ) && (
                <TableRow>
                  {table.getHeaderGroups()[0]?.headers.map((header) => {
                    const filterId = header.column.columnDef.meta?.filterId ?? header.column.id;
                    return (
                      <TableHead key={`${header.id}-filter`} className="py-2">
                        {header.column.columnDef.meta?.filterComponent &&
                          header.column.columnDef.meta.filterComponent({
                            value: filterApi.getFilterValue(filterId),
                            onChange: (value) => filterApi.setFilterValue(filterId, value),
                          })}
                      </TableHead>
                    );
                  })}
                </TableRow>
              )}
            </TableHeader>
            <TableBody>
              {(loading || data.length === 0) && (
                <TableRow>
                  <TableCell colSpan={columns.length} className="py-10 text-center text-muted-foreground">
                    {loading ? "Loading…" : emptyText}
                  </TableCell>
                </TableRow>
              )}
              {!loading &&
                table.getRowModel().rows.map((row) => (
                  <TableRow key={row.id}>
                    {row.getVisibleCells().map((cell) => (
                      <TableCell
                        key={cell.id}
                        className={cell.column.columnDef.meta?.className}
                      >
                        {flexRender(cell.column.columnDef.cell, cell.getContext())}
                      </TableCell>
                    ))}
                  </TableRow>
                ))}
            </TableBody>
          </Table>
        </CardContent>
      </Card>
      {pagination && pagination.result.totalPages > 1 && (
        <div className="flex items-center justify-between text-sm">
          <span className="text-muted-foreground">
            Showing {(pagination.result.page - 1) * pagination.result.pageSize + 1}–
            {Math.min(pagination.result.page * pagination.result.pageSize, pagination.result.totalCount)} of{" "}
            {pagination.result.totalCount}
          </span>
          <div className="flex gap-2">
            <Button
              variant="outline"
              size="sm"
              disabled={!pagination.result.hasPreviousPage}
              onClick={pagination.onPrevious}
            >
              Previous
            </Button>
            <Button
              variant="outline"
              size="sm"
              disabled={!pagination.result.hasNextPage}
              onClick={pagination.onNext}
            >
              Next
            </Button>
          </div>
        </div>
      )}
    </div>
  );
}
