# Sprint 0.3 Status - Auth Hardening and Gap Closure

**Date:** March 9, 2026  
**Sprint:** Phase 0, Sprint 0.3  
**Status:** ✅ Complete

---

## Sprint Goals

Close critical gaps found in Sprint 0.2 review:
- Add backend logout endpoint
- Add policy-based authorization setup
- Add tenant/company context middleware baseline
- Add global 401/403 handling on frontend
- Resolve broken profile navigation path
- Align implementation and status documentation
- **Security hardening based on auth vulnerability audit**

---

## Tasks

### Backend (Sprint 0.3a — Functional Gaps)
- [x] Add logout endpoint to authentication controller
- [x] Add logout contract in auth service
- [x] Add policy-based authorization setup in API startup
- [x] Migrate admin endpoint to named authorization policy
- [x] Add tenant context middleware baseline

### Frontend (Sprint 0.3a — Functional Gaps)
- [x] Add global auth error dispatch on 401/403 from API client
- [x] Add global auth-error listener in auth context
- [x] Implement logout API call before local session cleanup
- [x] Add Profile page and route for header navigation

### Backend (Sprint 0.3b — Security Hardening)
- [x] (#4) Set JWT in httpOnly cookie on login/register/refresh; configure JWT Bearer to read from cookie
- [x] (#4) Add `GET /api/v1/auth/me` endpoint for session restoration from cookie
- [x] (#4) Clear httpOnly cookie on logout
- [x] (#5) Create `Role` entity and `roles` table with seeded Admin/User roles
- [x] (#5) Add `RoleId` FK to `User` entity; resolve role from DB join instead of email convention
- [x] (#6) Create `RevokedToken` entity and `revoked_tokens` table with JTI index
- [x] (#6) Implement `ITokenRevocationService` / `TokenRevocationService`
- [x] (#6) Add `TokenRevocationMiddleware` to reject revoked tokens on every request
- [x] (#6) Revoke current token JTI on logout
- [x] (#7) Move `TenantContextMiddleware` after authentication; derive tenant from user claims, not client headers
- [x] (#8) Tighten CORS: restrict to specific methods (`GET, POST, PUT, PATCH, DELETE, OPTIONS`), specific headers (`Content-Type, Authorization, Accept`), and `AllowCredentials()`
- [x] (#9) Seed default admin with `MustChangePassword = true`; log password-change-required message
- [x] (#10) Reduce JWT access token expiry from 120 min to 15 min
- [x] (#11) Add `jti` (unique token ID) claim to every issued JWT
- [x] (#12) Add `SecurityHeadersMiddleware` — `X-Content-Type-Options`, `X-Frame-Options`, `Referrer-Policy`, `Permissions-Policy`, `X-Permitted-Cross-Domain-Policies`
- [x] (#16) Align `LoginRequestValidator` password minimum length to 8 (was 6), matching `RegisterRequestValidator`
- [x] Create EF Core migration `AddRolesAndTokenRevocation` with inline role seeding and existing user assignment

### Frontend (Sprint 0.3b — Security Hardening)
- [x] (#4) Remove `localStorage` token storage entirely; JWT is now in httpOnly cookie
- [x] (#4) Use `credentials: "include"` on all `apiFetch` calls so browser sends cookie automatically
- [x] (#4) Remove `Authorization` header construction and `token` parameter from `apiFetch`
- [x] (#4) Restore session on page load via `GET /api/v1/auth/me` + `POST /api/v1/auth/refresh`
- [x] (#4) Add `isRestoring` state in `AuthProvider`; `ProtectedRoute` waits for session restoration
- [x] (#9) Remove pre-filled demo credentials from LoginPage
- [x] (#14) Auth state validity is now based on in-memory expiry (no stale storage reads)
- [x] Update `AuthState` type: removed `token` field; added `role` to `AuthUser`

### Validation
- [x] Build backend and frontend
- [x] Smoke test login, refresh, logout endpoints
- [x] Verify admin policy endpoint behavior

---

## Security Audit Findings — Out of Scope (Tracked for Future Sprints)

| # | Finding | Severity | Reason Deferred |
|---|---------|----------|-----------------|
| 1 | JWT secret hardcoded in `appsettings.json` and source code | Critical | Requires secrets management infra (Key Vault / env var injection in CI/CD) |
| 2 | No rate limiting or CAPTCHA on login/register endpoints | Critical | Requires rate-limiting middleware or external service (e.g. `AspNetCoreRateLimit`, API gateway) |
| 3 | No brute-force protection / account lockout on login | Critical | Requires account lockout policy and failed-attempt tracking |
| 13 | Database credentials in `appsettings.json` | Medium | Same as #1 — requires secrets management infra |
| 15 | User enumeration via register endpoint distinct error message | Low | Requires email verification flow to fully address |

---

## Notes

- Logout now revokes the current JWT by persisting its JTI to the `revoked_tokens` table; `TokenRevocationMiddleware` rejects revoked tokens.
- Tenant middleware now runs after authentication and derives tenant context from user claims, not from client-supplied headers.
- Token no longer sent in API response body; stored exclusively in httpOnly cookie (`erp_access_token`).
- Frontend session restoration on page reload: calls `/api/v1/auth/me` → `/api/v1/auth/refresh` on mount.
- `MustChangePassword` flag is set on the seeded admin user; enforcement UI is deferred but the data contract is in place.
- Security headers middleware runs before CORS/auth in the pipeline.

---

## Progress

**Overall:** 100%  
**Backend:** 100%  
**Frontend:** 100%  
**Validation:** 100%
