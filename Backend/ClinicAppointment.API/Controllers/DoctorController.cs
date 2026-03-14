using ClinicAppointment.API.DTOs;
using ClinicAppointment.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClinicAppointment.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class DoctorController : ControllerBase
    {
        private readonly DoctorService _doctorService;

        public DoctorController(DoctorService doctorService)
        {
            _doctorService = doctorService;
        }

        /// <summary>
        /// Gets all doctors.
        /// </summary>
        /// <response code="200">Returns the list of doctors</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<DoctorDto>>> GetDoctors()
        {
            return Ok(await _doctorService.GetDoctorsAsync());
        }

        /// <summary>
        /// Creates a new doctor.
        /// </summary>
        /// <remarks>
        /// Each doctor is associated with a clinic and a speciality.
        /// </remarks>
        /// <response code="201">Doctor created successfully</response>
        /// <response code="400">If the request data is invalid</response>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<DoctorDto>> CreateDoctor(DoctorCreateDto dto)
        {
            var doctor = await _doctorService.CreateDoctorAsync(dto);

            if (doctor == null)
                return BadRequest(new { message = "Doctor already exists." });

            return CreatedAtAction(nameof(GetDoctor), new { id = doctor.Id }, doctor);
        }

        /// <summary>
        /// Gets a specific doctor by ID.   
        /// </summary>
        /// <response code="200">Returns the doctor</response>  
        /// <response code="404">If the doctor is not found</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DoctorDto>> GetDoctor(int id)
        {
            var doctor = await _doctorService.GetDoctorAsync(id);

            if (doctor == null)
                return NotFound(new { message = "Doctor not found." });

            return Ok(doctor);
        }

        /// <summary>
        /// Gets doctors by clinic ID.  
        /// </summary>
        /// <response code="200">Returns the list of doctors in the specified clinic</response>
        [HttpGet("by-clinic/{clinicId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<DoctorDto>>> GetDoctorsByClinic(int clinicId)
        {
            var doctors = await _doctorService.GetDoctorsByClinicAsync(clinicId);

            return Ok(doctors);
        }

        /// <summary>
        /// Gets doctors by speciality ID.
        /// </summary>
        /// <response code="200">Returns the list of doctors with the specified speciality</response>
        [HttpGet("by-speciality/{specialityId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<DoctorDto>>> GetDoctorsBySpeciality(int specialityId)
        {
            var doctors = await _doctorService.GetDoctorsBySpecialityAsync(specialityId);

            return Ok(doctors);
        }

        /// <summary>
        /// Searches for doctors by first name or last name.
        /// </summary>
        /// <response code="200">Returns the list of matching doctors</response>
        [HttpGet("search")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<DoctorSearchDto>>> SearchDoctors([FromQuery] string query)
        {
            var results = await _doctorService.SearchDoctorsAsync(query);

            return Ok(results);
        }

        /// <summary>
        /// Updates an existing doctor.
        /// </summary>
        /// <remarks>
        /// Only Admin users can modify doctor data.
        /// </remarks>
        /// <response code="200">Doctor updated successfully</response>
        /// <response code="400">If the doctor is not found</response>
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateDoctor(int id, DoctorCreateDto dto)
        {
            var updated = await _doctorService.UpdateDoctorAsync(id, dto);

            if (!updated)
                return BadRequest(new { message = "Update failed due to validation or duplicate doctor." });

            return Ok(new { message = "Doctor updated successfully" });
        }

        /// <summary>
        /// Deletes a doctor.
        /// </summary>
        /// <remarks>
        /// Only Admin users can delete doctor data.
        /// </remarks>
        /// <response code="200">Doctor deleted successfully</response>
        /// <response code="400">If deletion fails because doctor has appointments or is not found.</response>
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> DeleteDoctor(int id)
        {
            var result = await _doctorService.DeleteDoctorAsync(id);

            if (result == null)
                return NotFound(new { message = "Doctor not found." });

            if (result == false)
                return Conflict(new { message = "Doctor has appointments." });

            return Ok(new { message = "Doctor deleted successfully" });
        }
    }
}
