import clsx from 'clsx'
import { Fragment, useCallback, useEffect, useState, type MouseEvent } from 'react'
import { Collapse } from 'react-bootstrap'
import { Link, useLocation } from 'react-router-dom'

import IconifyIcon from '@/components/wrappers/IconifyIcon'
import { findAllParent, findMenuItem, getMenuItemFromURL } from '@/helpers/menu'
import type { MenuItemType, SubMenus } from '@/types/menu'

const MenuItemWithChildren = ({ item, className, linkClassName, subMenuClassName, activeMenuItems, toggleMenu }: SubMenus) => {
  const [open, setOpen] = useState<boolean>(activeMenuItems!.includes(item.key))

  useEffect(() => {
    setOpen(activeMenuItems!.includes(item.key))
  }, [activeMenuItems, item])

  const toggleMenuItem = (e: MouseEvent<HTMLDivElement>) => {
    e.preventDefault()
    const status = !open
    setOpen(status)
    if (toggleMenu) toggleMenu(item, status)
  }

  const getActiveClass = useCallback(
    (i: MenuItemType) => (activeMenuItems?.includes(i.key) ? 'active' : ''),
    [activeMenuItems],
  )

  return (
    <li className={className}>
      <div onClick={toggleMenuItem} aria-expanded={open} role="button" className={clsx(linkClassName)}>
        {item.icon && <span className="nav-icon"><IconifyIcon icon={item.icon} /></span>}
        <span className="nav-text">{item.label}</span>
        {!item.badge ? (
          <IconifyIcon icon="bx:chevron-down" className="menu-arrow" />
        ) : (
          <span className={`badge badge-pill text-end bg-${item.badge.variant}`}>{item.badge.text}</span>
        )}
      </div>
      <Collapse in={open}>
        <div>
          <ul className={clsx(subMenuClassName)}>
            {(item.children || []).map((child, idx) => (
              <Fragment key={child.key + idx}>
                {child.children ? (
                  <MenuItemWithChildren
                    item={child}
                    linkClassName={clsx('nav-link', getActiveClass(child))}
                    activeMenuItems={activeMenuItems}
                    className="sub-nav-item"
                    subMenuClassName="nav sub-navbar-nav"
                    toggleMenu={toggleMenu}
                  />
                ) : (
                  <MenuItem item={child} className="sub-nav-item" linkClassName={clsx('sub-nav-link', getActiveClass(child))} />
                )}
              </Fragment>
            ))}
          </ul>
        </div>
      </Collapse>
    </li>
  )
}

const MenuItem = ({ item, className, linkClassName }: SubMenus) => (
  <li className={className}>
    <Link to={item.url ?? ''} target={item.target} className={clsx(linkClassName, { disabled: item.isDisabled })}>
      {item.icon && <span className="nav-icon"><IconifyIcon icon={item.icon} /></span>}
      <span className="nav-text">{item.label}</span>
      {item.badge && <span className={`badge badge-pill text-end bg-${item.badge.variant}`}>{item.badge.text}</span>}
    </Link>
  </li>
)

type AppMenuProps = { menuItems: Array<MenuItemType> }

const AppMenu = ({ menuItems }: AppMenuProps) => {
  const { pathname } = useLocation()
  const [activeMenuItems, setActiveMenuItems] = useState<Array<string>>([])

  const toggleMenu = (menuItem: MenuItemType, show: boolean) => {
    if (show) setActiveMenuItems([menuItem.key, ...findAllParent(menuItems, menuItem)])
  }

  const getActiveClass = useCallback(
    (item: MenuItemType) => (activeMenuItems?.includes(item.key) ? 'active' : ''),
    [activeMenuItems],
  )

  const activeMenu = useCallback(() => {
    const matchingMenuItem = getMenuItemFromURL(menuItems, pathname)
    if (matchingMenuItem) {
      const activeMt = findMenuItem(menuItems, matchingMenuItem.key)
      if (activeMt) setActiveMenuItems([activeMt.key, ...findAllParent(menuItems, activeMt)])

      setTimeout(() => {
        const activatedItem = document.querySelector<HTMLAnchorElement>(
          `#leftside-menu-container .simplebar-content a[href="${pathname}"]`
        )
        if (activatedItem) {
          const simplebarContent = document.querySelector('#leftside-menu-container .simplebar-content-wrapper')
          if (simplebarContent) {
            const offset = activatedItem.offsetTop - window.innerHeight * 0.4
            simplebarContent.scrollTop = Math.max(0, offset)
          }
        }
      }, 400)
    }
  }, [pathname, menuItems])

  useEffect(() => {
    if (menuItems && menuItems.length > 0) activeMenu()
  }, [activeMenu, menuItems])

  return (
    <ul className="navbar-nav">
      {(menuItems || []).map((item, idx) => (
        <Fragment key={item.key + idx}>
          {item.isTitle ? (
            <li className="menu-title">{item.label}</li>
          ) : (
            <>
              {item.children ? (
                <MenuItemWithChildren
                  item={item}
                  toggleMenu={toggleMenu}
                  className="nav-item"
                  linkClassName={clsx('nav-link', getActiveClass(item))}
                  subMenuClassName="nav sub-navbar-nav"
                  activeMenuItems={activeMenuItems}
                />
              ) : (
                <MenuItem item={item} linkClassName={clsx('nav-link', getActiveClass(item))} className="nav-item" />
              )}
            </>
          )}
        </Fragment>
      ))}
    </ul>
  )
}

export default AppMenu
