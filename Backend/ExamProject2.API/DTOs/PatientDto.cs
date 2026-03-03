namespace ExamProject2.API.DTOs
{
    public class PatientDto
    {
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public DateTime DateOfBirth { get; set; }

        public string? Gender { get; set; }

        public bool IsRegistered { get; set; }

        public string Role { get; set; }
    }
}
