import { render, screen, fireEvent } from '@testing-library/react'
import { describe, it, expect, vi, beforeEach } from 'vitest'
import TransfersLayer from '../../components/accounts/TransfersLayer'

vi.mock('../../hook/useTransfers', () => ({
  useTransfers: () => ({
    pagedQuery: {
      result: { data: { items: [], totalCount: 0 }, isLoading: false },
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
    accountsQuery: { data: [
      { id: 1, accountName: 'Cash', code: 'CASH', balance: 500 },
      { id: 2, accountName: 'Bank', code: 'BANK', balance: 1000 },
    ]},
    createMutation: { mutate: vi.fn(), isPending: false },
    deleteMutation: { mutate: vi.fn(), isPending: false },
  }),
}))

vi.mock('../../hook/usePermissions', () => ({
  usePermissions: () => ({
    hasPermission: () => true,
  }),
}))

describe('TransfersLayer — inline same-account validation', () => {
  it('shows error when From and To accounts are the same', () => {
    render(<TransfersLayer />)

    const selects = screen.getAllByRole('combobox')
    const fromSelect = selects[0]
    const toSelect = selects[1]

    fireEvent.change(fromSelect, { target: { value: '1' } })
    fireEvent.change(toSelect, { target: { value: '1' } })

    expect(screen.getByText(/cannot transfer to the same account/i)).toBeInTheDocument()
  })

  it('does not show error when From and To accounts differ', () => {
    render(<TransfersLayer />)

    const selects = screen.getAllByRole('combobox')
    const fromSelect = selects[0]
    const toSelect = selects[1]

    fireEvent.change(fromSelect, { target: { value: '1' } })
    fireEvent.change(toSelect, { target: { value: '2' } })

    expect(screen.queryByText(/cannot transfer to the same account/i)).not.toBeInTheDocument()
  })

  it('submit button is disabled when From === To', () => {
    render(<TransfersLayer />)

    const selects = screen.getAllByRole('combobox')
    fireEvent.change(selects[0], { target: { value: '1' } })
    fireEvent.change(selects[1], { target: { value: '1' } })

    const submitBtn = screen.getByRole('button', { name: /save|submit|create/i })
    expect(submitBtn).toBeDisabled()
  })
})
