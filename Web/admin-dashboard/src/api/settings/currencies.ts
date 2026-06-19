import { apiClient } from "../client";
import type { PagedQuery, PagedResult } from "./paging";

export interface CurrencyItem {
  id: number;
  name: string;
  code: string;
  symbol: string | null;
  country: string | null;
  description: string | null;
  isDefault: boolean;
  isActive: boolean;
}

export interface CurrencyUpsert {
  name: string;
  code: string;
  symbol: string | null;
  country: string | null;
  description: string | null;
}

const base = "/api/settings/currencies";

export const currenciesApi = {
  list: async (query: PagedQuery): Promise<PagedResult<CurrencyItem>> =>
    (await apiClient.get<PagedResult<CurrencyItem>>(base, { params: query })).data,
  listAllActive: async (): Promise<CurrencyItem[]> =>
    (await apiClient.get<CurrencyItem[]>(`${base}/all-active`)).data,
  create: async (payload: CurrencyUpsert): Promise<CurrencyItem> =>
    (await apiClient.post<CurrencyItem>(base, payload)).data,
  edit: async (id: number, payload: CurrencyUpsert): Promise<CurrencyItem> =>
    (await apiClient.put<CurrencyItem>(`${base}/${id}`, payload)).data,
  remove: async (id: number): Promise<void> => {
    await apiClient.delete(`${base}/${id}`);
  },
};
