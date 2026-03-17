using ClinicAppointment.API.DTOs;

namespace ClinicAppointment.API.Services
{
    public interface IClinicService
    {
        Task<List<ClinicDto>> GetClinicsAsync();
        Task<ClinicDto?> CreateClinicAsync(ClinicCreateDto dto);
        Task<ClinicDto?> GetClinicAsync(int id);
        Task<bool> UpdateClinicAsync(int id, ClinicCreateDto dto);
        Task<bool?> DeleteClinicAsync(int id);
    }
}
