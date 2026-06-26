import IconifyIcon from '@/components/wrappers/IconifyIcon'

const StatCard = ({ icon, label, value, colorClass }) => (
  <div className="col-sm-6 col-xl-3">
    <div className="card">
      <div className="card-body">
        <div className="d-flex align-items-center gap-3">
          <div className={`rounded p-2 ${colorClass}`}>
            <IconifyIcon icon={icon} className="fs-3 text-white" />
          </div>
          <div>
            <p className="text-muted small mb-0">{label}</p>
            <h4 className="fw-bold mb-0">{value}</h4>
          </div>
        </div>
      </div>
    </div>
  </div>
)

const DashBoardLayerOne = () => {
  return (
    <>
      <div className="row g-3 mb-4">
        <StatCard icon="solar:wallet-money-bold" label="Total Revenue" value="—" colorClass="bg-primary" />
        <StatCard icon="solar:bill-list-bold" label="Open Invoices" value="—" colorClass="bg-warning" />
        <StatCard icon="solar:box-bold" label="Products" value="—" colorClass="bg-success" />
        <StatCard icon="solar:users-group-rounded-bold" label="Customers" value="—" colorClass="bg-info" />
      </div>
      <div className="row g-3">
        <div className="col-12">
          <div className="card">
            <div className="card-header">
              <h5 className="card-title mb-0">Welcome to LitXusCount</h5>
            </div>
            <div className="card-body">
              <p className="text-muted">
                Use the navigation on the left to manage accounts, sales, inventory, purchasing, and system settings.
              </p>
              <div className="row g-3 mt-1">
                <div className="col-md-4">
                  <div className="border rounded p-3">
                    <h6 className="fw-semibold"><IconifyIcon icon="solar:chart-2-bold" className="me-2 text-primary" />Accounts</h6>
                    <p className="small text-muted mb-0">Track deposits, expenses, and transfers.</p>
                  </div>
                </div>
                <div className="col-md-4">
                  <div className="border rounded p-3">
                    <h6 className="fw-semibold"><IconifyIcon icon="solar:document-bold" className="me-2 text-success" />Sales</h6>
                    <p className="small text-muted mb-0">Create and manage sales invoices.</p>
                  </div>
                </div>
                <div className="col-md-4">
                  <div className="border rounded p-3">
                    <h6 className="fw-semibold"><IconifyIcon icon="solar:settings-bold" className="me-2 text-info" />Settings</h6>
                    <p className="small text-muted mb-0">Configure currencies, VAT, and system preferences.</p>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </>
  )
}

export default DashBoardLayerOne
