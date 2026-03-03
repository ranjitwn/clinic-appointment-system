import type { AppointmentCreateDTO } from "../types/AppointmentCreateDTO";
import type { AppointmentUpdateDTO } from "../types/AppointmentUpdateDTO";
import type { AppointmentDTO } from "../types/AppointmentDTO";
import { getAuthHeaders } from "./authHeaders";

const API_BASE: string = import.meta.env.VITE_API_BASE_URL as string;

// Get logged-in user appointments
export async function getMyAppointments(): Promise<AppointmentDTO[]> {
  const response = await fetch(`${API_BASE}/api/Appointment/my`, {
    headers: getAuthHeaders(),
  });

  const result = (await response.json()) as AppointmentDTO[] & {
    message?: string;
  };

  if (!response.ok) {
    throw new Error(result.message ?? "Failed to fetch appointments");
  }

  return result;
}

// Admin/overview: appointments by clinic
export async function getAppointmentsByClinic(clinicId: number): Promise<AppointmentDTO[]> {
  const response = await fetch(`${API_BASE}/api/Appointment/clinic/${String(clinicId)}`, {
    headers: getAuthHeaders(),
  });

  const result = (await response.json()) as AppointmentDTO[] & {
    message?: string;
  };

  if (!response.ok) {
    throw new Error(result.message ?? "Failed to fetch clinic appointments");
  }

  return result;
}

// Admin/overview: appointments by doctor
export async function getAppointmentsByDoctor(doctorId: number): Promise<AppointmentDTO[]> {
  const response = await fetch(`${API_BASE}/api/Appointment/doctor/${String(doctorId)}`, {
    headers: getAuthHeaders(),
  });

  const result = (await response.json()) as AppointmentDTO[] & {
    message?: string;
  };

  if (!response.ok) {
    throw new Error(result.message ?? "Failed to fetch doctor appointments");
  }

  return result;
}

// Fetch available appointment slots
export async function getAvailableSlots(
  doctorId: number,
  date: string,
  duration: number
): Promise<string[]> {
  const response = await fetch(
    `${API_BASE}/api/Appointment/available-slots?doctorId=${String(
      doctorId
    )}&date=${date}&duration=${String(duration)}`,
    {
      headers: getAuthHeaders(),
    }
  );

  const result = (await response.json()) as
    | { availableSlots: string[] }
    | { message?: string };

  if (!response.ok) {
    throw new Error(
      "message" in result && result.message
        ? result.message
        : "Failed to load available slots"
    );
  }

  return "availableSlots" in result ? result.availableSlots : [];
}


// Book appointment (guest OR logged-in)
export async function bookAppointment(data: AppointmentCreateDTO) {
  const response = await fetch(`${API_BASE}/api/Appointment`, {
    method: "POST",
    headers: getAuthHeaders(),
    body: JSON.stringify(data),
  });

  const result = (await response.json()) as { message?: string };

  if (!response.ok) {
    throw new Error(result.message ?? "Booking failed");
  }

  return result;
}

// Update appointment (logged-in only)
export async function updateAppointment(
  id: number,
  data: AppointmentUpdateDTO
  ) {
  const response = await fetch(`${API_BASE}/api/Appointment/${String(id)}`, {
    method: "PUT",
    headers: getAuthHeaders(),
    body: JSON.stringify(data),
  });

   const result = (await response.json()) as { message?: string };

  if (!response.ok) {
    throw new Error(result.message ?? "Update failed");
  }

  return result;
}

// Cancel appointment (logged-in only)
export async function deleteAppointment(id: number) {
  const response = await fetch(`${API_BASE}/api/Appointment/${String(id)}`, {
    method: "DELETE",
    headers: getAuthHeaders(),
  });

  const result = (await response.json()) as { message?: string };

  if (!response.ok) {
    throw new Error(result.message ?? "Failed to delete appointment");
  }

  return result;
}


