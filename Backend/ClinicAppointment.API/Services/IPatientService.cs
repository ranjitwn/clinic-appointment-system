using ClinicAppointment.API.DTOs;

namespace ClinicAppointment.API.Services
{
    public interface IPatientService
    {
        Task<List<PatientDto>> GetPatientsAsync();
        Task<string?> CreatePatientAsync(PatientCreateDto dto);
        Task<PatientDto?> GetPatientAsync(int id);
        Task<bool> UpdatePatientAsync(int id, PatientCreateDto dto);
        Task<bool?> DeletePatientAsync(int id);
    }
}
