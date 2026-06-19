import { apiClient } from "../client";
import type { PagedQuery, PagedResult } from "../settings/paging";

export interface UserItem {
  id: string;
  email: string;
  displayName: string | null;
  roles: string[];
  isActive: boolean;
}

export interface UserCreateRequest {
  email: string;
  displayName: string;
  password: string;
  roles: string[];
}

export interface UserEditRequest {
  email: string;
  displayName: string;
  roles: string[];
}

const base = "/api/admin/users";

export const usersApi = {
  list: async (query: PagedQuery, includeInactive: boolean): Promise<PagedResult<UserItem>> =>
    (await apiClient.get<PagedResult<UserItem>>(base, { params: { ...query, includeInactive } })).data,
  create: async (payload: UserCreateRequest): Promise<UserItem> => (await apiClient.post<UserItem>(base, payload)).data,
  edit: async (id: string, payload: UserEditRequest): Promise<UserItem> =>
    (await apiClient.put<UserItem>(`${base}/${id}`, payload)).data,
  deactivate: async (id: string): Promise<void> => {
    await apiClient.post(`${base}/${id}/deactivate`);
  },
  reactivate: async (id: string): Promise<void> => {
    await apiClient.post(`${base}/${id}/reactivate`);
  },
  resetPassword: async (id: string, newPassword: string): Promise<void> => {
    await apiClient.post(`${base}/${id}/reset-password`, { newPassword });
  },
};
