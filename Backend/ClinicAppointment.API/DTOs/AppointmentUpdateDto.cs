using System.ComponentModel.DataAnnotations;

namespace ClinicAppointment.API.DTOs
{
    public class AppointmentUpdateDto
    {
        [Required]
        public DateTime AppointmentDate { get; set; }

        [Required]
        [Range(15, 60, ErrorMessage = "Duration must be 15, 30, 45, or 60 minutes.")]
        public int DurationMinutes { get; set; }
    }
}
