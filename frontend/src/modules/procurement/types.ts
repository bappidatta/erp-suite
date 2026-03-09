export interface Vendor {
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
  paymentTerms?: string;
  currency: string;
  bankName?: string;
  bankAccountNumber?: string;
  bankRoutingNumber?: string;
  bankSwiftCode?: string;
  defaultTaxCodeId?: number;
  leadTimeDays: number;
  isActive: boolean;
  notes?: string;
  createdAt: string;
  createdBy: string;
  updatedAt?: string;
}

export interface CreateVendorRequest {
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
  paymentTerms?: string;
  currency?: string;
  bankName?: string;
  bankAccountNumber?: string;
  bankRoutingNumber?: string;
  bankSwiftCode?: string;
  defaultTaxCodeId?: number;
  leadTimeDays?: number;
  isActive?: boolean;
  notes?: string;
}

export interface UpdateVendorRequest {
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
  paymentTerms?: string;
  currency?: string;
  bankName?: string;
  bankAccountNumber?: string;
  bankRoutingNumber?: string;
  bankSwiftCode?: string;
  defaultTaxCodeId?: number;
  leadTimeDays?: number;
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
