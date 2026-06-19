import { useEffect, useState, type ReactNode } from "react";
import { Icon } from "@iconify/react/dist/iconify.js";
import { Link, NavLink, useLocation } from "react-router-dom";
import ThemeToggleButton from "../helper/ThemeToggleButton";
import { authStorage } from "../api/authStorage";
import { usePermissions } from "../hook/usePermissions";
import { Permissions } from "../api/permissions";

interface MasterLayoutProps {
  children: ReactNode;
}

const MasterLayout = ({ children }: MasterLayoutProps) => {
  const [sidebarActive, setSidebarActive] = useState(false);
  const [mobileMenu, setMobileMenu] = useState(false);
  const location = useLocation();
  const { hasPermission } = usePermissions();

  useEffect(() => {
    const handleDropdownClick = (event: Event) => {
      event.preventDefault();
      const clickedLink = event.currentTarget as HTMLElement;
      const clickedDropdown = clickedLink.closest(".dropdown");

      if (!clickedDropdown) return;

      const isActive = clickedDropdown.classList.contains("open");

      const allDropdowns = document.querySelectorAll(".sidebar-menu .dropdown");
      allDropdowns.forEach((dropdown) => {
        dropdown.classList.remove("open");
        const submenu = dropdown.querySelector<HTMLElement>(".sidebar-submenu");
        if (submenu) {
          submenu.style.maxHeight = "0px";
        }
      });

      if (!isActive) {
        clickedDropdown.classList.add("open");
        const submenu = clickedDropdown.querySelector<HTMLElement>(".sidebar-submenu");
        if (submenu) {
          submenu.style.maxHeight = `${submenu.scrollHeight}px`;
        }
      }
    };

    const dropdownTriggers = document.querySelectorAll(
      ".sidebar-menu .dropdown > a, .sidebar-menu .dropdown > Link",
    );

    dropdownTriggers.forEach((trigger) => {
      trigger.addEventListener("click", handleDropdownClick);
    });

    const openActiveDropdown = () => {
      const allDropdowns = document.querySelectorAll(".sidebar-menu .dropdown");
      allDropdowns.forEach((dropdown) => {
        const submenuLinks = dropdown.querySelectorAll("a, Link");
        submenuLinks.forEach((link) => {
          if (
            link.getAttribute("href") === location.pathname ||
            link.getAttribute("to") === location.pathname
          ) {
            dropdown.classList.add("open");
            const submenu = dropdown.querySelector<HTMLElement>(".sidebar-submenu");
            if (submenu) {
              submenu.style.maxHeight = `${submenu.scrollHeight}px`;
            }
          }
        });
      });
    };

    openActiveDropdown();

    return () => {
      dropdownTriggers.forEach((trigger) => {
        trigger.removeEventListener("click", handleDropdownClick);
      });
    };
  }, [location.pathname]);

  const sidebarControl = () => setSidebarActive(!sidebarActive);
  const mobileMenuControl = () => setMobileMenu(!mobileMenu);

  return (
    <section className={mobileMenu ? "overlay active" : "overlay "}>
      <aside
        className={
          sidebarActive ? "sidebar active " : mobileMenu ? "sidebar sidebar-open" : "sidebar"
        }
      >
        <button onClick={mobileMenuControl} type='button' className='sidebar-close-btn'>
          <Icon icon='radix-icons:cross-2' />
        </button>
        <div>
          <Link to='/' className='sidebar-logo'>
            <img src='assets/images/logo.png' alt='LitXusCount' className='light-logo' />
            <img src='assets/images/logo-light.png' alt='LitXusCount' className='dark-logo' />
            <img src='assets/images/logo-icon.png' alt='LitXusCount' className='logo-icon' />
          </Link>
        </div>
        <div className='sidebar-menu-area'>
          <ul className='sidebar-menu' id='sidebar-menu'>
            <li>
              <NavLink to='/' className={(navData) => (navData.isActive ? "active-page" : "")}>
                <Icon icon='solar:home-smile-angle-outline' className='menu-icon' />
                <span>Dashboard</span>
              </NavLink>
            </li>

            <li className='sidebar-menu-group-title'>Modules</li>
            <li>
              <NavLink
                to='/inventory'
                className={(navData) => (navData.isActive ? "active-page" : "")}
              >
                <Icon icon='solar:box-outline' className='menu-icon' />
                <span>Inventory</span>
              </NavLink>
            </li>
            <li>
              <NavLink
                to='/sales'
                className={(navData) => (navData.isActive ? "active-page" : "")}
              >
                <Icon icon='solar:cart-large-outline' className='menu-icon' />
                <span>Sales</span>
              </NavLink>
            </li>
            <li>
              <NavLink
                to='/purchasing'
                className={(navData) => (navData.isActive ? "active-page" : "")}
              >
                <Icon icon='solar:bag-outline' className='menu-icon' />
                <span>Purchasing</span>
              </NavLink>
            </li>
            <li>
              <NavLink
                to='/finance'
                className={(navData) => (navData.isActive ? "active-page" : "")}
              >
                <Icon icon='hugeicons:money-send-square' className='menu-icon' />
                <span>Finance</span>
              </NavLink>
            </li>
            {(hasPermission(Permissions.Users.View) || hasPermission(Permissions.Roles.View)) && (
              <li className='dropdown'>
                <Link to='#'>
                  <Icon icon='icon-park-outline:setting-two' className='menu-icon' />
                  <span>Identity / Admin</span>
                </Link>
                <ul className='sidebar-submenu'>
                  {hasPermission(Permissions.Users.View) && (
                    <li>
                      <NavLink to='/admin/users' className={(navData) => (navData.isActive ? "active-page" : "")}>
                        <i className='ri-circle-fill circle-icon text-primary-600 w-auto' /> Users
                      </NavLink>
                    </li>
                  )}
                  {hasPermission(Permissions.Roles.View) && (
                    <li>
                      <NavLink to='/admin/roles' className={(navData) => (navData.isActive ? "active-page" : "")}>
                        <i className='ri-circle-fill circle-icon text-warning-main w-auto' /> Roles
                      </NavLink>
                    </li>
                  )}
                </ul>
              </li>
            )}

            <li className='sidebar-menu-group-title'>System Settings</li>
            <li className='dropdown'>
              <Link to='#'>
                <Icon icon='solar:settings-outline' className='menu-icon' />
                <span>System Settings</span>
              </Link>
              <ul className='sidebar-submenu'>
                {hasPermission(Permissions.Settings.CompanyInfo.View) && (
                  <li>
                    <NavLink
                      to='/settings/company-info'
                      className={(navData) => (navData.isActive ? "active-page" : "")}
                    >
                      <i className='ri-circle-fill circle-icon text-primary-600 w-auto' /> Company Info
                    </NavLink>
                  </li>
                )}
                {hasPermission(Permissions.Settings.EmailConfig.View) && (
                  <li>
                    <NavLink
                      to='/settings/email-config'
                      className={(navData) => (navData.isActive ? "active-page" : "")}
                    >
                      <i className='ri-circle-fill circle-icon text-warning-main w-auto' /> Email Config
                    </NavLink>
                  </li>
                )}
                {hasPermission(Permissions.Settings.Currency.View) && (
                  <li>
                    <NavLink
                      to='/settings/currencies'
                      className={(navData) => (navData.isActive ? "active-page" : "")}
                    >
                      <i className='ri-circle-fill circle-icon text-success-main w-auto' /> Manage Currency
                    </NavLink>
                  </li>
                )}
                {hasPermission(Permissions.Settings.PaymentType.View) && (
                  <li>
                    <NavLink
                      to='/settings/payment-types'
                      className={(navData) => (navData.isActive ? "active-page" : "")}
                    >
                      <i className='ri-circle-fill circle-icon text-info-main w-auto' /> Payment Type
                    </NavLink>
                  </li>
                )}
                {hasPermission(Permissions.Settings.PaymentStatus.View) && (
                  <li>
                    <NavLink
                      to='/settings/payment-statuses'
                      className={(navData) => (navData.isActive ? "active-page" : "")}
                    >
                      <i className='ri-circle-fill circle-icon text-danger-main w-auto' /> Payment Status
                    </NavLink>
                  </li>
                )}
                {hasPermission(Permissions.Settings.CustomerType.View) && (
                  <li>
                    <NavLink
                      to='/settings/customer-types'
                      className={(navData) => (navData.isActive ? "active-page" : "")}
                    >
                      <i className='ri-circle-fill circle-icon text-primary-600 w-auto' /> Customer Type
                    </NavLink>
                  </li>
                )}
                {hasPermission(Permissions.Settings.VatPercentage.View) && (
                  <li>
                    <NavLink
                      to='/settings/vat-percentages'
                      className={(navData) => (navData.isActive ? "active-page" : "")}
                    >
                      <i className='ri-circle-fill circle-icon text-warning-main w-auto' /> VAT Percentage
                    </NavLink>
                  </li>
                )}
                {hasPermission(Permissions.Settings.Category.View) && (
                  <li>
                    <NavLink
                      to='/settings/categories'
                      className={(navData) => (navData.isActive ? "active-page" : "")}
                    >
                      <i className='ri-circle-fill circle-icon text-success-main w-auto' /> Categories
                    </NavLink>
                  </li>
                )}
                {hasPermission(Permissions.Settings.UnitOfMeasure.View) && (
                  <li>
                    <NavLink
                      to='/settings/units-of-measure'
                      className={(navData) => (navData.isActive ? "active-page" : "")}
                    >
                      <i className='ri-circle-fill circle-icon text-info-main w-auto' /> Units of Measure
                    </NavLink>
                  </li>
                )}
              </ul>
            </li>

            <li className='sidebar-menu-group-title'>Reference</li>
            <li className='dropdown'>
              <Link to='#'>
                <Icon icon='mingcute:storage-line' className='menu-icon' />
                <span>Table</span>
              </Link>
              <ul className='sidebar-submenu'>
                <li>
                  <NavLink
                    to='/table-basic'
                    className={(navData) => (navData.isActive ? "active-page" : "")}
                  >
                    <i className='ri-circle-fill circle-icon text-primary-600 w-auto' /> Basic
                    Table
                  </NavLink>
                </li>
                <li>
                  <NavLink
                    to='/table-data'
                    className={(navData) => (navData.isActive ? "active-page" : "")}
                  >
                    <i className='ri-circle-fill circle-icon text-warning-main w-auto' /> Data
                    Table
                  </NavLink>
                </li>
              </ul>
            </li>
            <li>
              <NavLink to='/form' className={(navData) => (navData.isActive ? "active-page" : "")}>
                <Icon icon='heroicons:document' className='menu-icon' />
                <span>Form</span>
              </NavLink>
            </li>
          </ul>
        </div>
      </aside>

      <main className={sidebarActive ? "dashboard-main active" : "dashboard-main"}>
        <div className='navbar-header'>
          <div className='row align-items-center justify-content-between'>
            <div className='col-auto'>
              <div className='d-flex flex-wrap align-items-center gap-4'>
                <button type='button' className='sidebar-toggle' onClick={sidebarControl}>
                  {sidebarActive ? (
                    <Icon icon='iconoir:arrow-right' className='icon text-2xl non-active' />
                  ) : (
                    <Icon icon='heroicons:bars-3-solid' className='icon text-2xl non-active ' />
                  )}
                </button>
                <button onClick={mobileMenuControl} type='button' className='sidebar-mobile-toggle'>
                  <Icon icon='heroicons:bars-3-solid' className='icon' />
                </button>
              </div>
            </div>
            <div className='col-auto'>
              <div className='d-flex flex-wrap align-items-center gap-3'>
                <ThemeToggleButton />
                <div className='dropdown'>
                  <button
                    className='d-flex justify-content-center align-items-center rounded-circle'
                    type='button'
                    data-bs-toggle='dropdown'
                  >
                    <img
                      src='assets/images/user.png'
                      alt='image_user'
                      className='w-40-px h-40-px object-fit-cover rounded-circle'
                    />
                  </button>
                  <div className='dropdown-menu to-top dropdown-menu-sm'>
                    <div className='py-12 px-16 radius-8 bg-primary-50 mb-16 d-flex align-items-center justify-content-between gap-2'>
                      <div>
                        <h6 className='text-lg text-primary-light fw-semibold mb-2'>Admin</h6>
                      </div>
                    </div>
                    <ul className='to-top-list'>
                      <li>
                        <Link
                          className='dropdown-item text-black px-0 py-8 hover-bg-transparent hover-text-danger d-flex align-items-center gap-3'
                          to='/sign-in'
                          onClick={() => authStorage.clear()}
                        >
                          <Icon icon='lucide:power' className='icon text-xl' /> Log Out
                        </Link>
                      </li>
                    </ul>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>

        <div className='dashboard-main-body'>{children}</div>

        <footer className='d-footer'>
          <div className='row align-items-center justify-content-between'>
            <div className='col-auto'>
              <p className='mb-0'>© 2026 LitXusCount. All Rights Reserved.</p>
            </div>
          </div>
        </footer>
      </main>
    </section>
  );
};

export default MasterLayout;
