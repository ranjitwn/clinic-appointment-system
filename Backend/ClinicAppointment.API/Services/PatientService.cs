using ClinicAppointment.API.Data;
using ClinicAppointment.API.DTOs;
using ClinicAppointment.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ClinicAppointment.API.Services
{
    public class PatientService
    {
        private readonly DataContext _dataContext;
        private readonly ILogger<PatientService> _logger;

        public PatientService(DataContext dataContext, ILogger<PatientService> logger)
        {
            _dataContext = dataContext;
            _logger = logger;
        }

        public async Task<List<PatientDto>> GetPatientsAsync()
        {
            _logger.LogInformation("Fetching all patients");

            return await _dataContext.Patients.Select(p => new PatientDto
            {
                Id = p.Id,
                FirstName = p.FirstName,
                LastName = p.LastName,
                Email = p.Email,
                DateOfBirth = p.DateOfBirth,
                IsRegistered = p.IsRegistered,
                Role = p.Role,
                Gender = p.Gender
            }).ToListAsync();
        }

        public async Task<string?> CreatePatientAsync(PatientCreateDto dto)
        {
            _logger.LogInformation("Attempting to create guest patient with email {Email}", dto.Email);

            // Validate Date of Birth FIRST
            var dob = dto.DateOfBirth;

            if (dob == default)
            {
                _logger.LogWarning("Patient creation failed: Date of birth missing for email {Email}", dto.Email);
                return "Date of birth is required.";
            }

            if (dob > DateTime.Today)
            {
                _logger.LogWarning("Patient creation failed: Date of birth in the future for email {Email}", dto.Email);
                return "Date of birth cannot be in the future.";
            }

            if (dob < DateTime.Today.AddYears(-120))
            {
                _logger.LogWarning("Patient creation failed: Invalid date of birth for email {Email}", dto.Email);
                return "Invalid date of birth.";
            }

            // Check if patient already exists by email
            var existingPatient = await _dataContext.Patients
                .FirstOrDefaultAsync(p => p.Email.ToLower() == dto.Email.ToLower());

            if (existingPatient != null && existingPatient.IsRegistered)
            {
                _logger.LogWarning("Patient creation failed: Registered patient already exists with email {Email}", dto.Email);
                return "A registered patient already exists with this email.";
            }

            if (existingPatient != null)
            {
                _logger.LogInformation("Guest patient already exists with email {Email}, using existing record", dto.Email);
                return null; // guest already exists OK
            }

            // Create new guest patient
            var patient = new Patient
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                DateOfBirth = dto.DateOfBirth,
                Gender = dto.Gender,
                IsRegistered = false,
                Role = "Patient"
            };

            _dataContext.Patients.Add(patient);
            await _dataContext.SaveChangesAsync();

            _logger.LogInformation("Guest patient created successfully with email {Email}", dto.Email);

            return null; // success
        }
        public async Task<PatientDto?> GetPatientAsync(int id)
        {
            _logger.LogInformation("Fetching patient with ID {PatientId}", id);

            return await _dataContext.Patients.Select(p => new PatientDto
            {
                Id = p.Id,
                FirstName = p.FirstName,
                LastName = p.LastName,
                Email = p.Email,
                DateOfBirth = p.DateOfBirth,
                Gender = p.Gender,
                IsRegistered = p.IsRegistered,
                Role = p.Role
            }).FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<bool> UpdatePatientAsync(int id, PatientCreateDto dto)
        {
            _logger.LogInformation("Attempting to update patient {PatientId}", id);

            var patient = await _dataContext.Patients.FindAsync(id);

            if (patient == null)
            {
                _logger.LogWarning("Update failed: patient {PatientId} not found", id);
                return false;
            }

            patient.FirstName = dto.FirstName;
            patient.LastName = dto.LastName;
            patient.Email = dto.Email;
            patient.DateOfBirth = dto.DateOfBirth;
            patient.Gender = dto.Gender;

            await _dataContext.SaveChangesAsync();

            _logger.LogInformation("Patient {PatientId} updated successfully", id);

            return true;
        }

        public async Task<bool?> DeletePatientAsync(int id)
        {
            _logger.LogInformation("Attempting to delete patient {PatientId}", id);

            var patient = await _dataContext.Patients.FindAsync(id);

            if (patient == null)
            {
                _logger.LogWarning("Delete failed: patient {PatientId} not found", id);
                return null;
            }

            // Prevent deletion if appointments exist
            var hasAppointments = await _dataContext.Appointments
                .AnyAsync(a => a.PatientId == id);

            if (hasAppointments)
            {
                _logger.LogWarning("Delete prevented: patient {PatientId} has existing appointments", id);
                return false;
            }

            _dataContext.Patients.Remove(patient);
            await _dataContext.SaveChangesAsync();

            _logger.LogInformation("Patient {PatientId} deleted successfully", id);

            return true;
        }
    }
}
