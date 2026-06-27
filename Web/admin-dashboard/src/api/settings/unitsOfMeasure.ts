import { apiClient } from '../client'
import type { PagedQuery, PagedResult } from './paging'

export interface UnitOfMeasureItem {
  id: number
  unCefactCode: string | null
  name: string
  isActive: boolean
}

export const unitsOfMeasureApi = {
  list: (q: PagedQuery) =>
    apiClient.get<PagedResult<UnitOfMeasureItem>>('/api/settings/units-of-measure', { params: q }).then(r => r.data),
  listAllActive: () =>
    apiClient.get<UnitOfMeasureItem[]>('/api/settings/units-of-measure/all-active').then(r => r.data),
}
