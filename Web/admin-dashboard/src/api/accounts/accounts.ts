import { apiClient } from "../client";
import type { PagedQuery, PagedResult } from "../settings/paging";

export interface AccAccountItem {
  id: number;
  code: string;
  accountName: string;
  accountNumber: string | null;
  description: string | null;
  credit: number;
  debit: number;
  balance: number;
  isActive: boolean;
}

export interface AccAccountUpsert {
  code: string;
  accountName: string;
  accountNumber: string | null;
  description: string | null;
}

const base = "/api/accounts";

export const accountsApi = {
  list: async (query: PagedQuery): Promise<PagedResult<AccAccountItem>> =>
    (await apiClient.get<PagedResult<AccAccountItem>>(base, { params: query })).data,
  listAllActive: async (): Promise<AccAccountItem[]> =>
    (await apiClient.get<AccAccountItem[]>(`${base}/all-active`)).data,
  get: async (id: number): Promise<AccAccountItem> =>
    (await apiClient.get<AccAccountItem>(`${base}/${id}`)).data,
  create: async (payload: AccAccountUpsert): Promise<AccAccountItem> =>
    (await apiClient.post<AccAccountItem>(base, payload)).data,
  edit: async (id: number, payload: AccAccountUpsert): Promise<AccAccountItem> =>
    (await apiClient.put<AccAccountItem>(`${base}/${id}`, payload)).data,
  remove: async (id: number): Promise<void> => { await apiClient.delete(`${base}/${id}`); },
};
