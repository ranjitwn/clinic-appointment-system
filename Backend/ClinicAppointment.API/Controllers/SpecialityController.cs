using ClinicAppointment.API.Constants;
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
    public class SpecialityController : ControllerBase
    {
        private readonly ISpecialityService _specialityService;

        public SpecialityController(ISpecialityService specialityService)
        {
            _specialityService = specialityService;
        }

        /// <summary>
        /// Gets all doctor specialities.
        /// </summary>
        /// <remarks>
        /// A speciality represents a medical field (e.g. Cardiology, Dermatology).
        /// Multiple doctors can share the same speciality.
        /// </remarks>
        /// <response code="200">Returns the list of specialities</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<SpecialityDto>>> GetSpecialities()
        {
            return Ok(await _specialityService.GetSpecialitiesAsync());
        }

        /// <summary>
        /// Creates a new doctor speciality.
        /// </summary>
        /// <remarks>
        /// This endpoint allows adding new medical specialities that can later be assigned to doctors.
        /// Access is restricted to Admin users only.
        /// </remarks>
        /// <response code="201">Speciality created successfully</response>
        /// <response code="400">If the request data is invalid</response>
        [Authorize(Roles = Roles.Admin)]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<SpecialityDto>> CreateSpeciality(SpecialityCreateDto dto)
        {
            var speciality = await _specialityService.CreateSpecialityAsync(dto);

            if (speciality == null)
                return BadRequest(new { message = "Speciality already exists." });


            return CreatedAtAction(nameof(GetSpeciality), new { id = speciality.Id }, speciality);
        }

        /// <summary>
        /// Gets a specific doctor speciality by ID.
        /// </summary>
        /// <response code="200">Returns the speciality</response>
        /// <response code="404">If the speciality is not found</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<SpecialityDto>> GetSpeciality(int id)
        {
            var speciality = await _specialityService.GetSpecialityAsync(id);

            if (speciality == null)
                return NotFound(new { message = "Speciality not found." });

            return Ok(speciality);
        }

        /// <summary>
        /// Updates a doctor speciality.
        /// </summary>
        /// <remarks>
        /// Only Admin users can modify speciality data.
        /// </remarks>
        /// <response code="200">Speciality updated successfully</response>
        /// <response code="400">If update fails due to duplicate name or speciality not found.</response>
        [Authorize(Roles = Roles.Admin)]
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateSpeciality(int id, SpecialityCreateDto dto)
        {
            var updated = await _specialityService.UpdateSpecialityAsync(id, dto);

            if (!updated)
                return BadRequest(new { message = "Update failed — duplicate name or speciality not found." });

            return Ok(new { message = "Speciality updated successfully" });
        }

        /// <summary>
        /// Deletes a doctor speciality.
        /// </summary>
        /// <remarks>
        /// Only Admin users can delete speciality data.
        /// </remarks>
        /// <response code="200">Speciality deleted successfully</response>
        /// <response code="400">If deletion fails because speciality is in use or not found.</response>
        [Authorize(Roles = Roles.Admin)]
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> DeleteSpeciality(int id)
        {
            var result = await _specialityService.DeleteSpecialityAsync(id);

            if (result == null)
                return NotFound(new { message = "Speciality not found." });

            if (result == false)
                return Conflict(new { message = "Speciality is used by doctors." });

            return Ok(new { message = "Speciality deleted successfully" });
        }
    }
}
