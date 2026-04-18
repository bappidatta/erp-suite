import { useEffect, useState, useCallback } from "react";
import type { ColumnDef, ColumnFiltersState, OnChangeFn, SortingState } from "@tanstack/react-table";
import { Plus, Trash2, Edit } from "lucide-react";
import { Button } from "@app/components/ui/button";
import { Input } from "@app/components/ui/input";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@app/components/ui/select";
import {
  Dialog, DialogContent, DialogHeader, DialogTitle,
} from "@app/components/ui/dialog";
import {
  PageHeader, DeleteDialog, FormError,
  PageLayout, DataTable, FormField, FormGrid, FormActions,
  ColumnFilterInput, ColumnFilterSelect,
} from "@shared/components";
import { getEmployees, createEmployee, updateEmployee, deleteEmployee, getDepartments } from "../api/hrApi";
import type { Employee, CreateEmployeeRequest, UpdateEmployeeRequest, PagedResult, Department } from "../types";

const STATUS_OPTIONS_FILTER = [
  { value: "", label: "All" },
  { value: "1", label: "Active" },
  { value: "2", label: "On Leave" },
  { value: "3", label: "Terminated" },
  { value: "4", label: "Resigned" },
];

const NONE_OPTION_VALUE = "__none__";

const STATUS_OPTIONS = [
  { value: "1", label: "Active" },
  { value: "2", label: "On Leave" },
  { value: "3", label: "Terminated" },
  { value: "4", label: "Resigned" },
];

const TYPE_OPTIONS = [
  { value: "1", label: "Full Time" },
  { value: "2", label: "Part Time" },
  { value: "3", label: "Contract" },
  { value: "4", label: "Intern" },
];

