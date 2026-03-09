export interface AuthUser {
  id: number;
  email: string;
  fullName: string;
  role: string;
}

export interface AuthResponse {
  expiresAtUtc: string;
  user: AuthUser;
}

export interface AuthState {
  user: AuthUser;
  expiresAtUtc: string;
}
