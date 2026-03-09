import { createContext, useContext, useEffect, useMemo, useRef, useState } from "react";
import type { ReactNode } from "react";
import { apiFetch } from "@shared/api/client";
import type { AuthState, LoginResult } from "@shared/auth/types";

interface AuthContextValue {
  auth: AuthState | null;
  isAuthenticated: boolean;
  login: (email: string, password: string) => Promise<void>;
  register: (email: string, password: string, firstName: string, lastName: string) => Promise<void>;
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
  const isRefreshingRef = useRef(false);

  function persistAuth(nextAuth: AuthState): void {
    localStorage.setItem(AUTH_STORAGE_KEY, JSON.stringify(nextAuth));
    setAuth(nextAuth);
  }

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

    persistAuth(nextAuth);
  }

  async function register(
    email: string,
    password: string,
    firstName: string,
    lastName: string
  ): Promise<void> {
    const result = await apiFetch<LoginResult>("/api/v1/auth/register", {
      method: "POST",
      body: JSON.stringify({ email, password, firstName, lastName })
    });

    const nextAuth: AuthState = {
      token: result.accessToken,
      user: result.user,
      expiresAtUtc: result.expiresAtUtc
    };

    persistAuth(nextAuth);
  }

  async function refreshToken(currentAuth: AuthState): Promise<void> {
    if (isRefreshingRef.current) {
      return;
    }

    isRefreshingRef.current = true;

    try {
      const result = await apiFetch<LoginResult>(
        "/api/v1/auth/refresh",
        {
          method: "POST"
        },
        currentAuth.token
      );

      const nextAuth: AuthState = {
        token: result.accessToken,
        user: result.user,
        expiresAtUtc: result.expiresAtUtc
      };

      persistAuth(nextAuth);
    } catch {
      logout();
    } finally {
      isRefreshingRef.current = false;
    }
  }

  async function callLogoutEndpoint(currentAuth: AuthState): Promise<void> {
    try {
      await apiFetch<void>(
        "/api/v1/auth/logout",
        {
          method: "POST"
        },
        currentAuth.token
      );
    } catch {
      // Ignore logout API failures since client-side cleanup is the source of truth for session end.
    }
  }

  function logout(): void {
    if (auth) {
      void callLogoutEndpoint(auth);
    }

    localStorage.removeItem(AUTH_STORAGE_KEY);
    setAuth(null);
  }

  useEffect(() => {
    if (!auth) {
      return;
    }

    const timer = setInterval(() => {
      const expiresAt = new Date(auth.expiresAtUtc).getTime();
      const now = Date.now();
      const refreshThresholdMs = 2 * 60 * 1000;

      if (expiresAt - now <= refreshThresholdMs) {
        void refreshToken(auth);
      }
    }, 30 * 1000);

    return () => clearInterval(timer);
  }, [auth]);

  useEffect(() => {
    function handleAuthError(event: Event) {
      const customEvent = event as CustomEvent<{ status: number; path: string }>;
      const requestPath = customEvent.detail?.path ?? "";

      // Do not force logout for explicit auth endpoint failures.
      if (requestPath.startsWith("/api/v1/auth/login") || requestPath.startsWith("/api/v1/auth/register")) {
        return;
      }

      if (auth) {
        localStorage.removeItem(AUTH_STORAGE_KEY);
        setAuth(null);
      }
    }

    window.addEventListener("erp:auth-error", handleAuthError as EventListener);
    return () => window.removeEventListener("erp:auth-error", handleAuthError as EventListener);
  }, [auth]);

  const value = useMemo<AuthContextValue>(
    () => ({
      auth,
      isAuthenticated: auth !== null,
      login,
      register,
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
