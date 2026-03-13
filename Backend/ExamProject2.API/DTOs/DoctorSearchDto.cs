namespace ExamProject2.API.DTOs
{
    public class DoctorSearchDto
    {
        public int DoctorId { get; set; }
        public required string FullName { get; set; }
        public required string Clinic { get; set; }
        public required string Speciality { get; set; }
    }
}
