import type { LoginDTO } from "../types/PatientLoginDTO";
import type { RegisterDTO } from "../types/PatientRegisterDTO";

const API_BASE: string = import.meta.env.VITE_API_BASE_URL as string;

// LOGIN
export async function loginUser(data: LoginDTO) {
  const response = await fetch(`${API_BASE}/api/Auth/login`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify(data),
  });

  const result = (await response.json()) as {
    token?: string;
    role?: string;
    message?: string;
  };

  if (!response.ok) {
    throw new Error(result.message ?? "Login failed");
  }

  return result;
}


// REGISTER
export async function registerUser(data: RegisterDTO) {
  const response = await fetch(`${API_BASE}/api/Auth/register`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify(data),
  });

  const result = (await response.json()) as {
    message?: string;
  };

  if (!response.ok) {
    throw new Error(result.message ?? "Registration failed");
  }

  return result;
}