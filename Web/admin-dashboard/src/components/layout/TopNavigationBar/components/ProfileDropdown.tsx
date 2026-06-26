import { Dropdown, DropdownDivider, DropdownItem, DropdownMenu, DropdownToggle } from 'react-bootstrap'
import { Link } from 'react-router-dom'
import IconifyIcon from '@/components/wrappers/IconifyIcon'
import { useAuthContext } from '@/context/useAuthContext'

const ProfileDropdown = () => {
  const { removeSession, userName } = useAuthContext()
  return (
    <Dropdown className="topbar-item" align="end">
      <DropdownToggle
        as="button"
        type="button"
        className="topbar-button content-none"
        aria-haspopup="true"
        aria-expanded="false">
        <span className="d-flex align-items-center">
          <span className="rounded-circle bg-primary text-white d-flex align-items-center justify-content-center" style={{ width: 32, height: 32, fontSize: 14, fontWeight: 600 }}>
            {userName.charAt(0).toUpperCase()}
          </span>
        </span>
      </DropdownToggle>
      <DropdownMenu>
        <div className="px-3 py-2">
          <h6 className="mb-0 fw-semibold">{userName}</h6>
          <small className="text-muted">Administrator</small>
        </div>
        <DropdownDivider />
        <DropdownItem as={Link} to="/sign-in" className="text-danger" onClick={removeSession}>
          <IconifyIcon icon="bx:log-out" className="fs-18 align-middle me-1" />
          <span className="align-middle">Logout</span>
        </DropdownItem>
      </DropdownMenu>
    </Dropdown>
  )
}

export default ProfileDropdown
