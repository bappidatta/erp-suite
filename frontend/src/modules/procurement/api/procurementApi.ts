import { apiFetch } from "@shared/api/client";
import type {
  Vendor,
  CreateVendorRequest,
  UpdateVendorRequest,
  PagedResult,
} from "../types";

const PROCUREMENT = "/api/procurement";

export const getVendors = (params?: Record<string, string>) => {
  const qs = params ? "?" + new URLSearchParams(params).toString() : "";
  return apiFetch<PagedResult<Vendor>>(`${PROCUREMENT}/vendors${qs}`);
};

export const getVendorById = (id: number) =>
  apiFetch<Vendor>(`${PROCUREMENT}/vendors/${id}`);

export const createVendor = (data: CreateVendorRequest) =>
  apiFetch<Vendor>(`${PROCUREMENT}/vendors`, {
    method: "POST",
    body: JSON.stringify(data),
  });

export const updateVendor = (id: number, data: UpdateVendorRequest) =>
  apiFetch<Vendor>(`${PROCUREMENT}/vendors/${id}`, {
    method: "PUT",
    body: JSON.stringify(data),
  });

export const deleteVendor = (id: number) =>
  apiFetch<void>(`${PROCUREMENT}/vendors/${id}`, { method: "DELETE" });

export const activateVendor = (id: number) =>
  apiFetch<{ message: string }>(`${PROCUREMENT}/vendors/${id}/activate`, { method: "POST" });

export const deactivateVendor = (id: number) =>
  apiFetch<{ message: string }>(`${PROCUREMENT}/vendors/${id}/deactivate`, { method: "POST" });
