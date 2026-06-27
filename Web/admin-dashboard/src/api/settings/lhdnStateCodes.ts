import { apiClient } from '../client'
import type { PagedQuery, PagedResult } from './paging'

export interface LhdnStateCodeItem {
  id: number
  code: string
  name: string
  isActive: boolean
}

export const lhdnStateCodesApi = {
  list: (q: PagedQuery) =>
    apiClient.get<PagedResult<LhdnStateCodeItem>>('/api/settings/lhdn-state-codes', { params: q }).then(r => r.data),
  listAllActive: () =>
    apiClient.get<LhdnStateCodeItem[]>('/api/settings/lhdn-state-codes/all-active').then(r => r.data),
}
