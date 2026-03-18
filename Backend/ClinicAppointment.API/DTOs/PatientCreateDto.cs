using System.ComponentModel.DataAnnotations;

namespace ClinicAppointment.API.DTOs
{
    public class PatientCreateDto
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

        public DateTime DateOfBirth { get; set; }

        [StringLength(20)]
        public string? Gender { get; set; }

    }
}
