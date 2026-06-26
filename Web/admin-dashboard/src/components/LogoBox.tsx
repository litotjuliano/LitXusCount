import { Link } from 'react-router-dom'
import type { LogoBoxProps } from '@/types/component-props'

const LogoBox = ({ containerClassName, height = 36 }: LogoBoxProps) => {
  return (
    <div className={containerClassName ?? ''}>
      <Link to="/" className="logo-dark">
        <img src="/assets/images/litxus-logo.png" height={height} alt="LitXus Account" />
      </Link>
      <Link to="/" className="logo-light">
        <img src="/assets/images/litxus-logo.png" height={height} alt="LitXus Account" />
      </Link>
    </div>
  )
}

export default LogoBox
