import { apiFetch } from "@shared/api/client";
import type {
  TaxCode,
  Account,
  AccountTreeNode,
  CreateTaxCodeRequest,
  UpdateTaxCodeRequest,
  CreateAccountRequest,
  UpdateAccountRequest,
  PagedResult,
} from "../types";

const FINANCE = "/api/finance";

// Tax Codes
export const getTaxCodes = (params?: Record<string, string>) => {
  const qs = params ? "?" + new URLSearchParams(params).toString() : "";
  return apiFetch<PagedResult<TaxCode>>(`${FINANCE}/tax-codes${qs}`);
};

export const getTaxCodeById = (id: number) =>
  apiFetch<TaxCode>(`${FINANCE}/tax-codes/${id}`);

export const createTaxCode = (data: CreateTaxCodeRequest) =>
  apiFetch<TaxCode>(`${FINANCE}/tax-codes`, {
    method: "POST",
    body: JSON.stringify(data),
  });

export const updateTaxCode = (id: number, data: UpdateTaxCodeRequest) =>
  apiFetch<TaxCode>(`${FINANCE}/tax-codes/${id}`, {
    method: "PUT",
    body: JSON.stringify(data),
  });

export const deleteTaxCode = (id: number) =>
  apiFetch<void>(`${FINANCE}/tax-codes/${id}`, { method: "DELETE" });

// Accounts (Chart of Accounts)
export const getAccounts = (params?: Record<string, string>) => {
  const qs = params ? "?" + new URLSearchParams(params).toString() : "";
  return apiFetch<PagedResult<Account>>(`${FINANCE}/accounts${qs}`);
};

export const getAccountTree = () =>
  apiFetch<AccountTreeNode[]>(`${FINANCE}/accounts/tree`);

export const getAccountById = (id: number) =>
  apiFetch<Account>(`${FINANCE}/accounts/${id}`);

export const createAccount = (data: CreateAccountRequest) =>
  apiFetch<Account>(`${FINANCE}/accounts`, {
    method: "POST",
    body: JSON.stringify(data),
  });

export const updateAccount = (id: number, data: UpdateAccountRequest) =>
  apiFetch<Account>(`${FINANCE}/accounts/${id}`, {
    method: "PUT",
    body: JSON.stringify(data),
  });

export const deleteAccount = (id: number) =>
  apiFetch<void>(`${FINANCE}/accounts/${id}`, { method: "DELETE" });
