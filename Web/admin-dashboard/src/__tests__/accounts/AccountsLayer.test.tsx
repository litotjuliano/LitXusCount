import { render, screen, fireEvent } from '@testing-library/react'
import { describe, it, expect, vi } from 'vitest'
import AccountsLayer from '../../components/accounts/AccountsLayer'

const mockAccount = {
  id: 1, code: 'CASH', accountName: 'Cash', accountNumber: '001',
  description: 'Default cash', credit: 500, debit: 200, balance: 300, isActive: true,
}

vi.mock('../../hook/useAccounts', () => ({
  useAccounts: () => ({
    pagedQuery: {
      result: { data: { items: [mockAccount], totalCount: 1 }, isLoading: false },
      page: 1,
      pageSize: 10,
      search: '',
      sortBy: undefined,
      sortDescending: false,
      setPage: vi.fn(),
      setSearch: vi.fn(),
      setPageSize: vi.fn(),
      toggleSort: vi.fn(),
    },
    createMutation: { mutate: vi.fn(), isPending: false },
    editMutation: { mutate: vi.fn(), isPending: false },
    deleteMutation: { mutate: vi.fn(), isPending: false },
  }),
}))

vi.mock('../../hook/usePermissions', () => ({
  usePermissions: () => ({
    hasPermission: () => true,
  }),
}))

describe('AccountsLayer — balance fields are read-only in edit mode', () => {
  it('shows Credit, Debit, Balance as non-input elements when editing', () => {
    render(<AccountsLayer />)

    const editBtn = screen.getByRole('button', { name: /edit/i })
    fireEvent.click(editBtn)

    // Balance fields must NOT be editable inputs
    const balanceInput = screen.queryByRole('spinbutton', { name: /balance/i })
    expect(balanceInput).not.toBeInTheDocument()

    const creditInput = screen.queryByRole('spinbutton', { name: /credit/i })
    expect(creditInput).not.toBeInTheDocument()

    const debitInput = screen.queryByRole('spinbutton', { name: /debit/i })
    expect(debitInput).not.toBeInTheDocument()

    // Balance value should appear as read-only text (in both the form info row and the table)
    expect(screen.getAllByText('300.00').length).toBeGreaterThanOrEqual(1)
  })
})
