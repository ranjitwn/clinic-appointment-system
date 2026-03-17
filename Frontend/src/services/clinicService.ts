import type { ClinicDTO } from "../types/ClinicDTO";
import type { ClinicCreateDTO } from "../types/ClinicCreateDTO";
import { getAuthHeaders } from "./authHeaders";
import { fetchJson, friendlyStatusError } from "./apiHelpers";

const API_BASE: string = import.meta.env.VITE_API_BASE_URL as string;

export async function getClinics(): Promise<ClinicDTO[]> {
  const { data: result, ok, status } = await fetchJson<ClinicDTO[] & { message?: string }>(
    `${API_BASE}/api/Clinic`
  );

  if (!ok) {
    throw new Error(result.message ?? friendlyStatusError(status));
  }

  return result;
}

export async function createClinic(data: ClinicCreateDTO) {
  const { data: result, ok, status } = await fetchJson<ClinicDTO & { message?: string }>(
    `${API_BASE}/api/Clinic`,
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

export async function updateClinic(id: number, data: ClinicCreateDTO) {
  const { data: result, ok, status } = await fetchJson<{ message?: string }>(
    `${API_BASE}/api/Clinic/${String(id)}`,
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

export async function deleteClinic(id: number) {
  const { data: result, ok, status } = await fetchJson<{ message?: string }>(
    `${API_BASE}/api/Clinic/${String(id)}`,
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
