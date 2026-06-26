import type { MenuItemType } from '@/types/menu'

export const findAllParent = (menuItems: MenuItemType[], menuItem: MenuItemType): string[] => {
  let parents: string[] = []
  const parent = findMenuItem(menuItems, menuItem.parentKey)
  if (parent) {
    parents.push(parent.key)
    if (parent.parentKey) parents = [...parents, ...findAllParent(menuItems, parent)]
  }
  return parents
}

export const getMenuItemFromURL = (items: MenuItemType | MenuItemType[], url: string): MenuItemType | undefined => {
  if (Array.isArray(items)) {
    for (const item of items) {
      const found = getMenuItemFromURL(item, url)
      if (found) return found
    }
  } else {
    if (items.url === url) return items
    if (items.children) {
      for (const child of items.children) {
        if (child.url === url) return child
      }
    }
  }
}

export const findMenuItem = (menuItems: MenuItemType[] | undefined, key: MenuItemType['key'] | undefined): MenuItemType | null => {
  if (menuItems && key) {
    for (const item of menuItems) {
      if (item.key === key) return item
      const found = findMenuItem(item.children, key)
      if (found) return found
    }
  }
  return null
}
