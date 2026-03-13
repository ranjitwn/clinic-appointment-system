namespace ExamProject2.API.DTOs
{
    public class DoctorDto
    {
        public int Id { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public int ClinicId { get; set; }
        public int SpecialityId { get; set; }
    }
}
