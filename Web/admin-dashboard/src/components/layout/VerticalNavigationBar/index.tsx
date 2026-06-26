import { lazy, Suspense } from 'react'
import FallbackLoading from '@/components/FallbackLoading'
import LogoBox from '@/components/LogoBox'
import SimplebarReactClient from '@/components/wrappers/SimplebarReactClient'
import { useAppMenuItems } from '@/hook/useAppMenuItems'
import HoverMenuToggle from './components/HoverMenuToggle'

const AppMenu = lazy(() => import('./components/AppMenu'))

const VerticalNavigationBar = () => {
  const menuItems = useAppMenuItems()

  return (
    <div className="main-nav" id="leftside-menu-container">
      <LogoBox containerClassName="logo-box" height={32} />
      <HoverMenuToggle />
      <SimplebarReactClient className="scrollbar">
        <Suspense fallback={<FallbackLoading />}>
          <AppMenu menuItems={menuItems} />
        </Suspense>
      </SimplebarReactClient>
    </div>
  )
}

export default VerticalNavigationBar
