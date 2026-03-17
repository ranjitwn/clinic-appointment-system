using ClinicAppointment.API.DTOs;
using ClinicAppointment.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ClinicAppointment.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class AppointmentController : ControllerBase
    {
        private readonly IAppointmentService _appointmentService;

        public AppointmentController(IAppointmentService appointmentService)
        {
            _appointmentService = appointmentService;

        }

        /// <summary>
        /// Creates a new appointment for a guest or registered patient.
        /// </summary>
        /// <remarks>
        /// Registered patient example:
        ///
        /// POST /api/Appointment
        /// {
        ///   "appointmentDate": "2026-02-28T10:00:00",
        ///   "durationMinutes": 30,
        ///   "clinicId": 1,
        ///   "doctorId": 2,
        ///   "categoryId": 1
        /// }
        ///
        /// Guest patient example:
        ///
        /// POST /api/Appointment
        /// {
        ///   "appointmentDate": "2026-02-28T10:00:00",
        ///   "durationMinutes": 30,
        ///   "clinicId": 1,
        ///   "doctorId": 2,
        ///   "categoryId": 1,
        ///   "firstName": "Ranjit",
        ///   "lastName": "Nair",
        ///   "email": "ranjit@test.com",
        ///   "dateOfBirth": "1990-11-04"
        /// }
        ///
        /// AppointmentDate must align to 15-minute intervals (:00, :15, :30, :45).
        /// Allowed duration values: 15, 30, 45, or 60 minutes.
        /// Registered patients use JWT token for PatientId.
        /// Guest patients are automatically created.
        /// </remarks>
        /// <response code="201">Appointment created successfully</response>
        /// <response code="400">If the appointment slot is already booked or conflicts with another appointment</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateAppointment(AppointmentCreateDto dto)
        {
            int? patientId = null;

            if (User.Identity?.IsAuthenticated == true)
            {
                var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (int.TryParse(claim, out var parsedId))
                {
                    patientId = parsedId;
                }
            }

            var errorMessage = await _appointmentService.CreateAppointmentAsync(dto, patientId);

            if (errorMessage != null)
            {
                return BadRequest(new { message = errorMessage });
            }

            return Created("", new { message = "Appointment created successfully." });
        }

        /// <summary>
        /// Gets all appointments belonging to the currently authenticated patient.
        /// </summary>
        /// <remarks>
        /// This endpoint is only accessible to registered patients.
        /// The patient ID is extracted from the JWT token.
        /// </remarks>
        /// <response code="200">Returns the list of the patient's appointments</response>
        /// <response code="401">If the user is not authenticated</response>
        [Authorize(Roles = "Patient")]
        [HttpGet("my")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<IEnumerable<AppointmentDto>>> GetMyAppointments()
        {
            if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var patientId))
                return Unauthorized();

            var appointments = await _appointmentService.GetMyAppointmentsAsync(patientId);

            return Ok(appointments);
        }

        /// <summary>
        /// Updates an existing appointment owned by the authenticated patient.
        /// </summary>
        /// <remarks>
        /// Patients can only update their own appointments.
        /// 
        ///
        /// Example:
        ///
        /// PUT /api/Appointment/{id}
        /// {
        ///   "appointmentDate": "2026-02-28T10:00:00",
        ///   "durationMinutes": 30
        /// }
        ///
        /// AppointmentDate must align to 15-minute intervals (:00, :15, :30, :45).
        /// Allowed duration values: 15, 30, 45, or 60 minutes.
        /// </remarks>
        /// <response code="200">Appointment updated successfully</response>
        /// <response code="401">If the user is not authenticated</response>
        /// <response code="400"> If update fails due to validation, conflict, or appointment not found. </response>
        [Authorize(Roles = "Patient")]
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UpdateAppointment(int id, AppointmentUpdateDto dto)
        {
            if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var patientId))
                return Unauthorized();

            var errorMessage = await _appointmentService.UpdateAppointmentAsync(id, patientId, dto);

            if (errorMessage != null)
                return BadRequest(new { message = errorMessage });

            return Ok(new { message = "Appointment updated successfully" });
        }

        /// <summary>
        /// Deletes an appointment owned by the authenticated patient.
        /// </summary>
        /// <remarks>
        /// Patients can only delete their own appointments.
        /// </remarks>
        /// <response code="200">Appointment deleted successfully</response>
        /// <response code="401">If the user is not authenticated</response>
        /// <response code="404">If the appointment does not exist or does not belong to the patient</response>
        [Authorize(Roles = "Patient")]
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteAppointment(int id)
        {
            if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var patientId))
                return Unauthorized();

            var success = await _appointmentService.DeleteAppointmentAsync(id, patientId);

            if (!success)
                return NotFound(new { message = "Appointment not found." });

            return Ok(new { message = "Appointment deleted successfully" });
        }

        /// <summary>
        /// Gets all appointments for a specific clinic.    
        /// </summary>
        /// <remarks>
        /// This endpoint requires Admin role authentication.
        /// It returns all appointments associated with the specified clinic.       
        /// </remarks>
        /// <response code="200">Returns the list of appointments for the specified clinic</response>
        /// <response code="401">If the user is not authenticated</response>
        [Authorize(Roles = "Admin")]
        [HttpGet("clinic/{clinicId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<IEnumerable<AppointmentDto>>> GetAppointmentsByClinic(int clinicId)
        {
            var appointments = await _appointmentService.GetAppointmentsByClinicAsync(clinicId);

            return Ok(appointments);
        }

        /// <summary>
        /// Gets all appointments for a specific doctor.
        /// </summary>
        /// <remarks>
        /// This endpoint requires Admin role authentication.
        /// It returns all appointments associated with the specified doctor.
        /// </remarks>
        /// <response code="200">Returns the list of appointments for the specified doctor</response>
        /// <response code="401">If the user is not authenticated</response>
        [Authorize(Roles = "Admin")]
        [HttpGet("doctor/{doctorId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<IEnumerable<AppointmentDto>>> GetAppointmentsByDoctor(int doctorId)
        {
            var appointments = await _appointmentService.GetAppointmentsByDoctorAsync(doctorId);

            return Ok(appointments);
        }

        /// <summary>
        /// Gets available appointment time slots for a specific doctor on a given date.
        /// </summary>
        /// <remarks>
        /// Generates available appointment slots dynamically based on:
        /// - Clinic working hours (08:00–18:00)
        /// - Existing booked appointments
        /// - Selected appointment duration (15, 30, 45, or 60 minutes)
        /// 
        ///
        /// Example:
        /// GET /api/Appointment/available-slots?doctorId=2&amp;date=2026-02-28&amp;duration=30
        ///
        /// Date should be provided in YYYY-MM-DD format.
        /// </remarks>
        /// <response code="200">Returns the list of available appointment time slots</response>
        /// <response code="400">If an invalid duration is provided</response>
        [HttpGet("available-slots")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAvailableSlots(
            int doctorId,
            DateTime date,
            int duration = 30)
        {
            // Validate duration (match appointment rules)
            if (duration != 15 && duration != 30 &&
                duration != 45 && duration != 60)
            {
                return BadRequest(new
                {
                    message = "Invalid duration. Allowed values: 15, 30, 45, 60 minutes."
                });
            }

            var slots = await _appointmentService.GetAvailableSlotsAsync(
                doctorId,
                date,
                duration);

            return Ok(new { availableSlots = slots });
        }

    }
}
