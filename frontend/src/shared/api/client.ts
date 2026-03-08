export const API_BASE_URL = "http://localhost:5000";

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
    throw new Error(errorText || `Request failed with status ${response.status}`);
  }

  return response.json() as Promise<T>;
}