function EmployeeForm({
  employee,
  departments,
  onSave,
  onCancel,
}: {
  employee?: Employee;
  departments: Department[];
  onSave: (data: CreateEmployeeRequest | UpdateEmployeeRequest) => Promise<void>;
  onCancel: () => void;
}) {
  const [form, setForm] = useState({
    employeeNumber: employee?.employeeNumber ?? "",
    firstName: employee?.firstName ?? "",
    lastName: employee?.lastName ?? "",
    email: employee?.email ?? "",
    phone: employee?.phone ?? "",
    departmentId: employee?.departmentId?.toString() ?? NONE_OPTION_VALUE,
    designation: employee?.designation ?? "",
    status: employee?.status?.toString() ?? "1",
    employmentType: employee?.employmentType?.toString() ?? "1",
    dateOfJoining: employee?.dateOfJoining?.split("T")[0] ?? "",
    dateOfBirth: employee?.dateOfBirth?.split("T")[0] ?? "",
    notes: employee?.notes ?? "",
  });
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState("");

  const set = (field: string, value: string) => setForm((f) => ({ ...f, [field]: value }));

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setSaving(true);
    setError("");
    try {
      const common = {
        firstName: form.firstName,
        lastName: form.lastName,
        email: form.email || undefined,
        phone: form.phone || undefined,
        departmentId: form.departmentId !== NONE_OPTION_VALUE ? Number(form.departmentId) : undefined,
        designation: form.designation || undefined,
        status: Number(form.status),
        employmentType: Number(form.employmentType),
        dateOfJoining: form.dateOfJoining,
        dateOfBirth: form.dateOfBirth || undefined,
        notes: form.notes || undefined,
      };
      if (employee) {
        await onSave(common as UpdateEmployeeRequest);
      } else {
        await onSave({ employeeNumber: form.employeeNumber, ...common } as CreateEmployeeRequest);
      }
    } catch (err) {
      setError(err instanceof Error ? err.message : "An error occurred.");
      setSaving(false);
    }
  };

  return (
    <form onSubmit={handleSubmit} className="space-y-4 max-h-[70vh] overflow-y-auto pr-2">
      <FormError error={error} />

      {!employee && (
        <FormField id="employeeNumber" label="Employee Number">
          <Input id="employeeNumber" required value={form.employeeNumber} onChange={(e) => set("employeeNumber", e.target.value)} />
        </FormField>
      )}

      <FormGrid>
        <FormField id="firstName" label="First Name">
          <Input id="firstName" required value={form.firstName} onChange={(e) => set("firstName", e.target.value)} />
        </FormField>
        <FormField id="lastName" label="Last Name">
          <Input id="lastName" required value={form.lastName} onChange={(e) => set("lastName", e.target.value)} />
        </FormField>
      </FormGrid>

      <FormGrid>
        <FormField id="email" label="Email">
          <Input id="email" type="email" value={form.email} onChange={(e) => set("email", e.target.value)} />
        </FormField>
        <FormField id="phone" label="Phone">
          <Input id="phone" value={form.phone} onChange={(e) => set("phone", e.target.value)} />
        </FormField>
      </FormGrid>

      <FormGrid>
        <FormField id="departmentId" label="Department">
          <Select value={form.departmentId} onValueChange={(v) => set("departmentId", v ?? "")}>
            <SelectTrigger id="departmentId"><SelectValue placeholder="None" /></SelectTrigger>
            <SelectContent>
              <SelectItem value={NONE_OPTION_VALUE}>None</SelectItem>
              {departments.map((d) => (
                <SelectItem key={d.id} value={d.id.toString()}>{d.name}</SelectItem>
              ))}
            </SelectContent>
          </Select>
        </FormField>
        <FormField id="designation" label="Designation">
          <Input id="designation" value={form.designation} onChange={(e) => set("designation", e.target.value)} />
        </FormField>
      </FormGrid>

      <FormGrid>
        <FormField id="status" label="Status">
          <Select value={form.status} onValueChange={(v) => set("status", v ?? "1")}>
            <SelectTrigger id="status"><SelectValue /></SelectTrigger>
            <SelectContent>
              {STATUS_OPTIONS.map((o) => (
                <SelectItem key={o.value} value={o.value}>{o.label}</SelectItem>
              ))}
            </SelectContent>
          </Select>
        </FormField>
        <FormField id="employmentType" label="Employment Type">
          <Select value={form.employmentType} onValueChange={(v) => set("employmentType", v ?? "1")}>
            <SelectTrigger id="employmentType"><SelectValue /></SelectTrigger>
            <SelectContent>
              {TYPE_OPTIONS.map((o) => (
                <SelectItem key={o.value} value={o.value}>{o.label}</SelectItem>
              ))}
            </SelectContent>
          </Select>
        </FormField>
      </FormGrid>

      <FormGrid>
        <FormField id="dateOfJoining" label="Date of Joining">
          <Input id="dateOfJoining" required type="date" value={form.dateOfJoining} onChange={(e) => set("dateOfJoining", e.target.value)} />
        </FormField>
        <FormField id="dateOfBirth" label="Date of Birth">
          <Input id="dateOfBirth" type="date" value={form.dateOfBirth} onChange={(e) => set("dateOfBirth", e.target.value)} />
        </FormField>
      </FormGrid>

      <FormField id="notes" label="Notes">
        <Input id="notes" value={form.notes} onChange={(e) => set("notes", e.target.value)} />
      </FormField>

      <FormActions onCancel={onCancel} saving={saving} saveLabel={employee ? "Update Employee" : "Create Employee"} />
    </form>
  );
}

