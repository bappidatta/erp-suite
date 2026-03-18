export interface Category {
  id: number;
  code: string;
  name: string;
  description?: string;
  parentCategoryId?: number;
  isActive: boolean;
  createdAt: string;
  createdBy: string;
  updatedAt?: string;
}

export interface CreateCategoryRequest {
  code: string;
  name: string;
  description?: string;
  parentCategoryId?: number;
  isActive?: boolean;
}

export interface UpdateCategoryRequest {
  name: string;
  description?: string;
  parentCategoryId?: number;
  isActive?: boolean;
}

export interface UnitOfMeasure {
  id: number;
  code: string;
  name: string;
  description?: string;
  isActive: boolean;
  createdAt: string;
  createdBy: string;
  updatedAt?: string;
}

export interface CreateUomRequest {
  code: string;
  name: string;
  description?: string;
  isActive?: boolean;
}

export interface UpdateUomRequest {
  name: string;
  description?: string;
  isActive?: boolean;
}

export interface Warehouse {
  id: number;
  code: string;
  name: string;
  location?: string;
  address?: string;
  contactPerson?: string;
  phone?: string;
  isActive: boolean;
  notes?: string;
  createdAt: string;
  createdBy: string;
  updatedAt?: string;
}

export interface CreateWarehouseRequest {
  code: string;
  name: string;
  location?: string;
  address?: string;
  contactPerson?: string;
  phone?: string;
  isActive?: boolean;
  notes?: string;
}

export interface UpdateWarehouseRequest {
  name: string;
  location?: string;
  address?: string;
  contactPerson?: string;
  phone?: string;
  isActive?: boolean;
  notes?: string;
}

export const ItemType = {
  Product: 1,
  Service: 2,
  RawMaterial: 3,
  SemiFinished: 4,
} as const;

export const ValuationMethod = {
  WeightedAverage: 1,
  FIFO: 2,
  StandardCost: 3,
} as const;

export interface Item {
  id: number;
  code: string;
  name: string;
  description?: string;
  categoryId?: number;
  categoryName?: string;
  uomId: number;
  uomCode: string;
  type: number;
  valuationMethod: number;
  standardCost: number;
  salePrice: number;
  reorderLevel: number;
  barcode?: string;
  isActive: boolean;
  notes?: string;
  imagePath?: string;
  createdAt: string;
  createdBy: string;
  updatedAt?: string;
}

export interface CreateItemRequest {
  code: string;
  name: string;
  description?: string;
  categoryId?: number;
  uomId: number;
  type: number;
  valuationMethod: number;
  standardCost?: number;
  salePrice?: number;
  reorderLevel?: number;
  barcode?: string;
  isActive?: boolean;
  notes?: string;
  imagePath?: string;
}

export interface UpdateItemRequest {
  name: string;
  description?: string;
  categoryId?: number;
  uomId: number;
  type: number;
  valuationMethod: number;
  standardCost?: number;
  salePrice?: number;
  reorderLevel?: number;
  barcode?: string;
  isActive?: boolean;
  notes?: string;
  imagePath?: string;
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
