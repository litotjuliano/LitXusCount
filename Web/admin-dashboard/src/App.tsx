import { BrowserRouter, Route, Routes } from 'react-router-dom'
import { ToastContainer } from 'react-toastify'
import { LayoutProvider } from './context/useLayoutContext'
import { AuthProvider } from './context/useAuthContext'
import RouteScrollToTop from './helper/RouteScrollToTop'
import ProtectedRoute from './components/ProtectedRoute'
import HomePageOne from './pages/HomePageOne'
import SignInPage from './pages/SignInPage'
import ForgotPasswordPage from './pages/ForgotPasswordPage'
import ResetPasswordPage from './pages/ResetPasswordPage'
import ErrorPage from './pages/ErrorPage'
import TableBasicPage from './pages/TableBasicPage'
import TableDataPage from './pages/TableDataPage'
import FormPage from './pages/FormPage'
import InventoryPage from './pages/InventoryPage'
import SalesPage from './pages/SalesPage'
import PurchasingPage from './pages/PurchasingPage'
import FinancePage from './pages/FinancePage'
import UsersPage from './pages/UsersPage'
import RolesPage from './pages/RolesPage'
import CompanyInfoPage from './pages/CompanyInfoPage'
import EmailConfigsPage from './pages/EmailConfigsPage'
import CurrenciesPage from './pages/CurrenciesPage'
import PaymentCodesPage from './pages/PaymentCodesPage'
import PaymentStatusesPage from './pages/PaymentStatusesPage'
import CustomerTypesPage from './pages/CustomerTypesPage'
import VatPercentagesPage from './pages/VatPercentagesPage'
import CategoriesPage from './pages/CategoriesPage'
import UnitsOfMeasurePage from './pages/UnitsOfMeasurePage'
import LhdnClassificationCodesPage from './pages/LhdnClassificationCodesPage'
import LhdnCountriesPage from './pages/LhdnCountriesPage'
import LhdnStateCodesPage from './pages/LhdnStateCodesPage'
import LhdnTaxTypesPage from './pages/LhdnTaxTypesPage'
import LhdnMsicCodesPage from './pages/LhdnMsicCodesPage'
import LhdnCurrencyCodesPage from './pages/LhdnCurrencyCodesPage'
import LhdnEInvoiceTypesPage from './pages/LhdnEInvoiceTypesPage'
import GlAccountsPage from './pages/GlAccountsPage'
import CustomersPage from './pages/CustomersPage'
import SuppliersPage from './pages/SuppliersPage'
import ProductsPage from './pages/ProductsPage'
import AccountsPage from './pages/AccountsPage'
import DepositsPage from './pages/DepositsPage'
import ExpensesPage from './pages/ExpensesPage'
import TransfersPage from './pages/TransfersPage'
import SalesInvoicesPage from './pages/SalesInvoicesPage'
import SalesInvoiceEditorPage from './pages/SalesInvoiceEditorPage'
import SalesInvoiceDetailPage from './pages/SalesInvoiceDetailPage'
import TenantsPage from './pages/TenantsPage'

