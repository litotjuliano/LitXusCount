import { unitsOfMeasureApi } from '../../api/settings/unitsOfMeasure'
import { usePaginatedQuery } from '../../hook/usePaginatedQuery'
import PaginatedTable from './PaginatedTable'

const UnitsOfMeasureLayer = () => {
  const pagedQuery = usePaginatedQuery(['settings', 'units-of-measure'], unitsOfMeasureApi.list)

  return (
    <div className='row'>
      <div className='col-12'>
        <PaginatedTable
          title='Unit of Measure'
          columns={[
            { key: 'unCefactCode', label: 'UN/CEFACT Code', sortable: true },
            { key: 'name',         label: 'Description',    sortable: true },
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
              <td><span className='badge bg-light text-dark fw-semibold'>{item.unCefactCode ?? '—'}</span></td>
              <td>{item.name}</td>
            </tr>
          )}
        />
      </div>
    </div>
  )
}

export default UnitsOfMeasureLayer
