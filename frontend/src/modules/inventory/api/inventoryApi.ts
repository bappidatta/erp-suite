import { apiFetch } from "@shared/api/client";
import type {
  Category,
  CreateCategoryRequest,
  UpdateCategoryRequest,
  UnitOfMeasure,
  CreateUomRequest,
  UpdateUomRequest,
  Warehouse,
  CreateWarehouseRequest,
  UpdateWarehouseRequest,
  Item,
  CreateItemRequest,
  UpdateItemRequest,
  PagedResult,
} from "../types";

const BASE = "/api/inventory";

// Categories
export const getCategories = (params?: Record<string, string>) => {
  const qs = params ? "?" + new URLSearchParams(params).toString() : "";
  return apiFetch<PagedResult<Category>>(`${BASE}/categories${qs}`);
};

export const getCategoryById = (id: number) =>
  apiFetch<Category>(`${BASE}/categories/${id}`);

export const createCategory = (data: CreateCategoryRequest) =>
  apiFetch<Category>(`${BASE}/categories`, {
    method: "POST",
    body: JSON.stringify(data),
  });

export const updateCategory = (id: number, data: UpdateCategoryRequest) =>
  apiFetch<Category>(`${BASE}/categories/${id}`, {
    method: "PUT",
    body: JSON.stringify(data),
  });

export const deleteCategory = (id: number) =>
  apiFetch<void>(`${BASE}/categories/${id}`, { method: "DELETE" });

export const activateCategory = (id: number) =>
  apiFetch<{ message: string }>(`${BASE}/categories/${id}/activate`, { method: "POST" });

export const deactivateCategory = (id: number) =>
  apiFetch<{ message: string }>(`${BASE}/categories/${id}/deactivate`, { method: "POST" });

// Units of Measure
export const getUoms = (params?: Record<string, string>) => {
  const qs = params ? "?" + new URLSearchParams(params).toString() : "";
  return apiFetch<PagedResult<UnitOfMeasure>>(`${BASE}/uoms${qs}`);
};

export const getUomById = (id: number) =>
  apiFetch<UnitOfMeasure>(`${BASE}/uoms/${id}`);

export const createUom = (data: CreateUomRequest) =>
  apiFetch<UnitOfMeasure>(`${BASE}/uoms`, {
    method: "POST",
    body: JSON.stringify(data),
  });

export const updateUom = (id: number, data: UpdateUomRequest) =>
  apiFetch<UnitOfMeasure>(`${BASE}/uoms/${id}`, {
    method: "PUT",
    body: JSON.stringify(data),
  });

export const deleteUom = (id: number) =>
  apiFetch<void>(`${BASE}/uoms/${id}`, { method: "DELETE" });

export const activateUom = (id: number) =>
  apiFetch<{ message: string }>(`${BASE}/uoms/${id}/activate`, { method: "POST" });

export const deactivateUom = (id: number) =>
  apiFetch<{ message: string }>(`${BASE}/uoms/${id}/deactivate`, { method: "POST" });

// Warehouses
export const getWarehouses = (params?: Record<string, string>) => {
  const qs = params ? "?" + new URLSearchParams(params).toString() : "";
  return apiFetch<PagedResult<Warehouse>>(`${BASE}/warehouses${qs}`);
};

export const getWarehouseById = (id: number) =>
  apiFetch<Warehouse>(`${BASE}/warehouses/${id}`);

export const createWarehouse = (data: CreateWarehouseRequest) =>
  apiFetch<Warehouse>(`${BASE}/warehouses`, {
    method: "POST",
    body: JSON.stringify(data),
  });

export const updateWarehouse = (id: number, data: UpdateWarehouseRequest) =>
  apiFetch<Warehouse>(`${BASE}/warehouses/${id}`, {
    method: "PUT",
    body: JSON.stringify(data),
  });

export const deleteWarehouse = (id: number) =>
  apiFetch<void>(`${BASE}/warehouses/${id}`, { method: "DELETE" });

export const activateWarehouse = (id: number) =>
  apiFetch<{ message: string }>(`${BASE}/warehouses/${id}/activate`, { method: "POST" });

export const deactivateWarehouse = (id: number) =>
  apiFetch<{ message: string }>(`${BASE}/warehouses/${id}/deactivate`, { method: "POST" });

// Items
export const getItems = (params?: Record<string, string>) => {
  const qs = params ? "?" + new URLSearchParams(params).toString() : "";
  return apiFetch<PagedResult<Item>>(`${BASE}/items${qs}`);
};

export const getItemById = (id: number) =>
  apiFetch<Item>(`${BASE}/items/${id}`);

export const createItem = (data: CreateItemRequest) =>
  apiFetch<Item>(`${BASE}/items`, {
    method: "POST",
    body: JSON.stringify(data),
  });

export const updateItem = (id: number, data: UpdateItemRequest) =>
  apiFetch<Item>(`${BASE}/items/${id}`, {
    method: "PUT",
    body: JSON.stringify(data),
  });

export const deleteItem = (id: number) =>
  apiFetch<void>(`${BASE}/items/${id}`, { method: "DELETE" });

export const activateItem = (id: number) =>
  apiFetch<{ message: string }>(`${BASE}/items/${id}/activate`, { method: "POST" });

export const deactivateItem = (id: number) =>
  apiFetch<{ message: string }>(`${BASE}/items/${id}/deactivate`, { method: "POST" });
