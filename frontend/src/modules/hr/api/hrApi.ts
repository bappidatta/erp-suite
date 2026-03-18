import { apiFetch } from "@shared/api/client";
import type {
  Department,
  CreateDepartmentRequest,
  UpdateDepartmentRequest,
  Employee,
  CreateEmployeeRequest,
  UpdateEmployeeRequest,
  PagedResult,
} from "../types";

const BASE = "/api/hr";

// Departments
export const getDepartments = (params?: Record<string, string>) => {
  const qs = params ? "?" + new URLSearchParams(params).toString() : "";
  return apiFetch<PagedResult<Department>>(`${BASE}/departments${qs}`);
};

export const getDepartmentById = (id: number) =>
  apiFetch<Department>(`${BASE}/departments/${id}`);

export const createDepartment = (data: CreateDepartmentRequest) =>
  apiFetch<Department>(`${BASE}/departments`, {
    method: "POST",
    body: JSON.stringify(data),
  });

export const updateDepartment = (id: number, data: UpdateDepartmentRequest) =>
  apiFetch<Department>(`${BASE}/departments/${id}`, {
    method: "PUT",
    body: JSON.stringify(data),
  });

export const deleteDepartment = (id: number) =>
  apiFetch<void>(`${BASE}/departments/${id}`, { method: "DELETE" });

export const activateDepartment = (id: number) =>
  apiFetch<{ message: string }>(`${BASE}/departments/${id}/activate`, { method: "POST" });

export const deactivateDepartment = (id: number) =>
  apiFetch<{ message: string }>(`${BASE}/departments/${id}/deactivate`, { method: "POST" });

// Employees
export const getEmployees = (params?: Record<string, string>) => {
  const qs = params ? "?" + new URLSearchParams(params).toString() : "";
  return apiFetch<PagedResult<Employee>>(`${BASE}/employees${qs}`);
};

export const getEmployeeById = (id: number) =>
  apiFetch<Employee>(`${BASE}/employees/${id}`);

export const createEmployee = (data: CreateEmployeeRequest) =>
  apiFetch<Employee>(`${BASE}/employees`, {
    method: "POST",
    body: JSON.stringify(data),
  });

export const updateEmployee = (id: number, data: UpdateEmployeeRequest) =>
  apiFetch<Employee>(`${BASE}/employees/${id}`, {
    method: "PUT",
    body: JSON.stringify(data),
  });

export const deleteEmployee = (id: number) =>
  apiFetch<void>(`${BASE}/employees/${id}`, { method: "DELETE" });
