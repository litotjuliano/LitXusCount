import { apiClient } from '../client'
import type { PagedQuery, PagedResult } from './paging'

export interface PaymentCodeItem {
  id: number
  code: string
  name: string
  isActive: boolean
}

export const paymentCodesApi = {
  list: (q: PagedQuery) =>
    apiClient.get<PagedResult<PaymentCodeItem>>('/api/settings/payment-codes', { params: q }).then(r => r.data),
  listAllActive: () =>
    apiClient.get<PaymentCodeItem[]>('/api/settings/payment-codes/all-active').then(r => r.data),
}
