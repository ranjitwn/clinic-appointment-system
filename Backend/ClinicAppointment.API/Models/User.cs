namespace ClinicAppointment.API.Models
{
    public class User
    {
        public int Id { get; set; }
        public required string Email { get; set; }
        public string Password { get; set; } = string.Empty;
        public string Role { get; set; } = "Admin";
    }
}
