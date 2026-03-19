# Backend — ClinicAppointment.API

ASP.NET Core Web API (.NET 9) serving the Clinic Appointment Booking System. Handles authentication, business logic, appointment scheduling, and all data access through Entity Framework Core.

> For full project context, deployment details, and screenshots see the [root README](../../README.md).

---

## Responsibilities

- JWT authentication for patients and admins
- Appointment booking with conflict detection and slot availability
- Role-based access control (`Patient`, `Admin`)
- CRUD management for clinics, doctors, categories, and specialities
- Guest booking without registration
- Global exception handling and structured request logging
- Health check endpoint

---

## Project Structure

```
ClinicAppointment.API/
├── Controllers/        # HTTP endpoints — thin, delegates to services
│   ├── AppointmentController.cs
│   ├── AuthController.cs
│   ├── ClinicController.cs
│   ├── DoctorController.cs
│   ├── CategoryController.cs
│   ├── SpecialityController.cs
│   └── PatientController.cs
├── Services/           # Business logic — all depend on interfaces
│   ├── AppointmentService.cs
│   ├── AuthService.cs
│   └── ...
├── Models/             # EF Core entities
├── DTOs/               # Request and response shapes
├── Data/               # DataContext (EF Core DbContext)
├── Middleware/         # GlobalExceptionMiddleware, RequestLoggingMiddleware
└── Program.cs          # DI registration, middleware pipeline, admin seeder
```

---

## Architecture

Requests flow through a single pipeline:

```
HTTP Request → Controller → Service (via interface) → DataContext → MySQL
```

Controllers are kept thin — they parse claims, call one service method, and return a result. All business logic lives in the service layer. Services are registered against their interfaces in `Program.cs`, which keeps controllers decoupled and unit-testable.

---

## Authentication

JWT Bearer tokens are issued by `AuthController` and validated on protected endpoints via the ASP.NET Core middleware pipeline.

**Token claims:**

| Claim | Value |
|---|---|
| `NameIdentifier` | User/Patient `Id` |
| `Email` | Account email |
| `Role` | `Patient` or `Admin` |

**Login flow:**
1. `AuthService.LoginAsync` looks up the email in the `Patients` table first
2. If not found (or not registered), it falls back to the `Users` table (admin accounts)
3. Password is verified using `Microsoft.AspNetCore.Identity.PasswordHasher`
4. On success, a signed JWT is returned with the role claim

This means patient and admin authentication share a single `/api/Auth/login` endpoint with no change to the JWT structure.

**Endpoint protection:**

```csharp
[Authorize(Roles = "Patient")]   // patient-only endpoints
[Authorize(Roles = "Admin")]     // admin-only endpoints
// No attribute = public (guest booking, doctor search)
```

---

## Database Design

Schema is managed with EF Core Code-First migrations against **Azure Database for MySQL**.

**Key entities and relationships:**

| Entity | Key relationships |
|---|---|
| `Patient` | Has many `Appointments`; stores registered and guest patients |
| `User` | Admin accounts — separate table, not mixed with patients |
| `Doctor` | Belongs to one `Clinic`, has one `Speciality` |
| `Appointment` | Links `Patient`, `Doctor`, `Clinic`, `Category` |
| `Clinic` | Has many `Doctors` and `Appointments` |
| `Category` | Classifies appointment type (check-up, consultation, etc.) |

**Why `User` is separate from `Patient`:**
Admin accounts were originally stored in the `Patient` table using a `Role` field. A dedicated `User` entity was added to separate concerns at the data model level. Both still share the same login endpoint — `AuthService` checks `Patients` first, then falls back to `Users`.

---

## Appointment Scheduling Logic

`AppointmentService` contains the core booking rules:

- Appointments must fall within clinic hours (08:00–18:00)
- Weekends are blocked
- Time must align to 15-minute intervals (`:00`, `:15`, `:30`, `:45`)
- Duration must be 15, 30, 45, or 60 minutes
- Appointment must finish within clinic hours
- **Doctor conflict check** — no overlapping bookings for the same doctor
- **Patient conflict check** — a patient cannot have two overlapping appointments

Overlap detection uses a strict interval formula (`newStart < existingEnd && newEnd > existingStart`), which means adjacent back-to-back slots are correctly allowed.

