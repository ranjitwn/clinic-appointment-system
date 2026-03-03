import type { SpecialityDTO } from "../types/SpecialityDTO";
import type { SpecialityCreateDTO } from "../types/SpecialityCreateDTO";
import { getAuthHeaders } from "./authHeaders";

const API_BASE: string = import.meta.env.VITE_API_BASE_URL as string;

export async function getSpecialities(): Promise<SpecialityDTO[]> {
  const response = await fetch(`${API_BASE}/api/Speciality`);

  const result = (await response.json()) as SpecialityDTO[] & {
    message?: string;
  };

  if (!response.ok) {
    throw new Error(result.message ?? "Failed to fetch specialities");
  }

  return result;
}

// Admin CRUD 

export async function createSpeciality(data: SpecialityCreateDTO) {
  const response = await fetch(`${API_BASE}/api/Speciality`, {
    method: "POST",
    headers: getAuthHeaders(),
    body: JSON.stringify(data),
  });

  const result = (await response.json()) as SpecialityDTO & {
    message?: string;
  };

  if (!response.ok) {
    throw new Error(result.message ?? "Failed to create speciality");
  }

  return result;
}

export async function updateSpeciality(id: number, data: SpecialityCreateDTO) {
  const response = await fetch(`${API_BASE}/api/Speciality/${String(id)}`, {
    method: "PUT",
    headers: getAuthHeaders(),
    body: JSON.stringify(data),
  });

  const result = (await response.json()) as { message?: string };

  if (!response.ok) {
    throw new Error(result.message ?? "Failed to update speciality");
  }

  return result;
}

export async function deleteSpeciality(id: number) {
  const response = await fetch(`${API_BASE}/api/Speciality/${String(id)}`, {
    method: "DELETE",
    headers: getAuthHeaders(),
  });

  const result = (await response.json()) as { message?: string };

  if (!response.ok) {
    throw new Error(result.message ?? "Failed to delete speciality");
  }

  return result;
}
