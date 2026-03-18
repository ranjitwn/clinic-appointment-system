using ClinicAppointment.API.Constants;

namespace ClinicAppointment.API.Models
{
    public class Patient
    {
        public int Id { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public DateTime DateOfBirth { get; set; }
        public bool IsRegistered { get; set; }
        public string? Password { get; set; } // Patient-based authentication (null = guest, set = registered)

        // Role field for authorization
        public required string Role { get; set; } = Roles.Patient;

        // Optional field for registered patients

        public string? Gender { get; set; }

        public ICollection<Appointment>? Appointments { get; set; }

    }
}
