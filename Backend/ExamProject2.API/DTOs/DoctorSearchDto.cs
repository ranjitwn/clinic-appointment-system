namespace ExamProject2.API.DTOs
{
    #nullable disable
    public class DoctorSearchDto
    {
        public int DoctorId { get; set; }
        public string FullName { get; set; }
        public string Clinic { get; set; }
        public string Speciality { get; set; }
    }
}
