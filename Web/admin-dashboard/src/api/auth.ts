import { apiClient } from "./client";

export interface AuthResult {
  userId: string;
  userName: string;
  roles: string[];
  accessToken: string;
  accessTokenExpiresAt: string;
  refreshToken: string;
}

export async function login(userNameOrEmail: string, password: string): Promise<AuthResult> {
  const { data } = await apiClient.post<AuthResult>("/api/auth/login", {
    userNameOrEmail,
    password,
  });
  return data;
}

export async function me(): Promise<{ userId: string; userName: string; roles: string[] }> {
  const { data } = await apiClient.get("/api/auth/me");
  return data;
}

export interface ForgotPasswordResult {
  message: string;
  devResetLink: string | null;
}

export async function forgotPassword(email: string): Promise<ForgotPasswordResult> {
  const { data } = await apiClient.post<ForgotPasswordResult>("/api/auth/forgot-password", { email });
  return data;
}

export async function resetPassword(
  email: string,
  token: string,
  newPassword: string,
): Promise<{ message: string }> {
  const { data } = await apiClient.post("/api/auth/reset-password", { email, token, newPassword });
  return data;
}
