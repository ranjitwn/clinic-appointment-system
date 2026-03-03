export interface AppointmentCreateDTO {
  appointmentDate: string;   // DateTime → ISO string
  durationMinutes: number;
  clinicId: number;
  doctorId: number;
  categoryId: number;

  // For registered patient booking
  patientId?: number;

  // Guest booking fields 
  firstName?: string;
  lastName?: string;
  email?: string;
  dateOfBirth?: string;
}
