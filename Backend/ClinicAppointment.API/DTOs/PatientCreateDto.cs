using System.ComponentModel.DataAnnotations;

namespace ClinicAppointment.API.DTOs
{
    public class PatientCreateDto
    {
        [Required]
        public required string FirstName { get; set; }

        [Required]
        public required string LastName { get; set; }

        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        [Required]
        public DateTime DateOfBirth { get; set; }

        public string? Gender { get; set; }

    }
}
