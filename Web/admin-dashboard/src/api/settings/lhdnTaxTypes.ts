import { apiClient } from '../client'
import type { PagedQuery, PagedResult } from './paging'

export interface LhdnTaxTypeItem {
  id: number
  code: string
  description: string
  isActive: boolean
}

export const lhdnTaxTypesApi = {
  list: (q: PagedQuery) =>
    apiClient.get<PagedResult<LhdnTaxTypeItem>>('/api/settings/lhdn-tax-types', { params: q }).then(r => r.data),
  listAllActive: () =>
    apiClient.get<LhdnTaxTypeItem[]>('/api/settings/lhdn-tax-types/all-active').then(r => r.data),
}
