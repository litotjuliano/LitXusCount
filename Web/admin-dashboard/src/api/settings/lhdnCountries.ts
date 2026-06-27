import { apiClient } from '../client'
import type { PagedQuery, PagedResult } from './paging'

export interface LhdnCountryItem {
  id: number
  code: string
  name: string
  isActive: boolean
}

export const lhdnCountriesApi = {
  list: (q: PagedQuery) =>
    apiClient
      .get<PagedResult<LhdnCountryItem>>('/api/settings/lhdn-countries', { params: q })
      .then((r) => r.data),

  listAllActive: () =>
    apiClient
      .get<LhdnCountryItem[]>('/api/settings/lhdn-countries/all-active')
      .then((r) => r.data),
}
