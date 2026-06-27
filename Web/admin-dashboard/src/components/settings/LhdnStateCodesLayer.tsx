import { lhdnStateCodesApi } from '../../api/settings/lhdnStateCodes'
import { usePaginatedQuery } from '../../hook/usePaginatedQuery'
import PaginatedTable from './PaginatedTable'

const LhdnStateCodesLayer = () => {
  const pagedQuery = usePaginatedQuery(['settings', 'lhdn-state-codes'], lhdnStateCodesApi.list)

  return (
    <div className='row'>
      <div className='col-12'>
        <PaginatedTable
          title='LHDN State Codes'
          columns={[
            { key: 'code', label: 'Code', sortable: true },
            { key: 'name', label: 'State / Territory', sortable: true },
          ]}
          items={pagedQuery.result.data?.items ?? []}
          totalCount={pagedQuery.result.data?.totalCount ?? 0}
          page={pagedQuery.page}
          pageSize={pagedQuery.pageSize}
          isLoading={pagedQuery.result.isLoading}
          isError={pagedQuery.result.isError}
          search={pagedQuery.search}
          onSearchChange={pagedQuery.setSearch}
          sortBy={pagedQuery.sortBy}
          sortDescending={pagedQuery.sortDescending}
          onSortChange={pagedQuery.toggleSort}
          onPageChange={pagedQuery.setPage}
          onPageSizeChange={pagedQuery.setPageSize}
          renderRow={(item) => (
            <tr key={item.id}>
              <td><span className='badge bg-light text-dark fw-semibold'>{item.code}</span></td>
              <td>{item.name}</td>
            </tr>
          )}
        />
      </div>
    </div>
  )
}

export default LhdnStateCodesLayer
