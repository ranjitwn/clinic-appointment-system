import type { ClinicDTO } from "../types/ClinicDTO";
import type { ClinicCreateDTO } from "../types/ClinicCreateDTO";
import { getAuthHeaders } from "./authHeaders";

const API_BASE: string = import.meta.env.VITE_API_BASE_URL as string;

export async function getClinics(): Promise<ClinicDTO[]> {
  const response = await fetch(`${API_BASE}/api/Clinic`);

  const result = (await response.json()) as ClinicDTO[] & {
    message?: string;
  };

  if (!response.ok) {
    throw new Error(result.message ?? "Failed to fetch clinics");
  }

  return result;
}

export async function createClinic(data: ClinicCreateDTO) {
  const response = await fetch(`${API_BASE}/api/Clinic`, {
    method: "POST",
    headers: getAuthHeaders(),
    body: JSON.stringify(data),
  });

  const result = (await response.json()) as ClinicDTO & {
    message?: string;
  };

  if (!response.ok) {
    throw new Error(result.message ?? "Failed to create clinic");
  }

  return result;
}


export async function updateClinic(id: number, data: ClinicCreateDTO) {
  const response = await fetch(`${API_BASE}/api/Clinic/${String(id)}`, {
    method: "PUT",
    headers: getAuthHeaders(),
    body: JSON.stringify(data),
  });

  const result = (await response.json()) as { message?: string };

  if (!response.ok) {
    throw new Error(result.message ?? "Failed to update clinic");
  }

  return result;
}


export async function deleteClinic(id: number) {
  const response = await fetch(`${API_BASE}/api/Clinic/${String(id)}`, {
    method: "DELETE",
    headers: getAuthHeaders(),
  });

  const result = (await response.json()) as { message?: string };

  if (!response.ok) {
    throw new Error(result.message ?? "Failed to delete clinic");
  }

  return result;
}