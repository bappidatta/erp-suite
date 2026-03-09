export interface User {
  id: number;
  email: string;
  firstName: string;
  lastName: string;
  fullName: string;
  phone?: string;
  status: number;
  statusName: string;
  roleId: number;
  roleName: string;
  departmentId?: number;
  managerId?: number;
  mustChangePassword: boolean;
  lastLoginAt?: string;
  createdAt: string;
  createdBy: string;
  updatedAt?: string;
}

export interface Role {
  id: number;
  name: string;
  description?: string;
  userCount: number;
  permissionCount: number;
  isSystem: boolean;
  createdAt: string;
}

export interface RoleDetail extends Role {
  permissions: Permission[];
}

export interface Permission {
  id: number;
  name: string;
  module: string;
  action: string;
  description?: string;
}

export interface DashboardStats {
  totalUsers: number;
  activeUsers: number;
  totalRoles: number;
  totalPermissions: number;
  lastActivity?: string;
  systemHealth: string;
}

export interface AuditLog {
  id: number;
  userId?: number;
  userName?: string;
  action: string;
  module: string;
  entityId?: string;
  oldValues?: string;
  newValues?: string;
  ipAddress?: string;
  createdAt: string;
}

export interface OrganizationSettings {
  id: number;
  companyName: string;
  legalName?: string;
  registrationNumber?: string;
  address?: string;
  phone?: string;
  email?: string;
  website?: string;
  logoPath?: string;
  currency: string;
  fiscalYearStart?: string;
  dateFormat?: string;
  timeZone?: string;
  updatedAt?: string;
}

export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}

export interface CreateUserRequest {
  email: string;
  password: string;
  firstName: string;
  lastName: string;
  roleId: number;
  phone?: string;
  departmentId?: number;
  managerId?: number;
  mustChangePassword?: boolean;
}

export interface UpdateUserRequest {
  firstName: string;
  lastName: string;
  roleId: number;
  phone?: string;
  departmentId?: number;
  managerId?: number;
  status?: number;
}

export interface CreateRoleRequest {
  name: string;
  description?: string;
}

export interface UpdateRoleRequest {
  name: string;
  description?: string;
}

export interface UpdateOrganizationSettingsRequest {
  companyName: string;
  legalName?: string;
  registrationNumber?: string;
  address?: string;
  phone?: string;
  email?: string;
  website?: string;
  currency?: string;
  fiscalYearStart?: string;
  dateFormat?: string;
  timeZone?: string;
}
