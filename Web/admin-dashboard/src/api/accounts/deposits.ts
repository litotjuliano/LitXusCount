import { apiClient } from "../client";
import type { PagedQuery, PagedResult } from "../settings/paging";

export interface AccDepositItem {
  id: number;
  accAccountId: number;
  accountName: string;
  depositDate: string;
  amount: number;
  note: string | null;
  isActive: boolean;
}

export interface AccDepositCreate {
  accAccountId: number;
  depositDate: string;
  amount: number;
  note: string | null;
}

const base = "/api/accounts/deposits";

export const depositsApi = {
  list: async (query: PagedQuery, accountId?: number): Promise<PagedResult<AccDepositItem>> =>
    (await apiClient.get<PagedResult<AccDepositItem>>(base, { params: { ...query, accountId } })).data,
  get: async (id: number): Promise<AccDepositItem> =>
    (await apiClient.get<AccDepositItem>(`${base}/${id}`)).data,
  create: async (payload: AccDepositCreate): Promise<AccDepositItem> =>
    (await apiClient.post<AccDepositItem>(base, payload)).data,
  remove: async (id: number): Promise<void> => { await apiClient.delete(`${base}/${id}`); },
};
