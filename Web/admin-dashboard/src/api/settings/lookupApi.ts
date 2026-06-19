import { apiClient } from "../client";
import type { LookupItem, LookupUpsert } from "./types";
import type { PagedQuery, PagedResult } from "./paging";

/**
 * Five lookup entities (PaymentType, PaymentStatus, CustomerType, Category, UnitOfMeasure)
 * share an identical Name+Description CRUD shape on the backend. This factory avoids
 * writing the same axios calls 5 times.
 */
export function createLookupApi(resourcePath: string) {
  const base = `/api/settings/${resourcePath}`;

  return {
    list: async (query: PagedQuery): Promise<PagedResult<LookupItem>> =>
      (await apiClient.get<PagedResult<LookupItem>>(base, { params: query })).data,
    listAllActive: async (): Promise<LookupItem[]> =>
      (await apiClient.get<LookupItem[]>(`${base}/all-active`)).data,
    create: async (payload: LookupUpsert): Promise<LookupItem> =>
      (await apiClient.post<LookupItem>(base, payload)).data,
    edit: async (id: number, payload: LookupUpsert): Promise<LookupItem> =>
      (await apiClient.put<LookupItem>(`${base}/${id}`, payload)).data,
    remove: async (id: number): Promise<void> => {
      await apiClient.delete(`${base}/${id}`);
    },
  };
}
