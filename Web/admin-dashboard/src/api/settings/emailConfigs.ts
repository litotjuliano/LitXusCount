import { apiClient } from "../client";
import type { PagedQuery, PagedResult } from "./paging";

/** No password field — the backend never returns it. */
export interface EmailConfigItem {
  id: number;
  email: string;
  hostname: string;
  port: number;
  sslEnabled: boolean;
  senderFullName: string | null;
  isDefault: boolean;
  isActive: boolean;
}

export interface EmailConfigUpsert {
  email: string;
  password: string;
  hostname: string;
  port: number;
  sslEnabled: boolean;
  senderFullName: string | null;
}

const base = "/api/settings/email-configs";

export const emailConfigsApi = {
  list: async (query: PagedQuery): Promise<PagedResult<EmailConfigItem>> =>
    (await apiClient.get<PagedResult<EmailConfigItem>>(base, { params: query })).data,
  listAllActive: async (): Promise<EmailConfigItem[]> =>
    (await apiClient.get<EmailConfigItem[]>(`${base}/all-active`)).data,
  create: async (payload: EmailConfigUpsert): Promise<EmailConfigItem> =>
    (await apiClient.post<EmailConfigItem>(base, payload)).data,
  edit: async (id: number, payload: EmailConfigUpsert): Promise<EmailConfigItem> =>
    (await apiClient.put<EmailConfigItem>(`${base}/${id}`, payload)).data,
  remove: async (id: number): Promise<void> => {
    await apiClient.delete(`${base}/${id}`);
  },
};
