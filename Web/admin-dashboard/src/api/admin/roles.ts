import { apiClient } from "../client";
import type { PagedQuery, PagedResult } from "../settings/paging";

export interface RoleListItem {
  id: string;
  name: string;
  isProtected: boolean;
}

export interface RoleDetail {
  id: string;
  name: string;
  isProtected: boolean;
  permissions: string[];
}

export interface RoleUpsertRequest {
  name: string;
  permissions: string[];
}

const base = "/api/admin/roles";

export const rolesApi = {
  list: async (query: PagedQuery): Promise<PagedResult<RoleListItem>> =>
    (await apiClient.get<PagedResult<RoleListItem>>(base, { params: query })).data,
  get: async (id: string): Promise<RoleDetail> => (await apiClient.get<RoleDetail>(`${base}/${id}`)).data,
  permissionsCatalog: async (): Promise<string[]> => (await apiClient.get<string[]>(`${base}/permissions-catalog`)).data,
  create: async (payload: RoleUpsertRequest): Promise<RoleDetail> => (await apiClient.post<RoleDetail>(base, payload)).data,
  edit: async (id: string, payload: RoleUpsertRequest): Promise<RoleDetail> =>
    (await apiClient.put<RoleDetail>(`${base}/${id}`, payload)).data,
  remove: async (id: string): Promise<void> => {
    await apiClient.delete(`${base}/${id}`);
  },
};
