import LeftSideBarToggle from './components/LeftSideBarToggle'
import ProfileDropdown from './components/ProfileDropdown'
import SearchBox from './components/SearchBox'
import ThemeModeToggle from './components/ThemeModeToggle'
import FullScreenToggler from './components/FullScreenToggler'

const TopNavigationBar = () => {
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
            <ProfileDropdown />
          </div>
        </div>
      </div>
    </header>
  )
}

export default TopNavigationBar
