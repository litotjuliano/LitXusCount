import { apiClient } from "../client";
import type { PagedQuery, PagedResult } from "./paging";

export interface CustomerItem {
  id: number;
  code: string;
  name: string;
  name2: string | null;
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
  phone2: string | null;
  fax: string | null;
  email: string | null;
  contactPerson: string | null;
  consigneeName: string | null;
  consigneeAddress1: string | null;
  consigneeAddress2: string | null;
  consigneeAddress3: string | null;
  consigneeAddressCode: string | null;
  consigneeCity: string | null;
  consigneeState: string | null;
  consigneeCountry: string | null;
  consigneePhone: string | null;
  paymentTermsDays: number;
  creditLimit: number;
  isLocked: boolean;
  isActive: boolean;
  tin: string | null;
  registrationType: string | null;
  registrationNumber: string | null;
  sSTRegistrationNumber: string | null;
}

export interface CustomerUpsert {
  code: string;
  name: string;
  name2: string | null;
  glAccountId: number;
  address1: string | null;
  address2: string | null;
  address3: string | null;
  addressCode: string | null;
  city: string | null;
  state: string | null;
  country: string | null;
  phone: string | null;
  phone2: string | null;
  fax: string | null;
  email: string | null;
  contactPerson: string | null;
  consigneeName: string | null;
  consigneeAddress1: string | null;
  consigneeAddress2: string | null;
  consigneeAddress3: string | null;
  consigneeAddressCode: string | null;
  consigneeCity: string | null;
  consigneeState: string | null;
  consigneeCountry: string | null;
  consigneePhone: string | null;
  paymentTermsDays: number;
  creditLimit: number;
  isLocked: boolean;
  tin: string | null;
  registrationType: string | null;
  registrationNumber: string | null;
  sSTRegistrationNumber: string | null;
}

const base = "/api/settings/customers";

export const customersApi = {
  list: async (query: PagedQuery): Promise<PagedResult<CustomerItem>> =>
    (await apiClient.get<PagedResult<CustomerItem>>(base, { params: query })).data,
  listAllActive: async (): Promise<CustomerItem[]> =>
    (await apiClient.get<CustomerItem[]>(`${base}/all-active`)).data,
  create: async (payload: CustomerUpsert): Promise<CustomerItem> =>
    (await apiClient.post<CustomerItem>(base, payload)).data,
  edit: async (id: number, payload: CustomerUpsert): Promise<CustomerItem> =>
    (await apiClient.put<CustomerItem>(`${base}/${id}`, payload)).data,
  remove: async (id: number): Promise<void> => { await apiClient.delete(`${base}/${id}`); },
};
