const ACCESS_TOKEN_KEY = "litxuscount_access_token";
const REFRESH_TOKEN_KEY = "litxuscount_refresh_token";
const REMEMBER_FLAG_KEY = "litxuscount_remember_me";
const ACTIVE_TENANT_KEY = "litxuscount_active_tenant_id";

function activeStorage(): Storage {
  return localStorage.getItem(REMEMBER_FLAG_KEY) === "true" ? localStorage : sessionStorage;
}

export const authStorage = {
  getAccessToken: () => activeStorage().getItem(ACCESS_TOKEN_KEY),
  getRefreshToken: () => activeStorage().getItem(REFRESH_TOKEN_KEY),
  setTokens: (accessToken: string, refreshToken: string, rememberMe: boolean) => {
    localStorage.setItem(REMEMBER_FLAG_KEY, String(rememberMe));
    const storage = rememberMe ? localStorage : sessionStorage;
    storage.setItem(ACCESS_TOKEN_KEY, accessToken);
    storage.setItem(REFRESH_TOKEN_KEY, refreshToken);
  },
  getActiveTenantId: (): string | null => sessionStorage.getItem(ACTIVE_TENANT_KEY) ?? localStorage.getItem(ACTIVE_TENANT_KEY),
  setActiveTenantId: (id: string) => {
    const storage = localStorage.getItem(REMEMBER_FLAG_KEY) === "true" ? localStorage : sessionStorage;
    storage.setItem(ACTIVE_TENANT_KEY, id);
  },
  clear: () => {
    localStorage.removeItem(ACCESS_TOKEN_KEY);
    localStorage.removeItem(REFRESH_TOKEN_KEY);
    localStorage.removeItem(REMEMBER_FLAG_KEY);
    localStorage.removeItem(ACTIVE_TENANT_KEY);
    sessionStorage.removeItem(ACCESS_TOKEN_KEY);
    sessionStorage.removeItem(REFRESH_TOKEN_KEY);
    sessionStorage.removeItem(ACTIVE_TENANT_KEY);
  },
  isAuthenticated: () => Boolean(activeStorage().getItem(ACCESS_TOKEN_KEY)),
};
