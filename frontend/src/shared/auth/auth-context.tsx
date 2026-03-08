import { createContext, useContext, useMemo, useState } from "react";
import type { ReactNode } from "react";
import { apiFetch } from "@shared/api/client";
import type { AuthState, LoginResult } from "@shared/auth/types";

interface AuthContextValue {
  auth: AuthState | null;
  isAuthenticated: boolean;
  login: (email: string, password: string) => Promise<void>;
  logout: () => void;
}

const AUTH_STORAGE_KEY = "erp_auth_state";

const AuthContext = createContext<AuthContextValue | undefined>(undefined);

function readStoredAuth(): AuthState | null {
  const raw = localStorage.getItem(AUTH_STORAGE_KEY);
  if (!raw) return null;

  try {
    const parsed = JSON.parse(raw) as AuthState;
    if (!parsed.token || !parsed.user) {
      return null;
    }

    return parsed;
  } catch {
    return null;
  }
}

export function AuthProvider({ children }: { children: ReactNode }) {
  const [auth, setAuth] = useState<AuthState | null>(() => readStoredAuth());

  async function login(email: string, password: string): Promise<void> {
    const result = await apiFetch<LoginResult>("/api/v1/auth/login", {
      method: "POST",
      body: JSON.stringify({ email, password })
    });

    const nextAuth: AuthState = {
      token: result.accessToken,
      user: result.user,
      expiresAtUtc: result.expiresAtUtc
    };

    localStorage.setItem(AUTH_STORAGE_KEY, JSON.stringify(nextAuth));
    setAuth(nextAuth);
  }

  function logout(): void {
    localStorage.removeItem(AUTH_STORAGE_KEY);
    setAuth(null);
  }

  const value = useMemo<AuthContextValue>(
    () => ({
      auth,
      isAuthenticated: auth !== null,
      login,
      logout
    }),
    [auth]
  );

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}

export function useAuth(): AuthContextValue {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error("useAuth must be used within AuthProvider");
  }

  return context;
}
