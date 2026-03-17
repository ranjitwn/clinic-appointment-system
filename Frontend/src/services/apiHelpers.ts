/**
 * Maps HTTP status codes to user-friendly messages.
 * Used when the API doesn't include a message field in the response.
 */
export function friendlyStatusError(status: number): string {
  if (status === 401) return "Your session has expired. Please log in again.";
  if (status === 403) return "You don't have permission to perform this action.";
  if (status >= 500) return "Something went wrong on our end. Please try again later.";
  return "An unexpected error occurred. Please try again.";
}

/**
 * Fetch wrapper that handles:
 * - Network failures (server down, no internet) → friendly message instead of raw TypeError
 * - Non-JSON responses (502/504 HTML error pages) → status-based message instead of SyntaxError
 */
export async function fetchJson<T>(
  url: string,
  options?: RequestInit
): Promise<{ data: T; ok: boolean; status: number }> {
  let response: Response;

  try {
    response = await fetch(url, options);
  } catch {
    throw new Error("Unable to connect. Please check your internet connection.");
  }

  let data: T;
  try {
    data = (await response.json()) as T;
  } catch {
    throw new Error(friendlyStatusError(response.status));
  }

  return { data, ok: response.ok, status: response.status };
}
