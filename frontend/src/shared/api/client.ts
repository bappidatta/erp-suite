export const API_BASE_URL = "http://localhost:5000";

export class ApiError extends Error {
  readonly status: number;

  constructor(status: number, message: string) {
    super(message);
    this.name = "ApiError";
    this.status = status;
  }
}

export async function apiFetch<T>(path: string, options: RequestInit = {}, token?: string): Promise<T> {
  let headers: HeadersInit = {
    "Content-Type": "application/json",
    ...(options.headers ?? {})
  };

  if (token) {
    headers = {
      ...headers,
      Authorization: `Bearer ${token}`
    };
  }

  const response = await fetch(`${API_BASE_URL}${path}`, {
    ...options,
    headers
  });

  if (!response.ok) {
    const errorText = await response.text();
    const message = errorText || `Request failed with status ${response.status}`;

    if ((response.status === 401 || response.status === 403) && typeof window !== "undefined") {
      window.dispatchEvent(
        new CustomEvent("erp:auth-error", {
          detail: {
            status: response.status,
            path
          }
        })
      );
    }

    throw new ApiError(response.status, message);
  }

  return response.json() as Promise<T>;
}
