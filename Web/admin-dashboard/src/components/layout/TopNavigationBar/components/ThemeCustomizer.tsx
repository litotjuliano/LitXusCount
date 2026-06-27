import IconifyIcon from '@/components/wrappers/IconifyIcon'
import { useLayoutContext } from '@/context/useLayoutContext'
import type { MenuType, ThemeType } from '@/types/context'

interface OptionCardProps {
  icon: string
  label: string
  active: boolean
  onClick: () => void
}

const OptionCard = ({ icon, label, active, onClick }: OptionCardProps) => (
  <button
    type="button"
    onClick={onClick}
    className={`btn border rounded p-2 d-flex flex-column align-items-center gap-1 ${
      active ? 'border-primary bg-primary bg-opacity-10 text-primary' : ''
    }`}
    style={{ width: 76, fontSize: 11 }}>
    <IconifyIcon icon={icon} style={{ fontSize: 20 }} />
    <span>{label}</span>
  </button>
)

interface SectionProps {
  title: string
  children: React.ReactNode
}

const Section = ({ title, children }: SectionProps) => (
  <div>
    <p className="text-uppercase fw-semibold mb-2" style={{ fontSize: 11, letterSpacing: '0.06em' }}>
      {title}
    </p>
    <div className="d-flex flex-wrap gap-2">{children}</div>
  </div>
)

const SIDENAV_MODES: { label: string; value: MenuType['size']; icon: string }[] = [
  { label: 'Default',       value: 'default',        icon: 'solar:sidebar-minimalistic-outline' },
  { label: 'Hover',         value: 'sm-hover',       icon: 'solar:quit-full-screen-square-outline' },
  { label: 'Hover Active',  value: 'sm-hover-active',icon: 'solar:full-screen-square-outline' },
  { label: 'Condensed',     value: 'condensed',      icon: 'solar:minimize-square-3-outline' },
  { label: 'Hidden',        value: 'hidden',         icon: 'solar:maximize-square-minimalistic-outline' },
]

const ThemeCustomizer = () => {
  const {
    theme,
    topbarTheme,
    menu,
    changeTheme,
    changeTopbarTheme,
    changeMenu,
    themeCustomizer: { open, toggle },
    resetSettings,
  } = useLayoutContext()

  return (
    <>
      {open && <div className="offcanvas-backdrop fade show" onClick={toggle} />}
      <div
        className={`offcanvas offcanvas-end${open ? ' show' : ''}`}
        style={{ width: 290, visibility: open ? 'visible' : 'hidden' }}>
        <div className="offcanvas-header border-bottom">
          <h5 className="offcanvas-title fw-semibold mb-0">UI Settings</h5>
          <button type="button" className="btn-close" onClick={toggle} aria-label="Close" />
        </div>

        <div className="offcanvas-body d-flex flex-column gap-4 overflow-auto">
          <Section title="Layout Mode">
            {(['light', 'dark'] as ThemeType[]).map((t) => (
              <OptionCard
                key={t}
                icon={t === 'light' ? 'solar:sun-outline' : 'solar:moon-outline'}
                label={t === 'light' ? 'Light' : 'Dark'}
                active={theme === t}
                onClick={() => changeTheme(t)}
              />
            ))}
          </Section>

          <Section title="Sidenav Menu">
            {SIDENAV_MODES.map(({ label, value, icon }) => (
              <OptionCard
                key={value}
                icon={icon}
                label={label}
                active={menu.size === value}
                onClick={() => changeMenu.size(value)}
              />
            ))}
          </Section>

          <Section title="Sidenav Color">
            {(['light', 'dark'] as MenuType['theme'][]).map((t) => (
              <OptionCard
                key={t}
                icon={t === 'light' ? 'solar:sidebar-outline' : 'solar:sidebar-bold'}
                label={t === 'light' ? 'Light' : 'Dark'}
                active={menu.theme === t}
                onClick={() => changeMenu.theme(t)}
              />
            ))}
          </Section>

          <Section title="Topbar Color">
            {(['light', 'dark'] as ThemeType[]).map((t) => (
              <OptionCard
                key={t}
                icon={t === 'light' ? 'solar:monitor-outline' : 'solar:monitor-bold'}
                label={t === 'light' ? 'Light' : 'Dark'}
                active={topbarTheme === t}
                onClick={() => changeTopbarTheme(t)}
              />
            ))}
          </Section>
        </div>

        <div className="p-3 border-top">
          <button type="button" className="btn btn-outline-secondary w-100" onClick={resetSettings}>
            Reset to Defaults
          </button>
        </div>
      </div>
    </>
  )
}

export default ThemeCustomizer
