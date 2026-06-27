import { useLayoutContext } from '@/context/useLayoutContext'
import IconifyIcon from '@/components/wrappers/IconifyIcon'
import LeftSideBarToggle from './components/LeftSideBarToggle'
import ProfileDropdown from './components/ProfileDropdown'
import SearchBox from './components/SearchBox'
import ThemeModeToggle from './components/ThemeModeToggle'
import FullScreenToggler from './components/FullScreenToggler'
import ThemeCustomizer from './components/ThemeCustomizer'

const TopNavigationBar = () => {
  const { themeCustomizer } = useLayoutContext()

  return (
    <header className="topbar">
      <div className="container-fluid">
        <div className="navbar-header">
          <div className="d-flex align-items-center gap-2">
            <LeftSideBarToggle />
            <SearchBox />
          </div>
          <div className="d-flex align-items-center gap-1">
            <ThemeModeToggle />
            <FullScreenToggler />
            <button
              type="button"
              className="nav-link"
              onClick={themeCustomizer.toggle}
              title="UI Settings"
              aria-label="UI Settings">
              <IconifyIcon icon="solar:settings-outline" className="fs-22" />
            </button>
            <ProfileDropdown />
          </div>
        </div>
      </div>
      <ThemeCustomizer />
    </header>
  )
}

export default TopNavigationBar
