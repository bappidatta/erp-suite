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
  JournalEntry,
  CreateJournalEntryRequest,
  UpdateJournalEntryRequest,
  FinancialPeriod,
  CreateFinancialPeriodRequest,
  UpdateFinancialPeriodRequest,
  TrialBalanceRow,
  LedgerEntry,
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

// Journal entries
export const getJournalEntries = (params?: Record<string, string>) => {
  const qs = params ? "?" + new URLSearchParams(params).toString() : "";
  return apiFetch<PagedResult<JournalEntry>>(`${FINANCE}/journal-entries${qs}`);
};

export const getJournalEntryById = (id: number) =>
  apiFetch<JournalEntry>(`${FINANCE}/journal-entries/${id}`);

export const createJournalEntry = (data: CreateJournalEntryRequest) =>
  apiFetch<JournalEntry>(`${FINANCE}/journal-entries`, {
    method: "POST",
    body: JSON.stringify(data),
  });

export const updateJournalEntry = (id: number, data: UpdateJournalEntryRequest) =>
  apiFetch<JournalEntry>(`${FINANCE}/journal-entries/${id}`, {
    method: "PUT",
    body: JSON.stringify(data),
  });

export const deleteJournalEntry = (id: number) =>
  apiFetch<void>(`${FINANCE}/journal-entries/${id}`, { method: "DELETE" });

export const postJournalEntry = (id: number) =>
  apiFetch<JournalEntry>(`${FINANCE}/journal-entries/${id}/post`, { method: "POST" });

// Financial periods
export const getFinancialPeriods = (params?: Record<string, string>) => {
  const qs = params ? "?" + new URLSearchParams(params).toString() : "";
  return apiFetch<PagedResult<FinancialPeriod>>(`${FINANCE}/financial-periods${qs}`);
};

export const createFinancialPeriod = (data: CreateFinancialPeriodRequest) =>
  apiFetch<FinancialPeriod>(`${FINANCE}/financial-periods`, {
    method: "POST",
    body: JSON.stringify(data),
  });

export const updateFinancialPeriod = (id: number, data: UpdateFinancialPeriodRequest) =>
  apiFetch<FinancialPeriod>(`${FINANCE}/financial-periods/${id}`, {
    method: "PUT",
    body: JSON.stringify(data),
  });

export const deleteFinancialPeriod = (id: number) =>
  apiFetch<void>(`${FINANCE}/financial-periods/${id}`, { method: "DELETE" });

export const closeFinancialPeriod = (id: number) =>
  apiFetch<FinancialPeriod>(`${FINANCE}/financial-periods/${id}/close`, { method: "POST" });

export const reopenFinancialPeriod = (id: number) =>
  apiFetch<FinancialPeriod>(`${FINANCE}/financial-periods/${id}/reopen`, { method: "POST" });

// Reports
export const getTrialBalance = (params?: Record<string, string>) => {
  const qs = params ? "?" + new URLSearchParams(params).toString() : "";
  return apiFetch<TrialBalanceRow[]>(`${FINANCE}/trial-balance${qs}`);
};

export const getLedgerEntries = (params: Record<string, string>) =>
  apiFetch<LedgerEntry[]>(`${FINANCE}/ledger?${new URLSearchParams(params).toString()}`);
