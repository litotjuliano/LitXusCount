const ACCESS_TOKEN_KEY = "litxuscount_access_token";
const REFRESH_TOKEN_KEY = "litxuscount_refresh_token";
const REMEMBER_FLAG_KEY = "litxuscount_remember_me";

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
  clear: () => {
    localStorage.removeItem(ACCESS_TOKEN_KEY);
    localStorage.removeItem(REFRESH_TOKEN_KEY);
    localStorage.removeItem(REMEMBER_FLAG_KEY);
    sessionStorage.removeItem(ACCESS_TOKEN_KEY);
    sessionStorage.removeItem(REFRESH_TOKEN_KEY);
  },
  isAuthenticated: () => Boolean(activeStorage().getItem(ACCESS_TOKEN_KEY)),
};