Clinic hours are defined as named constants (`ClinicOpeningHour`, `ClinicClosingHour`) rather than magic numbers. The doctor conflict query filters by `DoctorId` only — filtering by `ClinicId` in addition would be redundant since `DoctorId` already implies a single clinic via FK.

---

## API Endpoints

Full interactive documentation: [`/doc`](https://api.ranjitnair.dev/doc) (Swagger UI)

| Method | Route | Auth | Description |
|---|---|---|---|
| `POST` | `/api/Auth/register` | Public | Register a patient |
| `POST` | `/api/Auth/login` | Public | Login (patient or admin) |
| `POST` | `/api/Appointment` | Public / Patient | Book appointment |
| `GET` | `/api/Appointment/my` | Patient | Get own appointments |
| `PUT` | `/api/Appointment/{id}` | Patient | Reschedule appointment |
| `DELETE` | `/api/Appointment/{id}` | Patient | Cancel appointment |
| `GET` | `/api/Appointment/available-slots` | Public | Get available slots |
| `GET` | `/api/Appointment/clinic/{id}` | Admin | Appointments by clinic |
| `GET` | `/api/Appointment/doctor/{id}` | Admin | Appointments by doctor |
| `GET` | `/api/Doctor/search` | Public | Search doctors by name |
| `GET/POST/PUT/DELETE` | `/api/Clinic` | Admin (write) | Manage clinics |
| `GET/POST/PUT/DELETE` | `/api/Doctor` | Admin (write) | Manage doctors |
| `GET/POST/PUT/DELETE` | `/api/Category` | Admin (write) | Manage categories |
| `GET/POST/PUT/DELETE` | `/api/Speciality` | Admin (write) | Manage specialities |

---

## Error Handling

A global exception middleware (`GlobalExceptionMiddleware`) catches any unhandled exceptions and returns a consistent JSON error response, preventing stack traces from reaching the client.

`RequestLoggingMiddleware` logs incoming requests for observability.

Business logic errors are returned as `400 Bad Request` with a `{ "message": "..." }` body — the message is always user-readable (e.g. `"Doctor is already booked at this time."`), never an internal error string.

---

## Running Locally

### Prerequisites
- .NET 9 SDK
- MySQL Server

### Setup

```bash
cd Backend/ClinicAppointment.API
dotnet restore
```

Add `appsettings.Development.json` (not committed):

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "server=localhost;port=3306;database=clinicappointmentdb;user=root;password=yourpassword"
  },
  "JwtSettings": {
    "SecretKey": "your-secret-key-at-least-32-characters",
    "Issuer": "ClinicAppointmentAPI",
    "Audience": "ClinicAppointmentClient",
    "ExpiryMinutes": 60
  },
  "SeedAdmin": {
    "Email": "admin@example.com",
    "Password": "YourAdminPassword"
  }
}
```

Apply migrations and run:

```bash
dotnet ef database update
dotnet run
```

- API: `http://localhost:5108`
- Swagger: `http://localhost:5108/doc`

On startup, the application seeds an admin account into the `Users` table using the `SeedAdmin` config values, if no admin exists yet.

---

## Testing

```bash
cd Backend/ClinicAppointment.Tests
dotnet test
```

Tests use EF Core InMemory — no database required.

**`AppointmentServiceTests`** covers:
- Doctor slot conflict at exact same time
- Doctor slot conflict with partial overlap
- Adjacent slots are correctly allowed (boundary condition)
- Patient double-booking prevention

**`AuthServiceTests`** covers:
- Patient registration (new and guest upgrade)
- Registration rejection for already-registered email
- Login success and JWT return
- Login failure for wrong password or unregistered patient

Tests run automatically in the CI/CD pipeline before each deployment.

---

## Configuration

All sensitive values are stored as Azure App Service environment variables in production:

| Key | Purpose |
|---|---|
| `ConnectionStrings__DefaultConnection` | MySQL connection string |
| `JwtSettings__SecretKey` | JWT signing key |
| `JwtSettings__Issuer` | JWT issuer claim |
| `JwtSettings__Audience` | JWT audience claim |
| `JwtSettings__ExpiryMinutes` | Token lifetime in minutes |
| `SeedAdmin__Email` | Initial admin email |
| `SeedAdmin__Password` | Initial admin password |

Nothing sensitive is committed to the repository.
