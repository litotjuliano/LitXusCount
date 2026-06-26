import { apiClient } from '../client'

export interface SalesPaymentRecordItem {
  id: number
  salesInvoiceId: number
  accAccountId: number
  accAccountName: string
  modeOfPayment: string
  amount: number
  referenceNo: string | null
  isActive: boolean
}

export interface SalesPaymentRecordCreateDto {
  salesInvoiceId: number
  accAccountId: number
  modeOfPayment: string
  amount: number
  referenceNo: string | null
}

export const salesPaymentsApi = {
  getByInvoice: (invoiceId: number) =>
    apiClient.get<SalesPaymentRecordItem[]>('/api/sales/payments', { params: { invoiceId } }).then(r => r.data),

  recordPayment: (dto: SalesPaymentRecordCreateDto) =>
    apiClient.post<SalesPaymentRecordItem>('/api/sales/payments', dto).then(r => r.data),

  deletePayment: (id: number) => apiClient.delete(`/api/sales/payments/${id}`),
}
