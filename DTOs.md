📘 DTOs – Appointment Feature

DTO stands for Data Transfer Object.

DTOs define the data contract between the client and the API.

They control:

What data the client can send

What data the API returns

Validation rules for incoming requests

DTOs prevent exposing database models directly.

📘 File: AppointmentCreateDto.cs
Responsibility

This DTO represents the data required to create a new appointment.

It is used when the client sends a request like:

POST /api/appointments
Structure
public class AppointmentCreateDto
{
    [Required]
    public DateTime AppointmentDate { get; set; }

    public int? DurationMinutes { get; set; } = 30;

    [Required]
    public int ClinicId { get; set; }

    [Required]
    public int DoctorId { get; set; }

    [Required]
    public int CategoryId { get; set; }

    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }

    [Required]
    public DateTime DateOfBirth { get; set; }
}
What Each Field Means
AppointmentDate
[Required]
public DateTime AppointmentDate

Defines when the appointment will start.

The [Required] attribute means the request must include this field.

DurationMinutes
public int? DurationMinutes { get; set; } = 30;

Nullable value.

If not provided, the default duration is 30 minutes.

This makes the API flexible.

ClinicId
[Required]
public int ClinicId

References the clinic where the appointment will take place.

DoctorId
[Required]
public int DoctorId

Specifies the doctor for the appointment.

CategoryId
[Required]
public int CategoryId

Defines the appointment category.

Example:

Consultation
Check-up
Treatment
Guest Booking Fields
FirstName
LastName
Email
DateOfBirth

These fields allow guest patients to book appointments.

If the user is not logged in:

The system creates a temporary patient record.

This logic is handled in:

AppointmentService
📘 File: AppointmentDto.cs
Responsibility

This DTO represents the data returned to the client when retrieving appointments.

Example endpoint:

GET /api/appointments
Structure
public class AppointmentDto
{
    public int Id { get; set; }
    public DateTime AppointmentDate { get; set; }
    public int DurationMinutes { get; set; }

    public int ClinicId { get; set; }
    public string ClinicName { get; set; }

    public int DoctorId { get; set; }
    public string DoctorName { get; set; }

    public int CategoryId { get; set; }
    public string CategoryName { get; set; }

    public int? PatientId { get; set; }
}
Why This DTO Exists

Instead of returning the database model:

Appointment
Doctor
Clinic
Category

The API returns flattened information.

Example response:

{
  "id": 1,
  "appointmentDate": "2026-05-20T10:00:00",
  "durationMinutes": 30,
  "clinicId": 2,
  "clinicName": "Bergen Clinic",
  "doctorId": 4,
  "doctorName": "Dr. Smith",
  "categoryId": 1,
  "categoryName": "Consultation"
}

This is easier for the frontend to use.

Why #nullable disable Is Used
#nullable disable

This disables C# nullable reference warnings.

It ensures fields like:

ClinicName
DoctorName
CategoryName

are treated as non-nullable.

Because they are always populated in the service mapping.

📘 File: AppointmentUpdateDto.cs
Responsibility

Used when updating an existing appointment.

Example endpoint:

PUT /api/appointments/{id}
Structure
public class AppointmentUpdateDto
{
    [Required]
    public DateTime AppointmentDate { get; set; }

    [Required]
    [Range(15, 60, ErrorMessage = "Duration must be 15, 30, 45, or 60 minutes.")]
    public int DurationMinutes { get; set; }
}
Validation
Required
[Required]

The client must send these fields.

Range Validation
[Range(15, 60)]

Limits allowed values.

This prevents invalid durations.

Further validation happens in:

AppointmentService
Why DTOs Are Important

DTOs provide:

1️⃣ Security
Prevent exposing internal database models.

2️⃣ API flexibility
Database structure can change without breaking the API.

3️⃣ Validation
DataAnnotations enforce request validation.

4️⃣ Better API responses
Return only relevant data.

Interview Questions
Q: What is a DTO?

A DTO is a Data Transfer Object used to define the structure of data exchanged between the client and the API.

Q: Why not return EF models directly?

Because EF models represent database structure and may expose unnecessary or sensitive fields.

Q: What do DataAnnotations like [Required] do?

They enforce validation rules for incoming API requests.

Q: Why have different DTOs for Create, Update, and Response?

Because each operation requires different data.

Self Check

You understand DTOs if you can explain:

Why DTOs exist

Why Create/Update/Response DTOs are different

How validation attributes work

How DTO mapping happens in services

Next Step (Final Step of the Cycle)

Now we finish the full flow:

Model
↓
DataContext
↓
Service
↓
DTO
↓
Controller