📘 File: AppointmentController.cs
Responsibility

AppointmentController is responsible for HTTP handling for the appointment feature.

It should:

Receive HTTP requests (route + method)

Accept DTO input from the client

Read authentication info from JWT (claims)

Call AppointmentService for business logic

Return correct HTTP responses (200/201/400/401/404)

Provide Swagger documentation through XML comments

It should NOT contain heavy business logic (that stays in service).

Controller Setup
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class AppointmentController : ControllerBase
What these attributes do

[ApiController]

Enables automatic model validation

Improves binding/validation behavior for APIs

[Route("api/[controller]")]

Maps to /api/Appointment

[Produces("application/json")]

Declares JSON responses for Swagger/documentation

Dependency Injection
private readonly AppointmentService _appointmentService;

public AppointmentController(AppointmentService appointmentService)
{
    _appointmentService = appointmentService;
}

ASP.NET Core injects AppointmentService through DI.

Registered in Program.cs:

builder.Services.AddScoped<AppointmentService>();
Endpoint 1: Create Appointment (Guest or Registered)
[HttpPost]
public async Task<IActionResult> CreateAppointment(AppointmentCreateDto dto)
What it does

Allows both guest and logged-in users to create appointments

If user is authenticated → extracts PatientId from JWT

If not authenticated → sends null patientId (guest booking)

Calls service and returns either:

400 with error message

201 created success message

JWT Claim Extraction
if (User.Identity?.IsAuthenticated == true)
{
    var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
    patientId = int.Parse(claim);
}

This reads the PatientId from JWT NameIdentifier claim.

Service result pattern
var errorMessage = await _appointmentService.CreateAppointmentAsync(dto, patientId);

if (errorMessage != null)
    return BadRequest(new { message = errorMessage });

return Created("", new { message = "Appointment created successfully." });

Service returns:

null = success

"error message" = validation failure

Controller decides HTTP status code.

Endpoint 2: Get My Appointments (Patient only)
[Authorize(Roles = "Patient")]
[HttpGet("my")]
public async Task<ActionResult<IEnumerable<AppointmentDto>>> GetMyAppointments()
What it does

Only accessible to logged-in users with role Patient

Extracts patientId from JWT

Returns list of appointment DTOs

Returns:

200 OK with list

401 Unauthorized if no token

Endpoint 3: Update Appointment (Patient only)
[Authorize(Roles = "Patient")]
[HttpPut("{id}")]
public async Task<IActionResult> UpdateAppointment(int id, AppointmentUpdateDto dto)
What it does

Only updates appointments owned by the authenticated patient

Reads patientId from token

Calls service

Returns:

200 OK success

400 BadRequest with message

401 Unauthorized if claim missing

Endpoint 4: Delete Appointment (Patient only)
[Authorize(Roles = "Patient")]
[HttpDelete("{id}")]
public async Task<IActionResult> DeleteAppointment(int id)
What it does

Deletes appointment owned by the patient

Returns:

200 OK on success

404 NotFound if appointment not found or not owned

401 Unauthorized if not logged in

Uses service returning bool.

Endpoint 5: Get Appointments by Clinic (Admin only)
[Authorize(Roles = "Admin")]
[HttpGet("clinic/{clinicId}")]
public async Task<ActionResult<IEnumerable<AppointmentDto>>> GetAppointmentsByClinic(int clinicId)

Admin-only

Returns clinic appointments

Endpoint 6: Get Appointments by Doctor (Admin only)
[Authorize(Roles = "Admin")]
[HttpGet("doctor/{doctorId}")]
public async Task<ActionResult<IEnumerable<AppointmentDto>>> GetAppointmentsByDoctor(int doctorId)

Admin-only

Returns doctor appointments

Endpoint 7: Get Available Slots (Public)
[HttpGet("available-slots")]
public async Task<IActionResult> GetAvailableSlots(int doctorId, DateTime date, int duration = 30)
What it does

Validates duration in controller (basic check)

Calls service to generate available slots

Returns:

{ "availableSlots": [ ... ] }

This is used by frontend to show selectable time slots.

Key Concepts This Controller Demonstrates

✅ Thin controller, logic in service
✅ Uses DTOs for request/response
✅ Role-based authorization (Patient/Admin)
✅ Reads JWT claims for patient identity
✅ Correct HTTP response codes
✅ Swagger XML documentation support

Interview Questions & Strong Answers
Q: What is the controller’s responsibility?

Controllers handle HTTP concerns (routing, status codes, authorization, request/response). Business logic belongs in services.

Q: How do you get user ID from JWT?

After token validation, ASP.NET populates HttpContext.User. You can read claims using User.FindFirstValue(ClaimTypes.NameIdentifier).

Q: Why do you use [Authorize(Roles="Patient")]?

To restrict the endpoint so only authenticated users with the Patient role can access it.

Q: Why return DTOs instead of models?

DTOs protect the database structure and return only the fields the client needs, preventing overexposure and serialization issues.

Q: Why does CreateAppointment allow both guest and registered?

If authenticated, the patientId comes from JWT. If not, guest details are provided and the service creates a guest patient automatically.

Self-Check

You understand this controller if you can explain:

How routes map to endpoints

How JWT claims are read

Why controllers call services

Why different roles exist

Why DTOs are used

What each endpoint returns (status codes)

✅ Full Cycle Completed (Appointment Feature)
Appointment Model
↓
DataContext (tables + relationships)
↓
AppointmentService (business rules + DB queries)
↓
DTOs (API contract)
↓
AppointmentController (HTTP + auth + responses)
↓
Frontend calls endpoints
Next Step (Keep it structured)

