using ClinicAppointment.API.DTOs;

namespace ClinicAppointment.API.Services
{
    public interface IAuthService
    {
        Task<string?> RegisterAsync(PatientRegisterDto dto);
        Task<AuthLoginResultDto?> LoginAsync(PatientLoginDto dto);
    }
}
