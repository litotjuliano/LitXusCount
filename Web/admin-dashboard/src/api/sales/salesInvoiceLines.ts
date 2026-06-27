import { apiClient } from '../client'

export interface SalesInvoiceLineItem {
  id: number
  salesInvoiceId: number
  productId: number
  itemName: string
  quantity: number
  unitPrice: number
  itemVat: number
  itemVatAmount: number
  itemDiscount: number
  itemDiscountAmount: number
  totalAmount: number
  isReturn: boolean
  isActive: boolean
  classificationCode: string | null
  taxTypeCode: string | null
  unitCode: string | null
}

export interface SalesInvoiceLineCreateDto {
  salesInvoiceId: number
  productId: number
  quantity: number
  unitPrice: number
  itemVAT: number
  itemDiscount: number
  classificationCode?: string | null
  taxTypeCode?: string | null
}

export const salesInvoiceLinesApi = {
  getByInvoice: (invoiceId: number) =>
    apiClient.get<SalesInvoiceLineItem[]>('/api/sales/invoice-lines', { params: { invoiceId } }).then(r => r.data),

  addLine: (dto: SalesInvoiceLineCreateDto) =>
    apiClient.post<SalesInvoiceLineItem>('/api/sales/invoice-lines', dto).then(r => r.data),

  removeLine: (id: number) => apiClient.delete(`/api/sales/invoice-lines/${id}`),
}
