using System.ComponentModel.DataAnnotations;

namespace ClinicAppointment.API.DTOs
{
    public class PatientRegisterDto
    {
        [Required]
        [StringLength(100)]
        public required string FirstName { get; set; }

        [Required]
        [StringLength(100)]
        public required string LastName { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(256)]
        public required string Email { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6)]
        public required string Password { get; set; }

        [Required]
        public DateTime DateOfBirth { get; set; }

        // Additional data for registered patients
        public string? Gender { get; set; }
    }
}
