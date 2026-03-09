export interface TaxCode {
  id: number;
  code: string;
  name: string;
  rate: number;
  type: number;
  typeName: string;
  description?: string;
  appliesToSales: boolean;
  appliesToPurchases: boolean;
  isActive: boolean;
  createdAt: string;
  createdBy: string;
  updatedAt?: string;
}

export interface CreateTaxCodeRequest {
  code: string;
  name: string;
  rate: number;
  type: number;
  description?: string;
  appliesToSales?: boolean;
  appliesToPurchases?: boolean;
}

export interface UpdateTaxCodeRequest {
  name: string;
  rate: number;
  type: number;
  description?: string;
  appliesToSales?: boolean;
  appliesToPurchases?: boolean;
}

export interface Account {
  id: number;
  code: string;
  name: string;
  type: number;
  typeName: string;
  description?: string;
  parentId?: number;
  parentName?: string;
  isHeader: boolean;
  level: number;
  isActive: boolean;
  createdAt: string;
  createdBy: string;
  updatedAt?: string;
}

export interface AccountTreeNode {
  id: number;
  code: string;
  name: string;
  type: number;
  typeName: string;
  isHeader: boolean;
  level: number;
  isActive: boolean;
  children: AccountTreeNode[];
}

export interface CreateAccountRequest {
  code: string;
  name: string;
  type: number;
  description?: string;
  parentId?: number;
  isHeader?: boolean;
}

export interface UpdateAccountRequest {
  name: string;
  type: number;
  description?: string;
  parentId?: number;
  isHeader?: boolean;
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
