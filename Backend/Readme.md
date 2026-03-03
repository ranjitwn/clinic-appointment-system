# ExamProject2.API – Clinic Appointment Backend (.NET 9)

This project is a REST API built with **ASP.NET Core (.NET 9)** and **Entity Framework Core** using **MySQL** as the database.
It supports clinic appointment booking with patient authentication, guest booking, admin management, and JWT security.

---

## 1. Application Setup Instructions

1. Clone or download the project.
2. Open the solution in **Visual Studio Code**.
3. Ensure the **.NET 9 SDK** is installed.
4. Navigate to the backend project folder:

```bash
cd Backend
cd ExamProject2.API
```

5. Run the application:

```bash
dotnet run
```

6. The API URL will appear in the terminal
   (example: `http://localhost:5108`).

7. Swagger UI can be accessed at:

```
http://localhost:5108/doc
```

---

## 2. Instructions to Run the Application

1. Ensure **MySQL Server** is installed.
2. Create the database:

```sql
CREATE DATABASE examproject2db;
```

3. Update the connection string inside **appsettings.json** with your MySQL username and password.  
4. Apply database migrations (see section 3).
5. Start the API:

```bash
dotnet run
```

---

## 3. Instructions to Create Needed Migrations

Inside the **ExamProject2.API** folder:

### Apply migration

```bash
dotnet ef database update
```

This will:

* Apply all existing EF Core migrations in the project

* Create the database schema (tables, keys, relationships)

* Reflect any incremental schema changes created during development

Note:
Role-based authorization is implemented to restrict administrative CRUD operations.
---

## 4. Connection String Format (MySQL)

Inside **appsettings.json**:

```json
"ConnectionStrings": {
  "DefaultConnection": "server=localhost;database=examproject2db;user=YOUR_USERNAME;password=YOUR_PASSWORD"
}
```

Replace:

* `YOUR_USERNAME`
* `YOUR_PASSWORD`

with your MySQL credentials.

---

## 5. External Packages Used

| Package                                       | Purpose                    |
| --------------------------------------------- | -------------------------- |
| MySql.EntityFrameworkCore                     | EF Core provider for MySQL |
| Microsoft.EntityFrameworkCore.Design          | EF Core migrations         |
| Swashbuckle.AspNetCore                        | Swagger documentation      |
| Microsoft.AspNetCore.Authentication.JwtBearer | JWT Authentication         |
| System.IdentityModel.Tokens.Jwt               | Token creation/validation  |

---

## 6. Project Structure

```
ExamProject2.API/
│
├── Controllers/
│   ├── AppointmentController.cs
│   ├── AuthController.cs
│   ├── CategoryController.cs
│   ├── ClinicController.cs
│   ├── DoctorController.cs
│   ├── PatientController.cs
│   └── SpecialityController.cs
│
├── Data/
│   └── DataContext.cs
│
├── Models/
│   ├── Appointment.cs
│   ├── Category.cs
│   ├── Clinic.cs
│   ├── Doctor.cs
│   ├── Patient.cs
│   ├── Speciality.cs
│   └── JwtSettings.cs
│
├── DTOs/
│   ├── AppointmentCreateDto.cs
│   ├── AppointmentUpdateDto.cs
│   ├── CategoryCreateDto.cs
│   ├── ClinicCreateDto.cs
│   ├── DoctorCreateDto.cs
│   ├── DoctorSearchDto.cs
│   ├── PatientCreateDto.cs
│   ├── PatientLoginDto.cs
│   ├── PatientRegisterDto.cs
│   ├── AuthLoginResultDto.cs
│   └── SpecialityCreateDto.cs
│
├── Services/
│   ├── AppointmentService.cs
│   ├── AuthService.cs
│   ├── CategoryService.cs
│   ├── ClinicService.cs
│   ├── DoctorService.cs
│   ├── PatientService.cs
│   └── SpecialityService.cs
│
├── Middleware/
│   └── GlobalExceptionMiddleware.cs
│
├── Migrations/
├── appsettings.json
├── Program.cs
└── README.md
```

---

## 7. Authentication & Authorization

### JWT Authentication

* Registered patients can login.
* Guests can book appointments without login.
* JWT token includes:

  * PatientId
  * Email
  * Role claim.

The login endpoint returns authentication data using a dedicated DTO 
(AuthLoginResultDto), providing the JWT token and role in a structured 
JSON response format.

**Note:** Role claim was added to support basic authorization (Admin vs Patient), improving API security.
---

### Roles

Roles implemented:

* Patient – default role for registered users

* Admin – management role for system configuration

Admin-only actions include:

* Creating, updating, and deleting clinics

* Creating, updating, and deleting doctors

* Creating, updating, and deleting Categories

* Creating, updating, and deleting specialities

Example authorization usage:

```csharp
[Authorize(Roles = "Admin")]
```

Note: 
Role-based authorization was added as an enhancement.
This ensures that sensitive management actions (such as creating or modifying clinics, doctors, categories, and specialities) are restricted to authorized Admin users only, while regular patients can only access their own relevant functionality.

