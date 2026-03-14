namespace ClinicAppointment.API.DTOs
{
    public class AppointmentDto
    {
        public int Id { get; set; }
        public DateTime AppointmentDate { get; set; }
        public int DurationMinutes { get; set; }

        public int ClinicId { get; set; }
        public required string ClinicName { get; set; }

        public int DoctorId { get; set; }
        public required string DoctorName { get; set; }

        public int CategoryId { get; set; }
        public required string CategoryName { get; set; }

        public int? PatientId { get; set; }
    }
}