export function EmployeesPage() {
  const [result, setResult] = useState<PagedResult<Employee> | null>(null);
  const [departments, setDepartments] = useState<Department[]>([]);
  const [loading, setLoading] = useState(true);
  const [columnFilters, setColumnFilters] = useState<ColumnFiltersState>([]);
  const [sorting, setSorting] = useState<SortingState>([]);
  const [page, setPage] = useState(1);
  const [formOpen, setFormOpen] = useState(false);
  const [editingEmployee, setEditingEmployee] = useState<Employee | undefined>(undefined);
  const [deleteTarget, setDeleteTarget] = useState<Employee | null>(null);
  const [deleting, setDeleting] = useState(false);

  useEffect(() => {
    getDepartments({ pageSize: "500" }).then((r) => setDepartments(r.items)).catch((err) => console.warn("Failed to load departments:", err));
  }, []);

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

  const fetchEmployees = useCallback(() => {
    const search = String(columnFilters.find((f) => f.id === "searchTerm")?.value ?? "");
    const statusFilter = String(columnFilters.find((f) => f.id === "status")?.value ?? "");
    const params: Record<string, string> = { page: String(page), pageSize: "20" };
    if (search) params.searchTerm = search;
    if (statusFilter) params.status = statusFilter;
    if (sorting[0]) {
      params.sortBy = sorting[0].id;
      params.sortDescending = String(sorting[0].desc);
    }
    setLoading(true);
    getEmployees(params)
      .then(setResult)
      .finally(() => setLoading(false));
  }, [columnFilters, page, sorting]);

  useEffect(() => { fetchEmployees(); }, [fetchEmployees]);

  const handleSave = async (data: CreateEmployeeRequest | UpdateEmployeeRequest) => {
    if (editingEmployee) {
      await updateEmployee(editingEmployee.id, data as UpdateEmployeeRequest);
    } else {
      await createEmployee(data as CreateEmployeeRequest);
    }
    setFormOpen(false);
    setEditingEmployee(undefined);
    fetchEmployees();
  };

  const handleDelete = async () => {
    if (!deleteTarget) return;
    setDeleting(true);
    try {
      await deleteEmployee(deleteTarget.id);
      setDeleteTarget(null);
      fetchEmployees();
    } finally {
      setDeleting(false);
    }
  };

  const columns: ColumnDef<Employee>[] = [
    {
      accessorKey: "employeeNumber",
      header: "Emp #",
      cell: ({ row }) => <span className="font-medium">{row.original.employeeNumber}</span>,
      meta: {
        filterId: "searchTerm",
        filterComponent: ({ value, onChange }) => (
          <ColumnFilterInput value={value} onChange={onChange} placeholder="Search…" />
        ),
      },
    },
    {
      accessorKey: "fullName",
      header: "Name",
      cell: ({ row }) => row.original.fullName,
    },
    {
      accessorKey: "departmentName",
      header: "Department",
      cell: ({ row }) => <span className="text-muted-foreground">{row.original.departmentName ?? "—"}</span>,
      enableSorting: false,
    },
    {
      accessorKey: "designation",
      header: "Designation",
      cell: ({ row }) => <span className="text-muted-foreground">{row.original.designation ?? "—"}</span>,
      enableSorting: false,
    },
    {
      accessorKey: "status",
      header: "Status",
      cell: ({ row }) => <span className="text-muted-foreground">{row.original.statusName}</span>,
      enableSorting: false,
      meta: {
        filterComponent: ({ value, onChange }) => (
          <ColumnFilterSelect value={value} onChange={onChange} options={STATUS_OPTIONS_FILTER} placeholder="All" />
        ),
      },
    },
    {
      accessorKey: "employmentTypeName",
      header: "Type",
      cell: ({ row }) => <span className="text-muted-foreground">{row.original.employmentTypeName}</span>,
      enableSorting: false,
    },
    {
      id: "actions",
      header: "Actions",
      enableSorting: false,
      meta: { className: "text-right", headerClassName: "text-right" },
      cell: ({ row }) => {
        const e = row.original;
        return (
          <div className="flex items-center justify-end gap-1">
            <Button variant="ghost" size="sm" onClick={() => { setEditingEmployee(e); setFormOpen(true); }} title="Edit">
              <Edit className="h-4 w-4" />
            </Button>
            <Button variant="ghost" size="sm" onClick={() => setDeleteTarget(e)} title="Delete">
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
        title="Employees"
        description="Manage employee records"
        action={
          <Button onClick={() => { setEditingEmployee(undefined); setFormOpen(true); }}>
            <Plus className="mr-2 h-4 w-4" /> Add Employee
          </Button>
        }
      />

      <DataTable
        columns={columns}
        data={result?.items ?? []}
        loading={loading}
        emptyText="No employees found."
        filtering={{ state: columnFilters, onChange: handleFilteringChange, manual: true }}
        sorting={{ state: sorting, onChange: handleSortingChange, manual: true }}
        pagination={result ? { result, onPrevious: () => setPage((p) => p - 1), onNext: () => setPage((p) => p + 1) } : undefined}
      />

      <Dialog open={formOpen} onOpenChange={setFormOpen}>
        <DialogContent className="sm:max-w-lg">
          <DialogHeader><DialogTitle>{editingEmployee ? "Edit Employee" : "Create Employee"}</DialogTitle></DialogHeader>
          <EmployeeForm employee={editingEmployee} departments={departments} onSave={handleSave} onCancel={() => { setFormOpen(false); setEditingEmployee(undefined); }} />
        </DialogContent>
      </Dialog>

      <DeleteDialog
        open={!!deleteTarget}
        onOpenChange={(open) => { if (!open) setDeleteTarget(null); }}
        title="Delete Employee"
        entityLabel={deleteTarget ? `${deleteTarget.fullName} (${deleteTarget.employeeNumber})` : ""}
        onConfirm={handleDelete}
        deleting={deleting}
      />
    </PageLayout>
  );
}
