namespace ExamProject2.API.Models
{
    public class Appointment
    {
        public int Id { get; set; }
        public DateTime AppointmentDate { get; set; }
        public int DurationMinutes { get; set; }

        public int ClinicId { get; set; }
        public Clinic? Clinic { get; set; }

        public int DoctorId { get; set; }
        public Doctor? Doctor { get; set; }

        public int PatientId { get; set; }
        public Patient? Patient { get; set; }

        public int CategoryId { get; set; }
        public Category? Category { get; set; }
    }
}
