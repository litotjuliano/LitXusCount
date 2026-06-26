import { useMemo } from 'react'
import { usePermissions } from './usePermissions'
import { useRole } from './useRole'
import { Permissions } from '../api/permissions'
import type { MenuItemType } from '../types/menu'

export function useAppMenuItems(): MenuItemType[] {
  const { hasPermission } = usePermissions()
  const { isSuperAdmin } = useRole()

  return useMemo(() => {
    const items: MenuItemType[] = []

    items.push({ key: 'dashboard', icon: 'solar:home-smile-angle-outline', label: 'Dashboard', url: '/' })

    // Modules
    items.push({ key: 'modules-title', label: 'MODULES', isTitle: true })
    items.push({ key: 'inventory', icon: 'solar:box-outline', label: 'Inventory', url: '/inventory' })
    items.push({ key: 'sales-module', icon: 'solar:cart-large-outline', label: 'Sales', url: '/sales' })
    items.push({ key: 'purchasing', icon: 'solar:bag-outline', label: 'Purchasing', url: '/purchasing' })
    items.push({ key: 'finance', icon: 'solar:wallet-2-outline', label: 'Finance', url: '/finance' })

    // User Management
    const usersVisible = hasPermission(Permissions.Users.View)
    const rolesVisible = hasPermission(Permissions.Roles.View)
    if (usersVisible || rolesVisible) {
      const children: MenuItemType[] = []
      if (usersVisible) children.push({ key: 'admin-users', label: 'Users', url: '/admin/users', parentKey: 'admin' })
      if (rolesVisible) children.push({ key: 'admin-roles', label: 'Roles', url: '/admin/roles', parentKey: 'admin' })
      items.push({ key: 'admin', icon: 'solar:shield-user-outline', label: 'User Management', children })
    }

    // Platform (SuperAdmin only)
    if (isSuperAdmin && hasPermission(Permissions.Tenants.View)) {
      items.push({
        key: 'platform',
        icon: 'solar:server-square-outline',
        label: 'Platform',
        children: [
          { key: 'admin-tenants', label: 'Tenants', url: '/admin/tenants', parentKey: 'platform' },
        ],
      })
    }

    // Accounts
    const accV = hasPermission(Permissions.Accounts.Account.View)
    const depV = hasPermission(Permissions.Accounts.Deposit.View)
    const expV = hasPermission(Permissions.Accounts.Expense.View)
    const traV = hasPermission(Permissions.Accounts.Transfer.View)
    if (accV || depV || expV || traV) {
      items.push({ key: 'accounts-title', label: 'ACCOUNTS', isTitle: true })
      const children: MenuItemType[] = []
      if (accV) children.push({ key: 'acc-accounts', label: 'Accounts', url: '/accounts', parentKey: 'accounts' })
      if (depV) children.push({ key: 'acc-deposits', label: 'Deposits', url: '/accounts/deposits', parentKey: 'accounts' })
      if (expV) children.push({ key: 'acc-expenses', label: 'Expenses', url: '/accounts/expenses', parentKey: 'accounts' })
      if (traV) children.push({ key: 'acc-transfers', label: 'Transfers', url: '/accounts/transfers', parentKey: 'accounts' })
      items.push({ key: 'accounts', icon: 'solar:wallet-money-outline', label: 'Accounts', children })
    }

    // Sales
    if (hasPermission(Permissions.Sales.Invoice.View)) {
      items.push({ key: 'sales-title', label: 'SALES', isTitle: true })
      items.push({
        key: 'sales-group',
        icon: 'solar:document-text-outline',
        label: 'Sales',
        children: [{ key: 'sales-invoices', label: 'Invoices', url: '/sales/invoices', parentKey: 'sales-group' }],
      })
    }

    // Master Data
    const glV = hasPermission(Permissions.Settings.GlAccount.View)
    const custV = hasPermission(Permissions.Settings.Customer.View)
    const suppV = hasPermission(Permissions.Settings.Supplier.View)
    const prodV = hasPermission(Permissions.Settings.Product.View)
    if (glV || custV || suppV || prodV) {
      items.push({ key: 'masterdata-title', label: 'MASTER DATA', isTitle: true })
      const children: MenuItemType[] = []
      if (glV) children.push({ key: 'gl-accounts', label: 'GL Accounts', url: '/settings/gl-accounts', parentKey: 'master-data' })
      if (custV) children.push({ key: 'md-customers', label: 'Customers', url: '/settings/customers', parentKey: 'master-data' })
      if (suppV) children.push({ key: 'md-suppliers', label: 'Suppliers', url: '/settings/suppliers', parentKey: 'master-data' })
      if (prodV) children.push({ key: 'md-products', label: 'Products', url: '/settings/products', parentKey: 'master-data' })
      items.push({ key: 'master-data', icon: 'solar:database-outline', label: 'Master Data', children })
    }

    // System Settings
    items.push({ key: 'settings-title', label: 'SYSTEM SETTINGS', isTitle: true })
    const settingsChildren: MenuItemType[] = []
    if (hasPermission(Permissions.Settings.CompanyInfo.View)) settingsChildren.push({ key: 'company-info', label: 'Company Info', url: '/settings/company-info', parentKey: 'system-settings' })
    if (hasPermission(Permissions.Settings.EmailConfig.View)) settingsChildren.push({ key: 'email-config', label: 'Email Config', url: '/settings/email-config', parentKey: 'system-settings' })
    if (hasPermission(Permissions.Settings.Currency.View)) settingsChildren.push({ key: 'currencies', label: 'Manage Currency', url: '/settings/currencies', parentKey: 'system-settings' })
    if (hasPermission(Permissions.Settings.PaymentType.View)) settingsChildren.push({ key: 'payment-types', label: 'Payment Type', url: '/settings/payment-types', parentKey: 'system-settings' })
    if (hasPermission(Permissions.Settings.PaymentStatus.View)) settingsChildren.push({ key: 'payment-statuses', label: 'Payment Status', url: '/settings/payment-statuses', parentKey: 'system-settings' })
    if (hasPermission(Permissions.Settings.CustomerType.View)) settingsChildren.push({ key: 'customer-types', label: 'Customer Type', url: '/settings/customer-types', parentKey: 'system-settings' })
    if (hasPermission(Permissions.Settings.VatPercentage.View)) settingsChildren.push({ key: 'vat-percentages', label: 'VAT Percentage', url: '/settings/vat-percentages', parentKey: 'system-settings' })
    if (hasPermission(Permissions.Settings.Category.View)) settingsChildren.push({ key: 'categories', label: 'Categories', url: '/settings/categories', parentKey: 'system-settings' })
    if (hasPermission(Permissions.Settings.UnitOfMeasure.View)) settingsChildren.push({ key: 'uom', label: 'Units of Measure', url: '/settings/units-of-measure', parentKey: 'system-settings' })
    if (settingsChildren.length > 0) {
      items.push({ key: 'system-settings', icon: 'solar:settings-outline', label: 'System Settings', children: settingsChildren })
    }

    return items
  }, [hasPermission, isSuperAdmin])
}
