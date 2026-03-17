import type { DoctorSearchDTO } from "../types/DoctorSearchDTO";
import type { DoctorDTO } from "../types/DoctorDTO";
import type { DoctorCreateDTO } from "../types/DoctorCreateDTO";
import { getAuthHeaders } from "./authHeaders";
import { fetchJson, friendlyStatusError } from "./apiHelpers";

const API_BASE: string = import.meta.env.VITE_API_BASE_URL as string;

// Search doctors
export async function searchDoctors(query: string): Promise<DoctorSearchDTO[]> {
  const { data: result, ok, status } = await fetchJson<DoctorSearchDTO[] & { message?: string }>(
    `${API_BASE}/api/Doctor/search?query=${encodeURIComponent(query)}`
  );

  if (!ok) {
    throw new Error(result.message ?? friendlyStatusError(status));
  }

  return result;
}

// All doctors
export async function getDoctors(): Promise<DoctorDTO[]> {
  const { data: result, ok, status } = await fetchJson<DoctorDTO[] & { message?: string }>(
    `${API_BASE}/api/Doctor`
  );

  if (!ok) {
    throw new Error(result.message ?? friendlyStatusError(status));
  }

  return result;
}

// Doctors by clinic
export async function getDoctorsByClinic(clinicId: number): Promise<DoctorDTO[]> {
  const { data: result, ok, status } = await fetchJson<DoctorDTO[] & { message?: string }>(
    `${API_BASE}/api/Doctor/by-clinic/${String(clinicId)}`,
    { headers: getAuthHeaders() }
  );

  if (!ok) {
    throw new Error(result.message ?? friendlyStatusError(status));
  }

  return result;
}

// Admin CRUD
export async function createDoctor(data: DoctorCreateDTO) {
  const { data: result, ok, status } = await fetchJson<DoctorDTO & { message?: string }>(
    `${API_BASE}/api/Doctor`,
    {
      method: "POST",
      headers: getAuthHeaders(),
      body: JSON.stringify(data),
    }
  );

  if (!ok) {
    throw new Error(result.message ?? friendlyStatusError(status));
  }

  return result;
}

export async function updateDoctor(id: number, data: DoctorCreateDTO) {
  const { data: result, ok, status } = await fetchJson<{ message?: string }>(
    `${API_BASE}/api/Doctor/${String(id)}`,
    {
      method: "PUT",
      headers: getAuthHeaders(),
      body: JSON.stringify(data),
    }
  );

  if (!ok) {
    throw new Error(result.message ?? friendlyStatusError(status));
  }

  return result;
}

export async function deleteDoctor(id: number) {
  const { data: result, ok, status } = await fetchJson<{ message?: string }>(
    `${API_BASE}/api/Doctor/${String(id)}`,
    {
      method: "DELETE",
      headers: getAuthHeaders(),
    }
  );

  if (!ok) {
    throw new Error(result.message ?? friendlyStatusError(status));
  }

  return result;
}
