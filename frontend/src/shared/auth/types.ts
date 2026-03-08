export interface AuthUser {
  id: number;
  email: string;
  fullName: string;
}

export interface LoginResult {
  accessToken: string;
  expiresAtUtc: string;
  user: AuthUser;
}

export interface AuthState {
  token: string;
  user: AuthUser;
  expiresAtUtc: string;
}
