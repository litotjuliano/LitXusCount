import { apiClient } from "../client";
import type { PagedQuery, PagedResult } from "./paging";

export interface SupplierItem {
  id: number;
  code: string;
  name: string;
  glAccountId: number;
  glAccountName: string;
  glAccountCode: string;
  address1: string | null;
  address2: string | null;
  address3: string | null;
  addressCode: string | null;
  city: string | null;
  state: string | null;
  country: string | null;
  phone: string | null;
  fax: string | null;
  email: string | null;
  contactPerson: string | null;
  paymentTermsDays: number;
  defaultCurrencyId: number | null;
  defaultCurrencyCode: string | null;
  isActive: boolean;
  tin: string | null;
  registrationType: string | null;
  registrationNumber: string | null;
  sSTRegistrationNumber: string | null;
}

export interface SupplierUpsert {
  code: string;
  name: string;
  glAccountId: number;
  address1: string | null;
  address2: string | null;
  address3: string | null;
  addressCode: string | null;
  city: string | null;
  state: string | null;
  country: string | null;
  phone: string | null;
  fax: string | null;
  email: string | null;
  contactPerson: string | null;
  paymentTermsDays: number;
  defaultCurrencyId: number | null;
  tin: string | null;
  registrationType: string | null;
  registrationNumber: string | null;
  sSTRegistrationNumber: string | null;
}

const base = "/api/settings/suppliers";

export const suppliersApi = {
  list: async (query: PagedQuery): Promise<PagedResult<SupplierItem>> =>
    (await apiClient.get<PagedResult<SupplierItem>>(base, { params: query })).data,
  listAllActive: async (): Promise<SupplierItem[]> =>
    (await apiClient.get<SupplierItem[]>(`${base}/all-active`)).data,
  create: async (payload: SupplierUpsert): Promise<SupplierItem> =>
    (await apiClient.post<SupplierItem>(base, payload)).data,
  edit: async (id: number, payload: SupplierUpsert): Promise<SupplierItem> =>
    (await apiClient.put<SupplierItem>(`${base}/${id}`, payload)).data,
  remove: async (id: number): Promise<void> => { await apiClient.delete(`${base}/${id}`); },
};
