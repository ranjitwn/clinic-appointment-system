using System.ComponentModel.DataAnnotations;

namespace ClinicAppointment.API.DTOs
{
    public class AppointmentCreateDto
    {
        [Required]
        public DateTime AppointmentDate { get; set; }

        [Range(15, 60, ErrorMessage = "Duration must be between 15 and 60 minutes.")]
        public int? DurationMinutes { get; set; } = 30;

        [Range(1, int.MaxValue, ErrorMessage = "A valid ClinicId is required.")]
        public int ClinicId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "A valid DoctorId is required.")]
        public int DoctorId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "A valid CategoryId is required.")]
        public int CategoryId { get; set; }

        [StringLength(100)]
        public string? FirstName { get; set; }

        [StringLength(100)]
        public string? LastName { get; set; }

        [EmailAddress]
        [StringLength(256)]
        public string? Email { get; set; }

        public DateTime DateOfBirth { get; set; }
    }
}
