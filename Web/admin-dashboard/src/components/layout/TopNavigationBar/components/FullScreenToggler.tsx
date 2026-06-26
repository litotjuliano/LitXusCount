import { useState } from 'react'
import IconifyIcon from '@/components/wrappers/IconifyIcon'

const FullScreenToggler = () => {
  const [fullScreenOn, setFullScreenOn] = useState(false)

  const toggleFullScreen = () => {
    if (!document.fullscreenElement) {
      document.documentElement.requestFullscreen()
      setFullScreenOn(true)
    } else {
      document.exitFullscreen()
      setFullScreenOn(false)
    }
  }

  return (
    <div className="topbar-item d-none d-lg-flex">
      <button type="button" onClick={toggleFullScreen} className="topbar-button">
        <IconifyIcon icon={`solar:${fullScreenOn ? 'quit-full-screen' : 'full-screen'}-broken`} className="fs-24 align-middle" />
      </button>
    </div>
  )
}

export default FullScreenToggler
