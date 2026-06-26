import type { ReactNode } from 'react'
import { Pagination } from 'react-bootstrap'
import IconifyIcon from '@/components/wrappers/IconifyIcon'

export interface PaginatedColumn {
  key: string
  label: string
  sortable?: boolean
}

interface PaginatedTableProps<T> {
  title: string
  columns: PaginatedColumn[]
  items: T[]
  totalCount: number
  page: number
  pageSize: number
  isLoading: boolean
  isError?: boolean
  search: string
  onSearchChange: (value: string) => void
  sortBy?: string
  sortDescending: boolean
  onSortChange: (column: string) => void
  onPageChange: (page: number) => void
  onPageSizeChange: (size: number) => void
  renderRow: (item: T) => ReactNode
  emptyLabel?: string
  headerAction?: ReactNode
}

const PAGE_SIZE_OPTIONS = [10, 20, 50, 100]
const MAX_PAGES_SHOWN = 5

function buildPageNumbers(current: number, total: number): (number | '...')[] {
  if (total <= MAX_PAGES_SHOWN + 2) {
    return Array.from({ length: total }, (_, i) => i + 1)
  }
  const pages: (number | '...')[] = [1]
  const start = Math.max(2, current - 1)
  const end = Math.min(total - 1, current + 1)
  if (start > 2) pages.push('...')
  for (let i = start; i <= end; i++) pages.push(i)
  if (end < total - 1) pages.push('...')
  pages.push(total)
  return pages
}

function PaginatedTable<T>({
  title,
  columns,
  items,
  totalCount,
  page,
  pageSize,
  isLoading,
  isError = false,
  search,
  onSearchChange,
  sortBy,
  sortDescending,
  onSortChange,
  onPageChange,
  onPageSizeChange,
  renderRow,
  emptyLabel = 'No records yet.',
  headerAction,
}: PaginatedTableProps<T>) {
  const totalPages = Math.max(1, Math.ceil(totalCount / pageSize))
  const pageNumbers = buildPageNumbers(page, totalPages)
  const start = totalCount === 0 ? 0 : (page - 1) * pageSize + 1
  const end = Math.min(page * pageSize, totalCount)

  return (
    <div className="card">
      <div className="card-header d-flex flex-wrap align-items-center justify-content-between gap-2">
        <h5 className="card-title mb-0">{title}</h5>
        <div className="d-flex align-items-center gap-2 flex-wrap">
          {headerAction}
          <div className="input-group input-group-sm" style={{ width: 220 }}>
            <span className="input-group-text">
              <IconifyIcon icon="solar:magnifer-outline" />
            </span>
            <input
              type="text"
              className="form-control"
              placeholder="Search…"
              value={search}
              onChange={(e) => onSearchChange(e.target.value)}
            />
          </div>
        </div>
      </div>

      <div className="card-body p-0">
        <div className="table-responsive">
          <table className="table table-hover table-centered mb-0">
            <thead className="bg-light bg-opacity-50">
              <tr>
                {columns.map((col) => (
                  <th
                    key={col.key}
                    className={col.sortable ? 'cursor-pointer user-select-none' : ''}
                    onClick={col.sortable ? () => onSortChange(col.key) : undefined}
                  >
                    <div className="d-flex align-items-center gap-1">
                      {col.label}
                      {col.sortable && (
                        <span className="text-muted" style={{ fontSize: 11 }}>
                          {sortBy === col.key
                            ? sortDescending ? '▼' : '▲'
                            : '⇅'}
                        </span>
                      )}
                    </div>
                  </th>
                ))}
              </tr>
            </thead>
            <tbody>
              {isLoading && (
                <tr>
                  <td colSpan={columns.length} className="text-center py-5">
                    <div className="spinner-border spinner-border-sm text-primary me-2" role="status" />
                    <span className="text-muted">Loading…</span>
                  </td>
                </tr>
              )}
              {!isLoading && isError && (
                <tr>
                  <td colSpan={columns.length} className="text-center py-5">
                    <IconifyIcon icon="solar:danger-triangle-bold" className="fs-2 d-block mx-auto mb-2 text-danger" />
                    <span className="text-danger">Failed to load data — check that the API server is running.</span>
                  </td>
                </tr>
              )}
              {!isLoading && !isError && items.length === 0 && (
                <tr>
                  <td colSpan={columns.length} className="text-center text-muted py-5">
                    <IconifyIcon icon="solar:inbox-line-bold" className="fs-2 d-block mx-auto mb-2 text-muted" />
                    {emptyLabel}
                  </td>
                </tr>
              )}
              {!isLoading && items.map((item) => renderRow(item))}
            </tbody>
          </table>
        </div>

        <div className="d-flex flex-wrap align-items-center justify-content-between gap-3 px-3 py-3 border-top">
          <div className="d-flex align-items-center gap-2">
            <span className="text-muted small">
              {totalCount === 0 ? '0 records' : `Showing ${start}–${end} of ${totalCount}`}
            </span>
            <select
              className="form-select form-select-sm w-auto"
              value={pageSize}
              onChange={(e) => onPageSizeChange(Number(e.target.value))}
            >
              {PAGE_SIZE_OPTIONS.map((size) => (
                <option key={size} value={size}>{size} / page</option>
              ))}
            </select>
          </div>

          <Pagination className="pagination-rounded mb-0">
            <Pagination.Prev
              disabled={page <= 1}
              onClick={() => onPageChange(page - 1)}
            />
            {pageNumbers.map((p, i) =>
              p === '...'
                ? <Pagination.Ellipsis key={`ellipsis-${i}`} disabled />
                : <Pagination.Item
                    key={p}
                    active={p === page}
                    onClick={() => onPageChange(p as number)}
                  >
                    {p}
                  </Pagination.Item>
            )}
            <Pagination.Next
              disabled={page >= totalPages}
              onClick={() => onPageChange(page + 1)}
            />
          </Pagination>
        </div>
      </div>
    </div>
  )
}

export default PaginatedTable
