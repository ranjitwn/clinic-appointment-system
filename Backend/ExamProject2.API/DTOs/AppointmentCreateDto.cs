using System.ComponentModel.DataAnnotations;

namespace ExamProject2.API.DTOs
{
    public class AppointmentCreateDto
    {
        [Required]
        public DateTime AppointmentDate { get; set; }

        public int? DurationMinutes { get; set; } = 30;

        [Required]
        public int ClinicId { get; set; }

        [Required]
        public int DoctorId { get; set; }
        
        [Required]
        public int CategoryId { get; set; }

        // Guest booking support
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }

        [Required]
        public DateTime DateOfBirth { get; set; }
        }
}
