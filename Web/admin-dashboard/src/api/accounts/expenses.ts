import { apiClient } from "../client";
import type { PagedQuery, PagedResult } from "../settings/paging";

export interface AccExpenseItem {
  id: number;
  accAccountId: number;
  accountName: string;
  name: string;
  expenseDate: string;
  amount: number;
  note: string | null;
  isActive: boolean;
}

export interface AccExpenseCreate {
  accAccountId: number;
  name: string;
  expenseDate: string;
  amount: number;
  note: string | null;
}

const base = "/api/accounts/expenses";

export const expensesApi = {
  list: async (query: PagedQuery, accountId?: number): Promise<PagedResult<AccExpenseItem>> =>
    (await apiClient.get<PagedResult<AccExpenseItem>>(base, { params: { ...query, accountId } })).data,
  get: async (id: number): Promise<AccExpenseItem> =>
    (await apiClient.get<AccExpenseItem>(`${base}/${id}`)).data,
  create: async (payload: AccExpenseCreate): Promise<AccExpenseItem> =>
    (await apiClient.post<AccExpenseItem>(base, payload)).data,
  remove: async (id: number): Promise<void> => { await apiClient.delete(`${base}/${id}`); },
};
