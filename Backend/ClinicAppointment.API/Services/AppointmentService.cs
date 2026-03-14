using ClinicAppointment.API.Data;
using ClinicAppointment.API.DTOs;
using ClinicAppointment.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ClinicAppointment.API.Services
{
    public class AppointmentService
    {
        private readonly DataContext _dataContext;
        private readonly PatientService _patientService;
        private readonly ILogger<AppointmentService> _logger;

        public AppointmentService(DataContext dataContext, PatientService patientService, ILogger<AppointmentService> logger)
        {
            _dataContext = dataContext;
            _patientService = patientService;
            _logger = logger;
        }

        private static AppointmentDto MapToDto(Appointment a)
        {
            return new AppointmentDto
            {
                Id = a.Id,
                AppointmentDate = a.AppointmentDate,
                DurationMinutes = a.DurationMinutes,

                ClinicId = a.ClinicId,
                ClinicName = a.Clinic!.Name,

                DoctorId = a.DoctorId,
                DoctorName = a.Doctor!.FirstName + " " + a.Doctor!.LastName,

                CategoryId = a.CategoryId,
                CategoryName = a.Category!.Name,

                PatientId = a.PatientId
            };
        }

        public async Task<List<AppointmentDto>> GetMyAppointmentsAsync(int patientId)
        {
            _logger.LogInformation("Fetching appointments for Patient {PatientId}", patientId);

            var appointments = await _dataContext.Appointments
                .Where(a => a.PatientId == patientId)
                .Include(a => a.Clinic)
                .Include(a => a.Doctor)
                .Include(a => a.Category)
                .OrderBy(a => a.AppointmentDate)
                .ToListAsync();

            return appointments.Select(MapToDto).ToList();
        }

        public async Task<string?> CreateAppointmentAsync(AppointmentCreateDto dto, int? patientId)
        {
            _logger.LogInformation("Attempting to create appointment for Doctor {DoctorId} at {AppointmentDate}",
               dto.DoctorId,
               dto.AppointmentDate);

            // Guest booking → create patient
            if (patientId == null)
            {
                if (string.IsNullOrWhiteSpace(dto.FirstName) ||
                    string.IsNullOrWhiteSpace(dto.LastName) ||
                    string.IsNullOrWhiteSpace(dto.Email))
                {
                    _logger.LogWarning("Guest booking attempted without required patient details.");
                    return "Guest patient details are required.";
                }

                var error = await _patientService.CreatePatientAsync(
                    new PatientCreateDto
                    {
                        FirstName = dto.FirstName,
                        LastName = dto.LastName,
                        Email = dto.Email,
                        DateOfBirth = dto.DateOfBirth
                    });

                if (error != null)
                {
                    _logger.LogWarning("Guest patient creation failed: {Error}", error);
                    return error;
                }

                var patient = await _dataContext.Patients
                    .FirstOrDefaultAsync(p => p.Email.ToLower() == dto.Email.ToLower());

                if (patient == null)
                {
                    _logger.LogError("Guest patient creation failed unexpectedly for email {Email}", dto.Email);
                    return "Failed to create guest patient.";
                }

                patientId = patient.Id;
            }

            // Default duration if not specified
            var duration = dto.DurationMinutes ?? 30;

            // Prevent unrealistic durations
            if (duration != 15 && duration != 30 &&
                duration != 45 && duration != 60)
                return "Invalid duration.";

            // FK validation
            var clinicExists = await _dataContext.Clinics.AnyAsync(c => c.Id == dto.ClinicId);

            if (!clinicExists)
                return "Invalid clinic ID.";

            var doctorExists = await _dataContext.Doctors
                .AnyAsync(d => d.Id == dto.DoctorId && d.ClinicId == dto.ClinicId);

            if (!doctorExists)
                return "Invalid doctor ID for the specified clinic.";

            var categoryExists = await _dataContext.Categories.AnyAsync(c => c.Id == dto.CategoryId);

            if (!categoryExists)
                return "Invalid category ID.";


            // Prevent weekend bookings
            if (dto.AppointmentDate.DayOfWeek == DayOfWeek.Saturday ||
                dto.AppointmentDate.DayOfWeek == DayOfWeek.Sunday)
            {
                return "Appointments cannot be booked on weekends.";
            }

            // Prevent past appointments
            if (dto.AppointmentDate < DateTime.Now)
            {
                return "Appointments cannot be booked in the past.";
            }

            // Restrict clinic hours (08:00–18:00)
            var appointmentTime = dto.AppointmentDate.TimeOfDay;

            if (appointmentTime < TimeSpan.FromHours(8) ||
                appointmentTime >= TimeSpan.FromHours(18))
            {
                return "Appointments can only be booked between 08:00 and 18:00.";
            }

            // Ensure appointment finishes within clinic hours
            var appointmentEndTime = dto.AppointmentDate.AddMinutes(duration).TimeOfDay;

            if (appointmentEndTime > TimeSpan.FromHours(18))
            {
                return "Appointment exceeds clinic hours.";
            }

            // Allow only 15-minute intervals
            var minutes = dto.AppointmentDate.Minute;

            if (minutes != 0 && minutes != 15 &&
                minutes != 30 && minutes != 45)
            {
                return "Appointments must be scheduled in 15-minute intervals.";
            }

            var newEndTime = dto.AppointmentDate.AddMinutes(duration);

            // DOCTOR SLOT CONFLICT 
            var slotConflict = await _dataContext.Appointments.AnyAsync(a =>
                a.DoctorId == dto.DoctorId &&
                a.ClinicId == dto.ClinicId &&
                dto.AppointmentDate < a.AppointmentDate.AddMinutes(a.DurationMinutes) &&
                newEndTime > a.AppointmentDate
            );

            if (slotConflict)
            {
                _logger.LogWarning("Doctor booking conflict detected for Doctor {DoctorId} at {AppointmentDate}",
                    dto.DoctorId,
                    dto.AppointmentDate);

                return "Doctor is already booked at this time.";
            }

            // Patient conflict 
            var patientConflict = await _dataContext.Appointments.AnyAsync(a =>
                a.PatientId == patientId &&
                dto.AppointmentDate < a.AppointmentDate.AddMinutes(a.DurationMinutes) &&
                newEndTime > a.AppointmentDate
            );

            if (patientConflict)
            {
                _logger.LogWarning(
                    "Patient {PatientId} attempted double booking at {AppointmentDate}",
                    patientId,
                    dto.AppointmentDate);

                return "Patient already has an appointment at this time.";
            }

            var appointment = new Appointment
            {
                AppointmentDate = dto.AppointmentDate,
                DurationMinutes = duration,
                ClinicId = dto.ClinicId,
                DoctorId = dto.DoctorId,
                PatientId = patientId.Value,
                CategoryId = dto.CategoryId
            };

            _dataContext.Appointments.Add(appointment);
            await _dataContext.SaveChangesAsync();

            _logger.LogInformation("Appointment created successfully for Patient {PatientId} with Doctor {DoctorId} at {AppointmentDate}",
               patientId,
               dto.DoctorId,
               dto.AppointmentDate);

            return null;
        }



        public async Task<string?> UpdateAppointmentAsync(int id, int patientId, AppointmentUpdateDto dto)
        {
            _logger.LogInformation("Patient {PatientId} attempting to update appointment {AppointmentId}", patientId, id);

            var appointment = await _dataContext.Appointments
                .FirstOrDefaultAsync(a => a.Id == id && a.PatientId == patientId);

            if (appointment == null)
            {
                _logger.LogWarning("Update failed. Appointment {AppointmentId} not found for Patient {PatientId}",
                    id,
                    patientId);

                return "Appointment not found.";
            }

            // Validate duration
            if (dto.DurationMinutes != 15 &&
                dto.DurationMinutes != 30 &&
                dto.DurationMinutes != 45 &&
                dto.DurationMinutes != 60)
                return "Invalid duration";

            // Prevent past appointments
            if (dto.AppointmentDate < DateTime.Now)
                return "Appointment date is in the past";

            // Prevent weekend bookings
            if (dto.AppointmentDate.DayOfWeek == DayOfWeek.Saturday ||
                dto.AppointmentDate.DayOfWeek == DayOfWeek.Sunday)
                return "Appointments cannot be booked on weekends";

            // Restrict clinic hours (08:00–18:00)
            var appointmentTime = dto.AppointmentDate.TimeOfDay;

            if (appointmentTime < TimeSpan.FromHours(8) ||
                appointmentTime >= TimeSpan.FromHours(18))
                return "Appointments can only be booked between 08:00 and 18:00";

            // Allow only 15-minute intervals
            var minutes = dto.AppointmentDate.Minute;

            if (minutes != 0 && minutes != 15 &&
                minutes != 30 && minutes != 45)
                return "Appointments must be booked in 15-minute intervals";

            var newEndTime = dto.AppointmentDate.AddMinutes(dto.DurationMinutes);

            // Prevent doctor slot conflict when updating
            var slotConflict = await _dataContext.Appointments.AnyAsync(a =>
                a.Id != id &&
                a.DoctorId == appointment.DoctorId &&
                a.ClinicId == appointment.ClinicId &&
                dto.AppointmentDate < a.AppointmentDate.AddMinutes(a.DurationMinutes) &&
                newEndTime > a.AppointmentDate
            );

            if (slotConflict)
                return "Doctor is already booked at this time";

            // Prevent patient double booking
            var patientConflict = await _dataContext.Appointments.AnyAsync(a =>
                a.Id != id &&
                a.PatientId == patientId &&
                dto.AppointmentDate < a.AppointmentDate.AddMinutes(a.DurationMinutes) &&
                newEndTime > a.AppointmentDate
            );

            if (patientConflict)
                return "Patient is already booked at this time";


            appointment.AppointmentDate = dto.AppointmentDate;
            appointment.DurationMinutes = dto.DurationMinutes;

            await _dataContext.SaveChangesAsync();

            _logger.LogInformation("Appointment {AppointmentId} updated successfully by Patient {PatientId}", id, patientId);

            return null;
        }


        public async Task<bool> DeleteAppointmentAsync(int id, int patientId)
        {
            _logger.LogInformation("Patient {PatientId} attempting to delete appointment {AppointmentId}", patientId, id);

            var appointment = await _dataContext.Appointments
                .FirstOrDefaultAsync(a => a.Id == id && a.PatientId == patientId);

            if (appointment == null)
            {
                _logger.LogWarning("Delete failed. Appointment {AppointmentId} not found for Patient {PatientId}", id, patientId);

                return false;
            }

            _dataContext.Appointments.Remove(appointment);
            await _dataContext.SaveChangesAsync();

            _logger.LogInformation("Appointment {AppointmentId} deleted by Patient {PatientId}", id, patientId);

            return true;
        }

        public async Task<List<AppointmentDto>> GetAppointmentsByClinicAsync(int clinicId)
        {
            _logger.LogInformation("Fetching appointments for Clinic {ClinicId}", clinicId);

            var appointments = await _dataContext.Appointments
                .Where(a => a.ClinicId == clinicId)
                .Include(a => a.Clinic)
                .Include(a => a.Doctor)
                .Include(a => a.Category)
                .OrderBy(a => a.AppointmentDate)
                .ToListAsync();

            return appointments.Select(MapToDto).ToList();
        }

        public async Task<List<AppointmentDto>> GetAppointmentsByDoctorAsync(int doctorId)
        {
            _logger.LogInformation("Fetching appointments for Doctor {DoctorId}", doctorId);

            var appointments = await _dataContext.Appointments
                .Where(a => a.DoctorId == doctorId)
                .Include(a => a.Clinic)
                .Include(a => a.Doctor)
                .Include(a => a.Category)
                .OrderBy(a => a.AppointmentDate)
                .ToListAsync();

            return appointments.Select(MapToDto).ToList();
        }

        public async Task<List<DateTime>> GetAvailableSlotsAsync(
            int doctorId,
            DateTime date,
            int slotDuration = 30)
        {
            _logger.LogInformation("Fetching available slots for Doctor {DoctorId} on {Date}", doctorId, date);

            // Prevent weekend slots
            if (date.DayOfWeek == DayOfWeek.Saturday ||
                date.DayOfWeek == DayOfWeek.Sunday)
            {
                return new List<DateTime>();
            }

            // Prevent past date slots
            if (date.Date < DateTime.Today)
            {
                return new List<DateTime>();
            }

            var clinicStart = date.Date.AddHours(8);
            var clinicEnd = date.Date.AddHours(18);

            var existingAppointments = await _dataContext.Appointments
                .Where(a => a.DoctorId == doctorId &&
                            a.AppointmentDate.Date == date.Date)
                .ToListAsync();

            var availableSlots = new List<DateTime>();

            for (var time = clinicStart; time < clinicEnd; time = time.AddMinutes(slotDuration))
            {
                var slotEnd = time.AddMinutes(slotDuration);

                var conflict = existingAppointments.Any(a =>
                    time < a.AppointmentDate.AddMinutes(a.DurationMinutes) &&
                    slotEnd > a.AppointmentDate
                );

                if (!conflict)
                {
                    availableSlots.Add(time);
                }
            }

            return availableSlots;
        }

    }
}
