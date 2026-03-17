using ClinicAppointment.API.DTOs;

namespace ClinicAppointment.API.Services
{
    public interface IAppointmentService
    {
        Task<List<AppointmentDto>> GetMyAppointmentsAsync(int patientId);
        Task<string?> CreateAppointmentAsync(AppointmentCreateDto dto, int? patientId);
        Task<string?> UpdateAppointmentAsync(int id, int patientId, AppointmentUpdateDto dto);
        Task<bool> DeleteAppointmentAsync(int id, int patientId);
        Task<List<AppointmentDto>> GetAppointmentsByClinicAsync(int clinicId);
        Task<List<AppointmentDto>> GetAppointmentsByDoctorAsync(int doctorId);
        Task<List<DateTime>> GetAvailableSlotsAsync(int doctorId, DateTime date, int slotDuration = 30);
    }
}
