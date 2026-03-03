using ExamProject2.API.DTOs;
using ExamProject2.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace ExamProject2.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Registers a new patient as a registered user.
        /// </summary>
        /// <remarks>
        /// Guest patients can exist without registration.
        /// Only registered patients are allowed to log in.
        /// 
        /// Example request:
        ///
        /// POST /api/Auth/register
        /// {
        ///   "firstName": "Ranjit",
        ///   "lastName": "Nair",
        ///   "email": "ranjit@test.com",
        ///   "password": "Test123!",
        ///   "dateOfBirth": "1990-11-04",
        ///   "gender": "Male"
        /// }
        /// </remarks>
        /// <response code="200">Patient registered successfully</response>
        /// <response code="400">If the patient is already registered</response>
        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register(PatientRegisterDto dto)
        {
            var errorMessage = await _authService.RegisterAsync(dto);

            if (errorMessage != null)
                return BadRequest(new { message = errorMessage });

            return Ok(new { message = "Registration successful." });
        }
        /// <summary>
        /// Authenticates a registered patient and returns a JWT token.
        /// </summary>
        /// <remarks>
        /// Login is only allowed for registered patients.
        /// The system first checks registration status before validating the password.
        /// 
        /// POST /api/Auth/login
        /// {
        ///   "email": "ranjit@test.com",
        ///   "password": "Test123!"
        /// }
        /// </remarks>
        /// <response code="200">Returns a valid JWT token</response>
        /// <response code="401">If the patient is not registered or credentials are invalid</response>
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login(PatientLoginDto dto)
        {
            var result = await _authService.LoginAsync(dto);

            if (result == null)
                return Unauthorized(new { message = "Invalid credentials." });

            return Ok(result);
        }
    }
}
