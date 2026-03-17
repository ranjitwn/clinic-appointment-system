import type { AppointmentCreateDTO } from "../types/AppointmentCreateDTO";
import type { AppointmentUpdateDTO } from "../types/AppointmentUpdateDTO";
import type { AppointmentDTO } from "../types/AppointmentDTO";
import { getAuthHeaders } from "./authHeaders";
import { fetchJson, friendlyStatusError } from "./apiHelpers";

const API_BASE: string = import.meta.env.VITE_API_BASE_URL as string;

// Get logged-in user appointments
export async function getMyAppointments(): Promise<AppointmentDTO[]> {
  const { data: result, ok, status } = await fetchJson<AppointmentDTO[] & { message?: string }>(
    `${API_BASE}/api/Appointment/my`,
    { headers: getAuthHeaders() }
  );

  if (!ok) {
    throw new Error(result.message ?? friendlyStatusError(status));
  }

  return result;
}

// Admin/overview: appointments by clinic
export async function getAppointmentsByClinic(clinicId: number): Promise<AppointmentDTO[]> {
  const { data: result, ok, status } = await fetchJson<AppointmentDTO[] & { message?: string }>(
    `${API_BASE}/api/Appointment/clinic/${String(clinicId)}`,
    { headers: getAuthHeaders() }
  );

  if (!ok) {
    throw new Error(result.message ?? friendlyStatusError(status));
  }

  return result;
}

// Admin/overview: appointments by doctor
export async function getAppointmentsByDoctor(doctorId: number): Promise<AppointmentDTO[]> {
  const { data: result, ok, status } = await fetchJson<AppointmentDTO[] & { message?: string }>(
    `${API_BASE}/api/Appointment/doctor/${String(doctorId)}`,
    { headers: getAuthHeaders() }
  );

  if (!ok) {
    throw new Error(result.message ?? friendlyStatusError(status));
  }

  return result;
}

// Fetch available appointment slots
export async function getAvailableSlots(
  doctorId: number,
  date: string,
  duration: number
): Promise<string[]> {
  const url = `${API_BASE}/api/Appointment/available-slots?doctorId=${String(doctorId)}&date=${date}&duration=${String(duration)}`;
  const { data: result, ok, status } = await fetchJson<{ availableSlots: string[] } | { message?: string }>(
    url,
    { headers: getAuthHeaders() }
  );

  if (!ok) {
    throw new Error(
      "message" in result && result.message
        ? result.message
        : friendlyStatusError(status)
    );
  }

  return "availableSlots" in result ? result.availableSlots : [];
}

// Book appointment (guest OR logged-in)
export async function bookAppointment(data: AppointmentCreateDTO) {
  const { data: result, ok, status } = await fetchJson<{ message?: string }>(
    `${API_BASE}/api/Appointment`,
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

// Update appointment (logged-in only)
export async function updateAppointment(id: number, data: AppointmentUpdateDTO) {
  const { data: result, ok, status } = await fetchJson<{ message?: string }>(
    `${API_BASE}/api/Appointment/${String(id)}`,
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

// Cancel appointment (logged-in only)
export async function deleteAppointment(id: number) {
  const { data: result, ok, status } = await fetchJson<{ message?: string }>(
    `${API_BASE}/api/Appointment/${String(id)}`,
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
