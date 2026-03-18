export interface Department {
  id: number;
  code: string;
  name: string;
  description?: string;
  parentDepartmentId?: number;
  isActive: boolean;
  createdAt: string;
  createdBy: string;
  updatedAt?: string;
}

export interface CreateDepartmentRequest {
  code: string;
  name: string;
  description?: string;
  parentDepartmentId?: number;
  isActive?: boolean;
}

export interface UpdateDepartmentRequest {
  name: string;
  description?: string;
  parentDepartmentId?: number;
  isActive?: boolean;
}

export const EmploymentStatus = {
  Active: 1,
  OnLeave: 2,
  Terminated: 3,
  Resigned: 4,
} as const;

export const EmploymentType = {
  FullTime: 1,
  PartTime: 2,
  Contract: 3,
  Intern: 4,
} as const;

export interface Employee {
  id: number;
  employeeNumber: string;
  firstName: string;
  lastName: string;
  fullName: string;
  email?: string;
  phone?: string;
  departmentId?: number;
  departmentName?: string;
  designation?: string;
  status: number;
  statusName: string;
  employmentType: number;
  employmentTypeName: string;
  dateOfJoining: string;
  dateOfBirth?: string;
  managerId?: number;
  notes?: string;
  createdAt: string;
  createdBy: string;
  updatedAt?: string;
}

export interface CreateEmployeeRequest {
  employeeNumber: string;
  firstName: string;
  lastName: string;
  email?: string;
  phone?: string;
  departmentId?: number;
  designation?: string;
  status?: number;
  employmentType?: number;
  dateOfJoining: string;
  dateOfBirth?: string;
  managerId?: number;
  notes?: string;
}

export interface UpdateEmployeeRequest {
  firstName: string;
  lastName: string;
  email?: string;
  phone?: string;
  departmentId?: number;
  designation?: string;
  status: number;
  employmentType: number;
  dateOfJoining: string;
  dateOfBirth?: string;
  managerId?: number;
  notes?: string;
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
