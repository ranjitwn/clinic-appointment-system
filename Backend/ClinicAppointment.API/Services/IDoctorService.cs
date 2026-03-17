using ClinicAppointment.API.DTOs;

namespace ClinicAppointment.API.Services
{
    public interface IDoctorService
    {
        Task<List<DoctorDto>> GetDoctorsAsync();
        Task<DoctorDto?> CreateDoctorAsync(DoctorCreateDto dto);
        Task<DoctorDto?> GetDoctorAsync(int id);
        Task<List<DoctorDto>> GetDoctorsByClinicAsync(int clinicId);
        Task<List<DoctorDto>> GetDoctorsBySpecialityAsync(int specialityId);
        Task<List<DoctorSearchDto>> SearchDoctorsAsync(string query);
        Task<bool> UpdateDoctorAsync(int id, DoctorCreateDto dto);
        Task<bool?> DeleteDoctorAsync(int id);
    }
}
