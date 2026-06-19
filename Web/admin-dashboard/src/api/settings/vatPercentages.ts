import { apiClient } from "../client";
import type { PagedQuery, PagedResult } from "./paging";

export interface VatPercentageItem {
  id: number;
  name: string;
  percentage: number;
  isDefault: boolean;
  isActive: boolean;
}

export interface VatPercentageUpsert {
  name: string;
  percentage: number;
}

const base = "/api/settings/vat-percentages";

export const vatPercentagesApi = {
  list: async (query: PagedQuery): Promise<PagedResult<VatPercentageItem>> =>
    (await apiClient.get<PagedResult<VatPercentageItem>>(base, { params: query })).data,
  listAllActive: async (): Promise<VatPercentageItem[]> =>
    (await apiClient.get<VatPercentageItem[]>(`${base}/all-active`)).data,
  create: async (payload: VatPercentageUpsert): Promise<VatPercentageItem> =>
    (await apiClient.post<VatPercentageItem>(base, payload)).data,
  edit: async (id: number, payload: VatPercentageUpsert): Promise<VatPercentageItem> =>
    (await apiClient.put<VatPercentageItem>(`${base}/${id}`, payload)).data,
  remove: async (id: number): Promise<void> => {
    await apiClient.delete(`${base}/${id}`);
  },
};
