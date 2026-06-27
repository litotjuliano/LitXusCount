import { apiClient } from '../client'

export interface LhdnEInvoiceTypeItem {
  code: string
  description: string
}

export const LHDN_EINVOICE_TYPES: LhdnEInvoiceTypeItem[] = [
  { code: '01', description: 'Invoice' },
  { code: '02', description: 'Credit Note' },
  { code: '03', description: 'Debit Note' },
  { code: '04', description: 'Refund Note' },
  { code: '11', description: 'Self-billed Invoice' },
  { code: '12', description: 'Self-billed Credit Note' },
  { code: '13', description: 'Self-billed Debit Note' },
  { code: '14', description: 'Self-billed Refund Note' },
]

export const lhdnEInvoiceTypesApi = {
  list: () =>
    apiClient.get<LhdnEInvoiceTypeItem[]>('/api/settings/lhdn-einvoice-types').then(r => r.data),
}
