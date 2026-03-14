namespace ClinicAppointment.API.DTOs
{
    public class AuthLoginResultDto
    {
        public required string Token { get; set; }
        public required string Role { get; set; }
    }
}