import { createContext, useContext, useEffect, useMemo, useRef, useState } from "react";
import type { ReactNode } from "react";
import { apiFetch } from "@shared/api/client";
import type { AuthResponse, AuthState, AuthUser } from "@shared/auth/types";

interface AuthContextValue {
  auth: AuthState | null;
  isAuthenticated: boolean;
  login: (email: string, password: string) => Promise<void>;
  register: (email: string, password: string, firstName: string, lastName: string) => Promise<void>;
  logout: () => void;
  isRestoring: boolean;
}

const AuthContext = createContext<AuthContextValue | undefined>(undefined);

export function AuthProvider({ children }: { children: ReactNode }) {
  const [auth, setAuth] = useState<AuthState | null>(null);
  const [isRestoring, setIsRestoring] = useState(true);
  const isRefreshingRef = useRef(false);

  // On mount, try to restore session from httpOnly cookie via /me endpoint
  useEffect(() => {
    let cancelled = false;

    async function restoreSession() {
      try {
        const user = await apiFetch<AuthUser>("/api/v1/auth/me");
        if (!cancelled) {
          // We don't know exact expiry from /me, so trigger a refresh to get fresh expiry
          const result = await apiFetch<AuthResponse>("/api/v1/auth/refresh", {
            method: "POST"
          });
          if (!cancelled) {
            setAuth({ user: result.user, expiresAtUtc: result.expiresAtUtc });
          }
        }
      } catch {
        if (!cancelled) {
          setAuth(null);
        }
      } finally {
        if (!cancelled) {
          setIsRestoring(false);
        }
      }
    }

    void restoreSession();
    return () => { cancelled = true; };
  }, []);

  async function login(email: string, password: string): Promise<void> {
    const result = await apiFetch<AuthResponse>("/api/v1/auth/login", {
      method: "POST",
      body: JSON.stringify({ email, password })
    });

    setAuth({ user: result.user, expiresAtUtc: result.expiresAtUtc });
  }

  async function register(
    email: string,
    password: string,
    firstName: string,
    lastName: string
  ): Promise<void> {
    const result = await apiFetch<AuthResponse>("/api/v1/auth/register", {
      method: "POST",
      body: JSON.stringify({ email, password, firstName, lastName })
    });

    setAuth({ user: result.user, expiresAtUtc: result.expiresAtUtc });
  }

  async function refreshToken(): Promise<void> {
    if (isRefreshingRef.current) {
      return;
    }

    isRefreshingRef.current = true;

    try {
      const result = await apiFetch<AuthResponse>("/api/v1/auth/refresh", {
        method: "POST"
      });

      setAuth({ user: result.user, expiresAtUtc: result.expiresAtUtc });
    } catch {
      setAuth(null);
    } finally {
      isRefreshingRef.current = false;
    }
  }

  function logout(): void {
    void apiFetch<void>("/api/v1/auth/logout", { method: "POST" }).catch(() => {});
    setAuth(null);
  }

  // Auto-refresh: check every 30s, refresh if within 2 min of expiry
  useEffect(() => {
    if (!auth) {
      return;
    }

    const timer = setInterval(() => {
      const expiresAt = new Date(auth.expiresAtUtc).getTime();
      const now = Date.now();
      const refreshThresholdMs = 2 * 60 * 1000;

      if (expiresAt - now <= refreshThresholdMs) {
        void refreshToken();
      }
    }, 30 * 1000);

    return () => clearInterval(timer);
  }, [auth]);

  // Global 401/403 listener
  useEffect(() => {
    function handleAuthError(event: Event) {
      const customEvent = event as CustomEvent<{ status: number; path: string }>;
      const requestPath = customEvent.detail?.path ?? "";

      if (requestPath.startsWith("/api/v1/auth/login") || requestPath.startsWith("/api/v1/auth/register")) {
        return;
      }

      setAuth(null);
    }

    window.addEventListener("erp:auth-error", handleAuthError as EventListener);
    return () => window.removeEventListener("erp:auth-error", handleAuthError as EventListener);
  }, []);

  const value = useMemo<AuthContextValue>(
    () => ({
      auth,
      isAuthenticated: auth !== null,
      login,
      register,
      logout,
      isRestoring
    }),
    [auth, isRestoring]
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
