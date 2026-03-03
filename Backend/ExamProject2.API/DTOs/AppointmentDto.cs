namespace ExamProject2.API.DTOs
{
    #nullable disable
    public class AppointmentDto
    {
        public int Id { get; set; }
        public DateTime AppointmentDate { get; set; }
        public int DurationMinutes { get; set; }

        public int ClinicId { get; set; }
        public string ClinicName { get; set; }

        public int DoctorId { get; set; }
        public string DoctorName { get; set; }

        public int CategoryId { get; set; }
        public string CategoryName { get; set; }

        public int? PatientId { get; set; }
    }
}
