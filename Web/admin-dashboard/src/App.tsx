import { BrowserRouter, Route, Routes } from "react-router-dom";
import { ToastContainer } from "react-toastify";
import RouteScrollToTop from "./helper/RouteScrollToTop";
import ProtectedRoute from "./components/ProtectedRoute";
import HomePageOne from "./pages/HomePageOne";
import SignInPage from "./pages/SignInPage";
import ForgotPasswordPage from "./pages/ForgotPasswordPage";
import ResetPasswordPage from "./pages/ResetPasswordPage";
import ErrorPage from "./pages/ErrorPage";
import TableBasicPage from "./pages/TableBasicPage";
import TableDataPage from "./pages/TableDataPage";
import FormPage from "./pages/FormPage";
import InventoryPage from "./pages/InventoryPage";
import SalesPage from "./pages/SalesPage";
import PurchasingPage from "./pages/PurchasingPage";
import FinancePage from "./pages/FinancePage";
import UsersPage from "./pages/UsersPage";
import RolesPage from "./pages/RolesPage";
import CompanyInfoPage from "./pages/CompanyInfoPage";
import EmailConfigsPage from "./pages/EmailConfigsPage";
import CurrenciesPage from "./pages/CurrenciesPage";
import PaymentTypesPage from "./pages/PaymentTypesPage";
import PaymentStatusesPage from "./pages/PaymentStatusesPage";
import CustomerTypesPage from "./pages/CustomerTypesPage";
import VatPercentagesPage from "./pages/VatPercentagesPage";
import CategoriesPage from "./pages/CategoriesPage";
import UnitsOfMeasurePage from "./pages/UnitsOfMeasurePage";

function App() {
  return (
    <BrowserRouter>
      <RouteScrollToTop />
      <ToastContainer position='top-right' autoClose={4000} />
      <Routes>
        <Route path='/sign-in' element={<SignInPage />} />
        <Route path='/forgot-password' element={<ForgotPasswordPage />} />
        <Route path='/reset-password' element={<ResetPasswordPage />} />

        <Route
          path='/'
          element={
            <ProtectedRoute>
              <HomePageOne />
            </ProtectedRoute>
          }
        />
        <Route
          path='/inventory'
          element={
            <ProtectedRoute>
              <InventoryPage />
            </ProtectedRoute>
          }
        />
        <Route
          path='/sales'
          element={
            <ProtectedRoute>
              <SalesPage />
            </ProtectedRoute>
          }
        />
        <Route
          path='/purchasing'
          element={
            <ProtectedRoute>
              <PurchasingPage />
            </ProtectedRoute>
          }
        />
        <Route
          path='/finance'
          element={
            <ProtectedRoute>
              <FinancePage />
            </ProtectedRoute>
          }
        />
        <Route
          path='/admin/users'
          element={
            <ProtectedRoute>
              <UsersPage />
            </ProtectedRoute>
          }
        />
        <Route
          path='/admin/roles'
          element={
            <ProtectedRoute>
              <RolesPage />
            </ProtectedRoute>
          }
        />
        <Route
          path='/settings/company-info'
          element={
            <ProtectedRoute>
              <CompanyInfoPage />
            </ProtectedRoute>
          }
        />
        <Route
          path='/settings/email-config'
          element={
            <ProtectedRoute>
              <EmailConfigsPage />
            </ProtectedRoute>
          }
        />
        <Route
          path='/settings/currencies'
          element={
            <ProtectedRoute>
              <CurrenciesPage />
            </ProtectedRoute>
          }
        />
        <Route
          path='/settings/payment-types'
          element={
            <ProtectedRoute>
              <PaymentTypesPage />
            </ProtectedRoute>
          }
        />
        <Route
          path='/settings/payment-statuses'
          element={
            <ProtectedRoute>
              <PaymentStatusesPage />
            </ProtectedRoute>
          }
        />
        <Route
          path='/settings/customer-types'
          element={
            <ProtectedRoute>
              <CustomerTypesPage />
            </ProtectedRoute>
          }
        />
        <Route
          path='/settings/vat-percentages'
          element={
            <ProtectedRoute>
              <VatPercentagesPage />
            </ProtectedRoute>
          }
        />
        <Route
          path='/settings/categories'
          element={
            <ProtectedRoute>
              <CategoriesPage />
            </ProtectedRoute>
          }
        />
        <Route
          path='/settings/units-of-measure'
          element={
            <ProtectedRoute>
              <UnitsOfMeasurePage />
            </ProtectedRoute>
          }
        />
        <Route
          path='/table-basic'
          element={
            <ProtectedRoute>
              <TableBasicPage />
            </ProtectedRoute>
          }
        />
        <Route
          path='/table-data'
          element={
            <ProtectedRoute>
              <TableDataPage />
            </ProtectedRoute>
          }
        />
        <Route
          path='/form'
          element={
            <ProtectedRoute>
              <FormPage />
            </ProtectedRoute>
          }
        />

        <Route path='*' element={<ErrorPage />} />
      </Routes>
    </BrowserRouter>
  );
}

export default App;
