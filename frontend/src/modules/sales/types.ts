export interface Customer {
  id: number;
  code: string;
  name: string;
  contactPerson?: string;
  email?: string;
  phone?: string;
  website?: string;
  taxId?: string;
  addressLine1?: string;
  addressLine2?: string;
  city?: string;
  state?: string;
  postalCode?: string;
  country?: string;
  creditLimit: number;
  currency: string;
  paymentTerms?: string;
  defaultTaxCodeId?: number;
  isActive: boolean;
  notes?: string;
  createdAt: string;
  createdBy: string;
  updatedAt?: string;
}

export interface CreateCustomerRequest {
  code: string;
  name: string;
  contactPerson?: string;
  email?: string;
  phone?: string;
  website?: string;
  taxId?: string;
  addressLine1?: string;
  addressLine2?: string;
  city?: string;
  state?: string;
  postalCode?: string;
  country?: string;
  creditLimit?: number;
  currency?: string;
  paymentTerms?: string;
  defaultTaxCodeId?: number;
  isActive?: boolean;
  notes?: string;
}

export interface UpdateCustomerRequest {
  name: string;
  contactPerson?: string;
  email?: string;
  phone?: string;
  website?: string;
  taxId?: string;
  addressLine1?: string;
  addressLine2?: string;
  city?: string;
  state?: string;
  postalCode?: string;
  country?: string;
  creditLimit?: number;
  currency?: string;
  paymentTerms?: string;
  defaultTaxCodeId?: number;
  isActive?: boolean;
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