function App() {
  return (
    <BrowserRouter>
      <AuthProvider>
        <LayoutProvider>
          <RouteScrollToTop />
          <ToastContainer position="top-right" autoClose={4000} theme="colored" />
          <Routes>
            <Route path="/sign-in" element={<SignInPage />} />
            <Route path="/forgot-password" element={<ForgotPasswordPage />} />
            <Route path="/reset-password" element={<ResetPasswordPage />} />

            <Route path="/" element={<ProtectedRoute><HomePageOne /></ProtectedRoute>} />
            <Route path="/inventory" element={<ProtectedRoute><InventoryPage /></ProtectedRoute>} />
            <Route path="/sales" element={<ProtectedRoute><SalesPage /></ProtectedRoute>} />
            <Route path="/purchasing" element={<ProtectedRoute><PurchasingPage /></ProtectedRoute>} />
            <Route path="/finance" element={<ProtectedRoute><FinancePage /></ProtectedRoute>} />
            <Route path="/admin/users" element={<ProtectedRoute><UsersPage /></ProtectedRoute>} />
            <Route path="/admin/roles" element={<ProtectedRoute><RolesPage /></ProtectedRoute>} />
            <Route path="/admin/tenants" element={<ProtectedRoute><TenantsPage /></ProtectedRoute>} />
            <Route path="/settings/company-info" element={<ProtectedRoute><CompanyInfoPage /></ProtectedRoute>} />
            <Route path="/settings/email-config" element={<ProtectedRoute><EmailConfigsPage /></ProtectedRoute>} />
            <Route path="/settings/currencies" element={<ProtectedRoute><CurrenciesPage /></ProtectedRoute>} />
            <Route path="/settings/payment-codes" element={<ProtectedRoute><PaymentCodesPage /></ProtectedRoute>} />
            <Route path="/settings/payment-statuses" element={<ProtectedRoute><PaymentStatusesPage /></ProtectedRoute>} />
            <Route path="/settings/customer-types" element={<ProtectedRoute><CustomerTypesPage /></ProtectedRoute>} />
            <Route path="/settings/vat-percentages" element={<ProtectedRoute><VatPercentagesPage /></ProtectedRoute>} />
            <Route path="/settings/categories" element={<ProtectedRoute><CategoriesPage /></ProtectedRoute>} />
            <Route path="/settings/units-of-measure" element={<ProtectedRoute><UnitsOfMeasurePage /></ProtectedRoute>} />
            <Route path="/settings/lhdn-classification-codes" element={<ProtectedRoute><LhdnClassificationCodesPage /></ProtectedRoute>} />
            <Route path="/settings/lhdn-countries" element={<ProtectedRoute><LhdnCountriesPage /></ProtectedRoute>} />
            <Route path="/settings/lhdn-state-codes" element={<ProtectedRoute><LhdnStateCodesPage /></ProtectedRoute>} />
            <Route path="/settings/lhdn-tax-types" element={<ProtectedRoute><LhdnTaxTypesPage /></ProtectedRoute>} />
            <Route path="/settings/lhdn-msic-codes" element={<ProtectedRoute><LhdnMsicCodesPage /></ProtectedRoute>} />
            <Route path="/settings/lhdn-currency-codes" element={<ProtectedRoute><LhdnCurrencyCodesPage /></ProtectedRoute>} />
            <Route path="/settings/lhdn-einvoice-types" element={<ProtectedRoute><LhdnEInvoiceTypesPage /></ProtectedRoute>} />
            <Route path="/settings/gl-accounts" element={<ProtectedRoute><GlAccountsPage /></ProtectedRoute>} />
            <Route path="/settings/customers" element={<ProtectedRoute><CustomersPage /></ProtectedRoute>} />
            <Route path="/settings/suppliers" element={<ProtectedRoute><SuppliersPage /></ProtectedRoute>} />
            <Route path="/settings/products" element={<ProtectedRoute><ProductsPage /></ProtectedRoute>} />
            <Route path="/accounts" element={<ProtectedRoute><AccountsPage /></ProtectedRoute>} />
            <Route path="/accounts/deposits" element={<ProtectedRoute><DepositsPage /></ProtectedRoute>} />
            <Route path="/accounts/expenses" element={<ProtectedRoute><ExpensesPage /></ProtectedRoute>} />
            <Route path="/accounts/transfers" element={<ProtectedRoute><TransfersPage /></ProtectedRoute>} />
            <Route path="/sales/invoices" element={<ProtectedRoute><SalesInvoicesPage /></ProtectedRoute>} />
            <Route path="/sales/invoices/:id" element={<ProtectedRoute><SalesInvoiceEditorPage /></ProtectedRoute>} />
            <Route path="/sales/invoices/:id/view" element={<ProtectedRoute><SalesInvoiceDetailPage /></ProtectedRoute>} />
            <Route path="/table-basic" element={<ProtectedRoute><TableBasicPage /></ProtectedRoute>} />
            <Route path="/table-data" element={<ProtectedRoute><TableDataPage /></ProtectedRoute>} />
            <Route path="/form" element={<ProtectedRoute><FormPage /></ProtectedRoute>} />
            <Route path="*" element={<ErrorPage />} />
          </Routes>
        </LayoutProvider>
      </AuthProvider>
    </BrowserRouter>
  )
}

export default App
