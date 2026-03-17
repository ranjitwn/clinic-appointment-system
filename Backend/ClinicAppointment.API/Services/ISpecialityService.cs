using ClinicAppointment.API.DTOs;

namespace ClinicAppointment.API.Services
{
    public interface ISpecialityService
    {
        Task<List<SpecialityDto>> GetSpecialitiesAsync();
        Task<SpecialityDto?> CreateSpecialityAsync(SpecialityCreateDto dto);
        Task<SpecialityDto?> GetSpecialityAsync(int id);
        Task<bool> UpdateSpecialityAsync(int id, SpecialityCreateDto dto);
        Task<bool?> DeleteSpecialityAsync(int id);
    }
}
