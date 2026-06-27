import { apiClient } from '../client'
import type { PagedQuery, PagedResult } from './paging'

export interface LhdnMsicCodeItem {
  id: number
  code: string
  category: string | null
  description: string
  isActive: boolean
}

export const lhdnMsicCodesApi = {
  list: (q: PagedQuery) =>
    apiClient.get<PagedResult<LhdnMsicCodeItem>>('/api/settings/lhdn-msic-codes', { params: q }).then(r => r.data),
  listAllActive: () =>
    apiClient.get<LhdnMsicCodeItem[]>('/api/settings/lhdn-msic-codes/all-active').then(r => r.data),
}
