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
    public class ClinicController : ControllerBase
    {
        private readonly ClinicService _clinicService;

        public ClinicController(ClinicService clinicService)
        {
            _clinicService = clinicService;
        }

        /// <summary>
        /// Gets all clinics.
        /// </summary>
        /// <response code="200">Returns the list of clinics</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ClinicDto>>> GetClinics()
        {
            return Ok(await _clinicService.GetClinicsAsync());
        }

        /// <summary>
        /// Creates a new clinic.
        /// </summary>
        /// <remarks>
        /// Clinics represent physical or organizational locations where appointments take place.
        /// </remarks>
        /// <response code="201">Clinic created successfully</response>
        /// <response code="400">If the request data is invalid</response>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ClinicDto>> CreateClinic(ClinicCreateDto dto)
        {
            var clinic = await _clinicService.CreateClinicAsync(dto);

            if (clinic == null)
                return BadRequest(new { message = "Clinic already exists." });

            return CreatedAtAction(nameof(GetClinic), new { id = clinic.Id }, clinic);
        }
        /// <summary>
        /// Gets a specific clinic by ID.
        /// </summary>
        /// <response code="200">Returns the clinic</response>
        /// <response code="404">If the clinic is not found</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ClinicDto>> GetClinic(int id)
        {
            var clinic = await _clinicService.GetClinicAsync(id);

            if (clinic == null)
                return NotFound(new { message = "Clinic not found." });

            return Ok(clinic);
        }

        /// <summary>
        /// Updates a clinic.
        /// </summary>
        /// <remarks>
        /// Only Admin users can modify clinic data.
        /// </remarks>
        /// <response code="200">Clinic updated successfully</response>
        /// <response code="400">If update fails due to duplicate name or clinic not found.</response>
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateClinic(int id, ClinicCreateDto dto)
        {
            var updated = await _clinicService.UpdateClinicAsync(id, dto);

            if (!updated)
                return BadRequest(new { message = "Update failed — duplicate name or clinic not found." });

            return Ok(new { message = "Clinic updated successfully" });
        }

        /// <summary>
        /// Deletes a clinic.
        /// </summary>
        /// <remarks>
        /// Only Admin users can delete clinic data.
        /// </remarks>
        /// <response code="200">Clinic deleted successfully</response>
        /// <response code="404">If the clinic is not found</response>
        /// <response code="409">If the clinic has related doctors or appointments and cannot be deleted</response>
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]

        public async Task<IActionResult> DeleteClinic(int id)
        {
            var result = await _clinicService.DeleteClinicAsync(id);

            if (result == null)
                return NotFound(new { message = "Clinic not found." });

            if (result == false)
                return Conflict(new { message = "Clinic has related doctors or appointments." });

            return Ok(new { message = "Clinic deleted successfully" });
        }
    }
}
