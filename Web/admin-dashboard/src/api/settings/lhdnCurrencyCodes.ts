import { apiClient } from '../client'
import type { PagedQuery, PagedResult } from './paging'

export interface LhdnCurrencyCodeItem {
  id: number
  code: string
  name: string
  isActive: boolean
}

export const lhdnCurrencyCodesApi = {
  list: (q: PagedQuery) =>
    apiClient.get<PagedResult<LhdnCurrencyCodeItem>>('/api/settings/lhdn-currency-codes', { params: q }).then(r => r.data),
  listAllActive: () =>
    apiClient.get<LhdnCurrencyCodeItem[]>('/api/settings/lhdn-currency-codes/all-active').then(r => r.data),
}
