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

export interface JournalEntryLine {
  id: number;
  lineNumber: number;
  accountId: number;
  accountCode: string;
  accountName: string;
  debitAmount: number;
  creditAmount: number;
  description?: string;
}

export interface JournalEntry {
  id: number;
  number: string;
  entryDate: string;
  description: string;
  reference?: string;
  status: number;
  statusName: string;
  postedAt?: string;
  postedBy?: string;
  totalDebit: number;
  totalCredit: number;
  lines: JournalEntryLine[];
  createdAt: string;
  createdBy: string;
  updatedAt?: string;
}

export interface JournalEntryLineRequest {
  lineNumber: number;
  accountId: number;
  debitAmount: number;
  creditAmount: number;
  description?: string;
}

export interface CreateJournalEntryRequest {
  entryDate: string;
  description: string;
  reference?: string;
  lines: JournalEntryLineRequest[];
}

export interface UpdateJournalEntryRequest {
  entryDate: string;
  description: string;
  reference?: string;
  lines: JournalEntryLineRequest[];
}

export interface FinancialPeriod {
  id: number;
  name: string;
  startDate: string;
  endDate: string;
  status: number;
  statusName: string;
  closedAt?: string;
  closedBy?: string;
  createdAt: string;
  createdBy: string;
  updatedAt?: string;
}

export interface CreateFinancialPeriodRequest {
  name: string;
  startDate: string;
  endDate: string;
}

export interface UpdateFinancialPeriodRequest {
  name: string;
  startDate: string;
  endDate: string;
}

export interface TrialBalanceRow {
  accountId: number;
  accountCode: string;
  accountName: string;
  totalDebit: number;
  totalCredit: number;
  netBalance: number;
}

export interface LedgerEntry {
  journalEntryId: number;
  journalNumber: string;
  entryDate: string;
  description: string;
  reference?: string;
  debitAmount: number;
  creditAmount: number;
  runningBalance: number;
}
