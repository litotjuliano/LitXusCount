import { useQuery } from '@tanstack/react-query'
import { lhdnEInvoiceTypesApi } from '../../api/settings/lhdnEInvoiceTypes'

const LhdnEInvoiceTypesLayer = () => {
  const query = useQuery({
    queryKey: ['settings', 'lhdn-einvoice-types'],
    queryFn: lhdnEInvoiceTypesApi.list,
  })

  const items = query.data ?? []

  return (
    <div className='row'>
      <div className='col-12'>
        <div className='card'>
          <div className='card-header d-flex align-items-center justify-content-between'>
            <h5 className='card-title mb-0'>LHDN e-Invoice Types</h5>
          </div>
          <div className='card-body p-0'>
            {query.isLoading && (
              <div className='p-24 text-center'><div className='spinner-border spinner-border-sm' /></div>
            )}
            {query.isError && (
              <div className='p-24 text-danger'>Failed to load e-invoice types.</div>
            )}
            {!query.isLoading && !query.isError && (
              <table className='table table-sm mb-0'>
                <thead>
                  <tr>
                    <th style={{ width: 80 }}>Code</th>
                    <th>Description</th>
                  </tr>
                </thead>
                <tbody>
                  {items.map(item => (
                    <tr key={item.code}>
                      <td><span className='badge bg-light text-dark fw-semibold'>{item.code}</span></td>
                      <td>{item.description}</td>
                    </tr>
                  ))}
                </tbody>
              </table>
            )}
          </div>
        </div>
      </div>
    </div>
  )
}

export default LhdnEInvoiceTypesLayer
