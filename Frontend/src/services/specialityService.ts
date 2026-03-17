import type { SpecialityDTO } from "../types/SpecialityDTO";
import type { SpecialityCreateDTO } from "../types/SpecialityCreateDTO";
import { getAuthHeaders } from "./authHeaders";
import { fetchJson, friendlyStatusError } from "./apiHelpers";

const API_BASE: string = import.meta.env.VITE_API_BASE_URL as string;

export async function getSpecialities(): Promise<SpecialityDTO[]> {
  const { data: result, ok, status } = await fetchJson<SpecialityDTO[] & { message?: string }>(
    `${API_BASE}/api/Speciality`
  );

  if (!ok) {
    throw new Error(result.message ?? friendlyStatusError(status));
  }

  return result;
}

// Admin CRUD

export async function createSpeciality(data: SpecialityCreateDTO) {
  const { data: result, ok, status } = await fetchJson<SpecialityDTO & { message?: string }>(
    `${API_BASE}/api/Speciality`,
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

export async function updateSpeciality(id: number, data: SpecialityCreateDTO) {
  const { data: result, ok, status } = await fetchJson<{ message?: string }>(
    `${API_BASE}/api/Speciality/${String(id)}`,
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

export async function deleteSpeciality(id: number) {
  const { data: result, ok, status } = await fetchJson<{ message?: string }>(
    `${API_BASE}/api/Speciality/${String(id)}`,
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
