import { apiClient } from "../client";
import type { PagedQuery, PagedResult } from "./paging";

export const AccountTypeLabels: Record<number, string> = {
  1: "Asset", 2: "Liability", 3: "Equity", 4: "Revenue", 5: "Expense", 6: "COGS",
};

export interface GlAccountItem {
  id: number;
  code: string;
  name: string;
  accountType: number;
  parentId: number | null;
  parentName: string | null;
  isControl: boolean;
  openingBalance: number;
  isActive: boolean;
}

export interface GlAccountUpsert {
  code: string;
  name: string;
  accountType: number;
  parentId: number | null;
  isControl: boolean;
  openingBalance: number;
}

const base = "/api/settings/gl-accounts";

export const glAccountsApi = {
  list: async (query: PagedQuery): Promise<PagedResult<GlAccountItem>> =>
    (await apiClient.get<PagedResult<GlAccountItem>>(base, { params: query })).data,
  listAllActive: async (): Promise<GlAccountItem[]> =>
    (await apiClient.get<GlAccountItem[]>(`${base}/all-active`)).data,
  create: async (payload: GlAccountUpsert): Promise<GlAccountItem> =>
    (await apiClient.post<GlAccountItem>(base, payload)).data,
  edit: async (id: number, payload: GlAccountUpsert): Promise<GlAccountItem> =>
    (await apiClient.put<GlAccountItem>(`${base}/${id}`, payload)).data,
  remove: async (id: number): Promise<void> => { await apiClient.delete(`${base}/${id}`); },
};
