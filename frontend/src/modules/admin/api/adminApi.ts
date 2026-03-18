import { apiFetch } from "@shared/api/client";
import type {
  User,
  Role,
  RoleDetail,
  Permission,
  DashboardStats,
  AuditLog,
  OrganizationSettings,
  PagedResult,
  CreateUserRequest,
  UpdateUserRequest,
  CreateRoleRequest,
  UpdateRoleRequest,
  UpdateOrganizationSettingsRequest,
} from "../types";

const ADMIN = "/api/admin";

// Users
export const getUsers = (params?: Record<string, string>) => {
  const qs = params ? "?" + new URLSearchParams(params).toString() : "";
  return apiFetch<PagedResult<User>>(`${ADMIN}/users${qs}`);
};

export const getUserById = (id: number) =>
  apiFetch<User>(`${ADMIN}/users/${id}`);

export const createUser = (data: CreateUserRequest) =>
  apiFetch<User>(`${ADMIN}/users`, {
    method: "POST",
    body: JSON.stringify(data),
  });

export const updateUser = (id: number, data: UpdateUserRequest) =>
  apiFetch<User>(`${ADMIN}/users/${id}`, {
    method: "PUT",
    body: JSON.stringify(data),
  });

export const deleteUser = (id: number) =>
  apiFetch<void>(`${ADMIN}/users/${id}`, { method: "DELETE" });

export const activateUser = (id: number) =>
  apiFetch<{ message: string }>(`${ADMIN}/users/${id}/activate`, { method: "POST" });

export const deactivateUser = (id: number) =>
  apiFetch<{ message: string }>(`${ADMIN}/users/${id}/deactivate`, { method: "POST" });

// Roles
export const getRoles = () => apiFetch<Role[]>(`${ADMIN}/roles`);

export const getRoleById = (id: number) =>
  apiFetch<RoleDetail>(`${ADMIN}/roles/${id}`);

export const createRole = (data: CreateRoleRequest) =>
  apiFetch<Role>(`${ADMIN}/roles`, {
    method: "POST",
    body: JSON.stringify(data),
  });

export const updateRole = (id: number, data: UpdateRoleRequest) =>
  apiFetch<Role>(`${ADMIN}/roles/${id}`, {
    method: "PUT",
    body: JSON.stringify(data),
  });

export const deleteRole = (id: number) =>
  apiFetch<void>(`${ADMIN}/roles/${id}`, { method: "DELETE" });

export const getAllPermissions = () =>
  apiFetch<Permission[]>(`${ADMIN}/roles/permissions`);

export const assignPermissions = (roleId: number, permissionIds: number[]) =>
  apiFetch<{ message: string }>(`${ADMIN}/roles/${roleId}/permissions`, {
    method: "POST",
    body: JSON.stringify({ permissionIds }),
  });

// Organization
export const getOrganizationSettings = () =>
  apiFetch<OrganizationSettings>(`${ADMIN}/organization`);

export const updateOrganizationSettings = (data: UpdateOrganizationSettingsRequest) =>
  apiFetch<OrganizationSettings>(`${ADMIN}/organization`, {
    method: "PUT",
    body: JSON.stringify(data),
  });

export const uploadLogo = (file: File) => {
  const formData = new FormData();
  formData.append("file", file);
  return apiFetch<{ logoPath: string }>(`${ADMIN}/organization/logo`, {
    method: "POST",
    headers: {},
    body: formData,
  });
};

// Dashboard
export const getDashboardStats = () =>
  apiFetch<DashboardStats>(`${ADMIN}/dashboard/stats`);

export const getAuditLogs = (params?: Record<string, string>) => {
  const qs = params ? "?" + new URLSearchParams(params).toString() : "";
  return apiFetch<PagedResult<AuditLog>>(`${ADMIN}/audit-logs${qs}`);
};
