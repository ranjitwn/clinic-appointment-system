using ClinicAppointment.API.DTOs;
using ClinicAppointment.API.Models;
using ClinicAppointment.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClinicAppointment.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class PatientController : ControllerBase
    {
        private readonly PatientService _patientService;

        public PatientController(PatientService patientService)
        {
            _patientService = patientService;
        }

        /// <summary>
        /// Gets all patients.
        /// </summary>
        /// <remarks>
        /// This endpoint is mainly intended for administrative or internal use.
        /// </remarks>
        /// <response code="200">Returns the list of patients</response>
        [Authorize(Roles = "Admin")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<PatientDto>>> GetPatients()
        {
            var patients = await _patientService.GetPatientsAsync();

            return Ok(patients);
        }

        /// <summary>
        /// Creates a new patient.
        /// </summary>
        /// <remarks>
        /// This endpoint can be used to create guest patients or pre-registered patients.
        /// </remarks>
        /// <response code="201">Patient created successfully</response>
        /// <response code="400">If the request data is invalid</response>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PatientDto>> CreatePatient(PatientCreateDto dto)
        {
            var error = await _patientService.CreatePatientAsync(dto);

            if (error != null)
                return BadRequest(new { message = error });

            return Created("", new { message = "Patient created successfully." });
        }

        /// <summary>
        /// Gets a specific patient by ID.
        /// </summary>
        /// <response code="200">Returns the patient</response>
        /// <response code="404">If the patient is not found</response>
        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PatientDto>> GetPatient(int id)
        {
            var patients = await _patientService.GetPatientAsync(id);

            if (patients == null)
                return NotFound(new { message = "Patient not found." });

            return Ok(patients);
        }

        /// <summary>
        /// Updates patient information.
        /// </summary>
        /// <remarks>
        /// Only Admin users can modify patient data.
        /// </remarks>
        /// <response code="200">Patient updated successfully</response>
        /// <response code="404">If the patient is not found</response>
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdatePatient(int id, PatientCreateDto dto)
        {
            var updated = await _patientService.UpdatePatientAsync(id, dto);

            if (!updated)
                return NotFound(new { message = "Patient not found." });

            return Ok(new { message = "Patient updated successfully" });
        }

        /// <summary>
        /// Deletes a patient.
        /// </summary>
        /// <remarks>
        /// Only Admin users can delete patient records.
        /// </remarks>
        /// <response code="200">Patient deleted successfully</response>
        /// <response code="404">If the patient is not found</response>
        /// <response code="409">If the patient has related appointments and cannot be deleted</response>
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> DeletePatient(int id)
        {
            var result = await _patientService.DeletePatientAsync(id);

            if (result == null)
                return NotFound(new { message = "Patient not found." });

            if (result == false)
                return Conflict(new { message = "Patient has appointments." });

            return Ok(new { message = "Patient deleted successfully" });
        }
    }
}
