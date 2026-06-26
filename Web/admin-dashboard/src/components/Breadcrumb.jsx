import { Link } from 'react-router-dom'
import IconifyIcon from '@/components/wrappers/IconifyIcon'

// eslint-disable-next-line react/prop-types
const Breadcrumb = ({ title }) => {
  return (
    <div className="d-flex flex-wrap align-items-center justify-content-between gap-3 mb-4">
      <h5 className="fw-semibold mb-0">{title}</h5>
      <nav aria-label="breadcrumb">
        <ol className="breadcrumb mb-0">
          <li className="breadcrumb-item">
            <Link to="/" className="d-flex align-items-center gap-1 text-muted">
              <IconifyIcon icon="solar:home-smile-angle-outline" className="fs-6" />
              Dashboard
            </Link>
          </li>
          <li className="breadcrumb-item active" aria-current="page">{title}</li>
        </ol>
      </nav>
    </div>
  )
}

export default Breadcrumb
