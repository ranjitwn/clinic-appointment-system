import type { LoginDTO } from "../types/PatientLoginDTO";
import type { RegisterDTO } from "../types/PatientRegisterDTO";
import { fetchJson, friendlyStatusError } from "./apiHelpers";

const API_BASE: string = import.meta.env.VITE_API_BASE_URL as string;

// LOGIN
export async function loginUser(data: LoginDTO) {
  const { data: result, ok, status } = await fetchJson<{
    token?: string;
    role?: string;
    message?: string;
  }>(
    `${API_BASE}/api/Auth/login`,
    {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(data),
    }
  );

  if (!ok) {
    throw new Error(result.message ?? friendlyStatusError(status));
  }

  return result;
}

// REGISTER
export async function registerUser(data: RegisterDTO) {
  const { data: result, ok, status } = await fetchJson<{ message?: string }>(
    `${API_BASE}/api/Auth/register`,
    {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(data),
    }
  );

  if (!ok) {
    throw new Error(result.message ?? friendlyStatusError(status));
  }

  return result;
}
