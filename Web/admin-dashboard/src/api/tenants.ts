import { apiClient } from "./client";
import type { PagedQuery, PagedResult } from "./settings/paging";

export interface TenantItem {
  id: number;
  name: string;
  slug: string;
  contactEmail: string | null;
  notes: string | null;
  isActive: boolean;
  createdAt: string;
}

export interface TenantUpsert {
  name: string;
  slug: string;
  contactEmail: string | null;
  notes: string | null;
  adminEmail?: string | null;
  adminPassword?: string | null;
}

const base = "/api/admin/tenants";

export const tenantsApi = {
  list: async (query: PagedQuery): Promise<PagedResult<TenantItem>> =>
    (await apiClient.get<PagedResult<TenantItem>>(base, { params: query })).data,
  get: async (id: number): Promise<TenantItem> =>
    (await apiClient.get<TenantItem>(`${base}/${id}`)).data,
  create: async (payload: TenantUpsert): Promise<TenantItem> =>
    (await apiClient.post<TenantItem>(base, payload)).data,
  edit: async (id: number, payload: TenantUpsert): Promise<TenantItem> =>
    (await apiClient.put<TenantItem>(`${base}/${id}`, payload)).data,
  toggleActive: async (id: number): Promise<void> => {
    await apiClient.patch(`${base}/${id}/toggle-active`);
  },
};
