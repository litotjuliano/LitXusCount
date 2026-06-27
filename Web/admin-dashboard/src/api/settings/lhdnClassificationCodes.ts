import { apiClient } from '../client'
import type { PagedQuery, PagedResult } from './paging'

export interface LhdnClassificationCodeItem {
  id: number
  code: string
  description: string
  isActive: boolean
}

export const lhdnClassificationCodesApi = {
  list: (q: PagedQuery) =>
    apiClient
      .get<PagedResult<LhdnClassificationCodeItem>>('/api/settings/lhdn-classification-codes', { params: q })
      .then((r) => r.data),

  listAllActive: () =>
    apiClient
      .get<LhdnClassificationCodeItem[]>('/api/settings/lhdn-classification-codes/all-active')
      .then((r) => r.data),
}
