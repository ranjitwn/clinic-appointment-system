namespace ExamProject2.API.DTOs
{
    public class PatientDto
    {
        public int Id { get; set; }

        public required string FirstName { get; set; }

        public required string LastName { get; set; }

        public required string Email { get; set; }

        public DateTime DateOfBirth { get; set; }

        public string? Gender { get; set; }

        public bool IsRegistered { get; set; }

        public required string Role { get; set; }
    }
}