This improvement does not change any required project functionality and added as a security best practice.
---

## 8. Global Exception Middleware

A custom `GlobalExceptionMiddleware` is included to provide:

* Consistent JSON error responses
* Centralized exception handling
* Cleaner controller logic
* Improved debugging

Reference documentation used:

* Microsoft ASP.NET Core middleware documentation
* ASP.NET Core official exception handling guidance

This follows backend course principles for structured API error handling.

---

## 9. Appointment Booking Logic

The system prevents invalid bookings:

### Slot Conflict Validation

Prevents:

* Same doctor overlapping bookings
* Same clinic conflicts
* Overlapping appointment time

---

### Patient Conflict Validation

* Prevents a patient from booking multiple overlapping appointments.

---

### Additional Appointment Validation Rules

The system also enforces:

* No past appointments
* No weekend bookings (Saturday/Sunday)
* Clinic hours: **08:00–18:00**
* Appointment must finish before closing time
* Allowed durations:

  * 15 minutes
  * 30 minutes
  * 45 minutes
  * 60 minutes
* Time intervals must align to:

  * :00, :15, :30, :45

These ensure realistic scheduling.

### Available Slots Note

An endpoint is provided to generate available appointment time slots based on:

- Clinic working hours (08:00–18:00)
- Existing booked appointments
- Selected appointment duration

Duration validation is enforced during appointment creation and update.
The available slots endpoint assumes valid duration input and serves as a helper for frontend scheduling.

This approach keeps business validation centralized while still providing usable scheduling guidance.


## 10. Data Validation

DTO validation is implemented using DataAnnotations:

Examples:

* Required fields
* Email format validation
* Duration must be greater than zero
* Server-side control of registration status.

---

## 11. Service Layer Architecture

Business logic is implemented inside the **Services folder**:

Benefits:

* Cleaner controllers
* Better separation of concerns
* Easier future maintenance
* Industry-aligned structure.

Controllers now:

* Handle HTTP requests
* Call services
* Return responses.

## 11.1 Response Handling Approach
Business logic is handled in the service layer.
Most services return DTOs or boolean results.
Some endpoints (such as appointment creation and update) return validation messages as strings to provide clearer API feedback while keeping business logic inside the service layer.
All responses are returned as JSON objects from controllers to maintain consistent API behaviour.

* Consistent JSON response handling across all endpoints

---

## 12. CORS Configuration

CORS enabled for frontend communication during development:

Example allowed origin:

```
http://localhost:5173
```

This allows integration with the React frontend during development.

Configuration reference:

**Microsoft ASP.NET Core Security Documentation**
- Microsoft ASP.NET Core Security
https://learn.microsoft.com/en-us/aspnet/core/security/cors


---

## 13. Admin User Seeding

An Admin user is automatically created during application startup if none exists.

Configuration stored in `appsettings.json`:

```json
"SeedAdmin": {
  "Email": "admin@clinic.no",
  "Password": "Admin123!"
}
```

Startup process:

* Checks if Admin exists
* Creates one if missing
* Password is securely hashed before storage

### Admin Role Implementation Design

This project does not use a separate Admin model or table. Instead, the Admin user is stored in the existing Patients table as a registered patient with a role assignment:

- Role = "Admin"

- IsRegistered = true

- Password stored hashed

This approach:

-  Keeps authentication unified in one user table

- Avoids duplicate user entities

- Enables role-based authorization via JWT claims

- Aligns with common role-based access control practices

This allows immediate administrative access after setup while maintaining a simple and secure data model.
---

## 14. Patient Data & Privacy Consideration

The exam brief references extended patient personal information.
In this implementation, only essential appointment-related data is stored:

* First name
* Last name
* Email
* Date of birth
* Gender

Sensitive identifiers (SSN, tax number, religion, Driver’s license number and medical insurance membership number) were intentionally not stored due to privacy and security considerations.

This approach:

* Minimizes sensitive data exposure
* Supports GDPR-conscious design
* Still satisfies functional booking requirements

In production healthcare systems, such data would require:

* Encryption
* Strict access control
* Regulatory compliance.

---

## 15. Security Note (Development Configuration)

For exam development purposes:

* Database connection string
* JWT secret key
* Admin seed credentials

are stored in `appsettings.json`.

In production environments, these should be moved to:

* Environment variables
* Secret managers
* Secure configuration providers.

---

## 16. Additional Endpoints Added

Beyond minimum exam requirements:

* Doctor filtering by clinic/speciality
* Appointment filtering endpoints (admin overview)
* Doctor search endpoint
* Available appointment slots endpoint

These enhance usability while maintaining required functionality.

---

## 17. Summary

This backend includes:

* Full clinic appointment REST API
* Guest + registered patient workflows
* JWT authentication
* Role-based authorization
* DTO validation
* Appointment conflict validation
* Service-layer business logic
* Global exception middleware
* Swagger documentation
* MySQL EF Core database with migrations
* Admin seeding

---


