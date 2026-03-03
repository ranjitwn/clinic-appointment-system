namespace ExamProject2.API.Models
{
    public class Patient
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public DateTime DateOfBirth { get; set; }
        public bool IsRegistered { get; set; }
        public string? Password { get; set; } // Patient-based authentication (null = guest, set = registered)

        // Role field for authorization
        public string Role { get; set; } = "Patient";

        // Optional field for registered patients
        public string? Gender { get; set; }

        public ICollection<Appointment>? Appointments { get; set; }
        
    }
}
