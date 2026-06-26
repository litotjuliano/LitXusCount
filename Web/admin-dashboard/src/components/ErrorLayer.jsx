import { Link } from 'react-router-dom'
import IconifyIcon from '@/components/wrappers/IconifyIcon'

const ErrorLayer = () => {
  return (
    <div className="d-flex flex-column align-items-center justify-content-center py-5 text-center">
      <IconifyIcon icon="solar:folder-error-bold" className="text-muted mb-3" style={{ fontSize: 80 }} />
      <h4 className="fw-bold mb-2">Page Not Found</h4>
      <p className="text-muted mb-4">Sorry, the page you are looking for doesn't exist.</p>
      <Link to="/" className="btn btn-primary rounded px-4">Back to Dashboard</Link>
    </div>
  )
}

export default ErrorLayer
