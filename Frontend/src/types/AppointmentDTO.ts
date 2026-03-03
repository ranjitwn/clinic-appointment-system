export interface AppointmentDTO {
  id: number;
  appointmentDate: string;
  durationMinutes: number;

  clinicId: number;
  clinicName: string;

  doctorId: number;
  doctorName: string;

  categoryId: number;
  categoryName: string;

  patientId: number | null;
}
