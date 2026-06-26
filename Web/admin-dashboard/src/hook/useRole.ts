import { useMemo } from 'react'
import { decodeJwtPayload } from '../api/jwt'
import { authStorage } from '../api/authStorage'

const ROLE_CLAIM_LONG  = 'http://schemas.microsoft.com/ws/2008/06/identity/claims/role'
const ROLE_CLAIM_SHORT = 'role'

export function useRole() {
  const roles = useMemo(() => {
    const token = authStorage.getAccessToken()
    if (!token) return [] as string[]
    const payload = decodeJwtPayload(token)
    const raw = payload?.[ROLE_CLAIM_LONG] ?? payload?.[ROLE_CLAIM_SHORT]
    return (Array.isArray(raw) ? raw : raw ? [raw] : []).map(String)
  }, [])

  return {
    roles,
    isSuperAdmin: roles.includes('SuperAdmin'),
  }
}
