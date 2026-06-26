import { createContext, useContext } from 'react'
import { useNavigate } from 'react-router-dom'
import { authStorage } from '@/api/authStorage'
import type { ChildrenType } from '@/types/component-props'

type AuthContextType = {
  removeSession: () => void
  userName: string
}

const AuthContext = createContext<AuthContextType | undefined>(undefined)

export const useAuthContext = () => {
  const context = useContext(AuthContext)
  if (!context) throw new Error('useAuthContext must be used within AuthProvider')
  return context
}

export const AuthProvider = ({ children }: ChildrenType) => {
  const navigate = useNavigate()

  const removeSession = () => {
    authStorage.clear()
    navigate('/sign-in')
  }

  const userName = 'Admin'

  return (
    <AuthContext.Provider value={{ removeSession, userName }}>
      {children}
    </AuthContext.Provider>
  )
}
