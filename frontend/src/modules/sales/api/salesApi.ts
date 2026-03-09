import { apiFetch } from "@shared/api/client";
import type {
  Customer,
  CreateCustomerRequest,
  UpdateCustomerRequest,
  PagedResult,
} from "../types";

const SALES = "/api/sales";

export const getCustomers = (params?: Record<string, string>) => {
  const qs = params ? "?" + new URLSearchParams(params).toString() : "";
  return apiFetch<PagedResult<Customer>>(`${SALES}/customers${qs}`);
};

export const getCustomerById = (id: number) =>
  apiFetch<Customer>(`${SALES}/customers/${id}`);

export const createCustomer = (data: CreateCustomerRequest) =>
  apiFetch<Customer>(`${SALES}/customers`, {
    method: "POST",
    body: JSON.stringify(data),
  });

export const updateCustomer = (id: number, data: UpdateCustomerRequest) =>
  apiFetch<Customer>(`${SALES}/customers/${id}`, {
    method: "PUT",
    body: JSON.stringify(data),
  });

export const deleteCustomer = (id: number) =>
  apiFetch<void>(`${SALES}/customers/${id}`, { method: "DELETE" });

export const activateCustomer = (id: number) =>
  apiFetch<{ message: string }>(`${SALES}/customers/${id}/activate`, { method: "POST" });

export const deactivateCustomer = (id: number) =>
  apiFetch<{ message: string }>(`${SALES}/customers/${id}/deactivate`, { method: "POST" });
