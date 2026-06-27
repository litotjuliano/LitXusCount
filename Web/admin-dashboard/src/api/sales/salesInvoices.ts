import { apiClient } from '../client'

export type SalesInvoiceCategory = 1 | 2 | 3 | 4 // Regular=1, Draft=2, Quote=3, Manual=4
export type SalesPaymentStatus = 0 | 1 | 2 // Unpaid=0, PartiallyPaid=1, Paid=2

export interface SalesInvoiceItem {
  id: number
  invoiceNo: string
  category: SalesInvoiceCategory
  customerId: number
  customerName: string
  currencyId: number | null
  currencyCode: string | null
  subTotal: number
  discountAmount: number
  vatAmount: number
  grandTotal: number
  paidAmount: number
  dueAmount: number
  paymentStatus: SalesPaymentStatus
  notes: string | null
  isActive: boolean
  createdAt: string
  invoiceTypeCode: string
}

export interface SalesInvoiceCreateDto {
  customerId: number
  currencyId: number | null
}

export interface SalesInvoiceHeaderUpdateDto {
  customerId: number
  currencyId: number | null
  notes: string | null
  invoiceTypeCode: string | null
}

export interface PagedResult<T> {
  items: T[]
  totalCount: number
  page: number
  pageSize: number
}

export const salesInvoicesApi = {
  getPaged: (params: {
    page: number
    pageSize: number
    search?: string
    sortBy?: string
    sortDescending?: boolean
    category?: SalesInvoiceCategory | null
  }) => apiClient.get<PagedResult<SalesInvoiceItem>>('/api/sales/invoices', { params }).then(r => r.data),

  getById: (id: number) =>
    apiClient.get<SalesInvoiceItem>(`/api/sales/invoices/${id}`).then(r => r.data),

  createDraft: (dto: SalesInvoiceCreateDto) =>
    apiClient.post<SalesInvoiceItem>('/api/sales/invoices', dto).then(r => r.data),

  updateHeader: (id: number, dto: SalesInvoiceHeaderUpdateDto) =>
    apiClient.put<SalesInvoiceItem>(`/api/sales/invoices/${id}`, dto).then(r => r.data),

  promote: (id: number, targetCategory: SalesInvoiceCategory) =>
    apiClient.patch<SalesInvoiceItem>(`/api/sales/invoices/${id}/promote`, null, {
      params: { targetCategory },
    }).then(r => r.data),

  delete: (id: number) => apiClient.delete(`/api/sales/invoices/${id}`),
}
