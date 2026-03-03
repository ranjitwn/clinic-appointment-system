import type { DoctorSearchDTO } from "../types/DoctorSearchDTO";
import type { DoctorDTO } from "../types/DoctorDTO";
import type { DoctorCreateDTO } from "../types/DoctorCreateDTO";
import { getAuthHeaders } from "./authHeaders";

const API_BASE: string = import.meta.env.VITE_API_BASE_URL as string;

// Search doctors 
export async function searchDoctors(query: string): Promise<DoctorSearchDTO[]> {
  const response = await fetch(
    `${API_BASE}/api/Doctor/search?query=${encodeURIComponent(query)}`
  );

  const result = (await response.json()) as DoctorSearchDTO[] & {
    message?: string;
  };

  if (!response.ok) {
    throw new Error(result.message ?? "Failed to fetch doctors");
  }

  return result;
}

// All doctors 
export async function getDoctors(): Promise<DoctorDTO[]> {
  const response = await fetch(`${API_BASE}/api/Doctor`);

  const result = (await response.json()) as DoctorDTO[] & {
    message?: string;
  };

  if (!response.ok) {
    throw new Error(result.message ?? "Failed to fetch doctors");
  }

  return result;
}

// Doctors by clinic 
export async function getDoctorsByClinic(clinicId: number): Promise<DoctorDTO[]> {
  const response = await fetch(
    `${API_BASE}/api/Doctor/by-clinic/${String(clinicId)}`,
    {
      headers: getAuthHeaders(),
    }
  );

  const result = (await response.json()) as DoctorDTO[] & {
    message?: string;
  };

  if (!response.ok) {
    throw new Error(result.message ?? "Failed to fetch doctors");
  }

  return result;
}

// Admin CRUD
export async function createDoctor(data: DoctorCreateDTO) {
  const response = await fetch(`${API_BASE}/api/Doctor`, {
    method: "POST",
    headers: getAuthHeaders(),
    body: JSON.stringify(data),
  });

  const result = (await response.json()) as DoctorDTO & {
    message?: string;
  };

  if (!response.ok) {
    throw new Error(result.message ?? "Failed to create doctor");
  }

  return result;
}


export async function updateDoctor(id: number, data: DoctorCreateDTO) {
  const response = await fetch(`${API_BASE}/api/Doctor/${String(id)}`, {
    method: "PUT",
    headers: getAuthHeaders(),
    body: JSON.stringify(data),
  });

  const result = (await response.json()) as { message?: string };

  if (!response.ok) {
    throw new Error(result.message ?? "Failed to update doctor");
  }

  return result;
}


export async function deleteDoctor(id: number) {
  const response = await fetch(`${API_BASE}/api/Doctor/${String(id)}`, {
    method: "DELETE",
    headers: getAuthHeaders(),
  });

  const result = (await response.json()) as { message?: string };

  if (!response.ok) {
    throw new Error(result.message ?? "Failed to delete doctor");
  }

  return result;
}