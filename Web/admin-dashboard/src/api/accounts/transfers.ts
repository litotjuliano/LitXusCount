import { apiClient } from "../client";
import type { PagedQuery, PagedResult } from "../settings/paging";

export interface AccTransferItem {
  id: number;
  senderAccountId: number;
  senderAccountName: string;
  receiverAccountId: number;
  receiverAccountName: string;
  transferDate: string;
  amount: number;
  note: string | null;
  isActive: boolean;
}

export interface AccTransferCreate {
  senderAccountId: number;
  receiverAccountId: number;
  transferDate: string;
  amount: number;
  note: string | null;
}

const base = "/api/accounts/transfers";

export const transfersApi = {
  list: async (query: PagedQuery): Promise<PagedResult<AccTransferItem>> =>
    (await apiClient.get<PagedResult<AccTransferItem>>(base, { params: query })).data,
  get: async (id: number): Promise<AccTransferItem> =>
    (await apiClient.get<AccTransferItem>(`${base}/${id}`)).data,
  create: async (payload: AccTransferCreate): Promise<AccTransferItem> =>
    (await apiClient.post<AccTransferItem>(base, payload)).data,
  remove: async (id: number): Promise<void> => { await apiClient.delete(`${base}/${id}`); },
};
