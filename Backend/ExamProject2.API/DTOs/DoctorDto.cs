#nullable disable
namespace ExamProject2.API.DTOs
{
    public class DoctorDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int ClinicId { get; set; }
        public int SpecialityId { get; set; }
    }
}
