import type { ReactNode } from "react";

export interface PaginatedColumn {
  key: string;
  label: string;
  sortable?: boolean;
}

interface PaginatedTableProps<T> {
  title: string;
  columns: PaginatedColumn[];
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
  isLoading: boolean;
  search: string;
  onSearchChange: (value: string) => void;
  sortBy?: string;
  sortDescending: boolean;
  onSortChange: (column: string) => void;
  onPageChange: (page: number) => void;
  onPageSizeChange: (size: number) => void;
  renderRow: (item: T) => ReactNode;
  emptyLabel?: string;
}

const PAGE_SIZE_OPTIONS = [10, 20, 50, 100];

function PaginatedTable<T>({
  title,
  columns,
  items,
  totalCount,
  page,
  pageSize,
  isLoading,
  search,
  onSearchChange,
  sortBy,
  sortDescending,
  onSortChange,
  onPageChange,
  onPageSizeChange,
  renderRow,
  emptyLabel = "No records yet.",
}: PaginatedTableProps<T>) {
  const totalPages = Math.max(1, Math.ceil(totalCount / pageSize));

  return (
    <div className='card basic-data-table'>
      <div className='card-header d-flex flex-wrap align-items-center justify-content-between gap-3'>
        <h6 className='card-title mb-0'>{title}</h6>
        <input
          type='text'
          className='form-control w-auto'
          placeholder='Search...'
          value={search}
          onChange={(e) => onSearchChange(e.target.value)}
        />
      </div>
      <div className='card-body'>
        <table className='table bordered-table mb-0'>
          <thead>
            <tr>
              {columns.map((col) => (
                <th
                  key={col.key}
                  onClick={col.sortable ? () => onSortChange(col.key) : undefined}
                  style={col.sortable ? { cursor: "pointer", userSelect: "none" } : undefined}
                >
                  {col.label}
                  {col.sortable && sortBy === col.key && (sortDescending ? " ▼" : " ▲")}
                </th>
              ))}
            </tr>
          </thead>
          <tbody>
            {isLoading && (
              <tr>
                <td colSpan={columns.length}>Loading...</td>
              </tr>
            )}
            {!isLoading && items.length === 0 && (
              <tr>
                <td colSpan={columns.length} className='text-center text-secondary-light'>
                  {emptyLabel}
                </td>
              </tr>
            )}
            {!isLoading && items.map((item) => renderRow(item))}
          </tbody>
        </table>
        <div className='d-flex flex-wrap align-items-center justify-content-between gap-3 mt-16'>
          <div className='d-flex align-items-center gap-2'>
            <span className='text-secondary-light'>Rows per page</span>
            <select
              className='form-select form-select-sm w-auto'
              value={pageSize}
              onChange={(e) => onPageSizeChange(Number(e.target.value))}
            >
              {PAGE_SIZE_OPTIONS.map((size) => (
                <option key={size} value={size}>
                  {size}
                </option>
              ))}
            </select>
          </div>
          <div className='d-flex align-items-center gap-2'>
            <button
              type='button'
              className='btn btn-outline-secondary btn-sm'
              disabled={page <= 1}
              onClick={() => onPageChange(page - 1)}
            >
              Previous
            </button>
            <span className='text-secondary-light'>
              Page {page} of {totalPages} ({totalCount} total)
            </span>
            <button
              type='button'
              className='btn btn-outline-secondary btn-sm'
              disabled={page >= totalPages}
              onClick={() => onPageChange(page + 1)}
            >
              Next
            </button>
          </div>
        </div>
      </div>
    </div>
  );
}

export default PaginatedTable;
