# Clinic Appointment Booking System

A full-stack clinic appointment booking platform built with **ASP.NET Core (.NET 9)** and **React + TypeScript**, deployed on **Microsoft Azure** with automated CI/CD via GitHub Actions.

**Live application:** [app.ranjitnair.dev](https://app.ranjitnair.dev)
**API:** [api.ranjitnair.dev](https://api.ranjitnair.dev)
**Swagger docs:** [api.ranjitnair.dev/doc](https://api.ranjitnair.dev/doc)
**Health check:** [api.ranjitnair.dev/health](https://api.ranjitnair.dev/health)

---

## Overview

The platform supports two user types — **patients** and **admins** — each with separate authentication flows and access controls.

Patients can register, search for doctors, book appointments with real-time slot availability, reschedule, and cancel. Guest booking is also supported without registration. Admins manage clinics, doctors, categories, and specialities through a dedicated dashboard.

The project has been extended with production-quality improvements across architecture, testing, security, error handling, and UI/UX.

---

## Highlights

- Secure JWT authentication with role-based authorization
- EF Core Code-First with Azure MySQL
- Full CI/CD pipeline using GitHub Actions
- Cloud deployment on Microsoft Azure

---

## Key Improvements (Beyond Original Exam)

These improvements were made after the initial submission to bring the project closer to production quality:

- **Service interfaces** — All services depend on interfaces (`IAppointmentService`, `IDoctorService`, etc.), following the Dependency Inversion Principle and enabling clean unit testing
- **Separate User entity for admins** — Admins are stored in a dedicated `Users` table instead of the `Patients` table, improving database design without breaking existing API behaviour
- **Defensive controller parsing** — Replaced `int.Parse` with `int.TryParse` throughout all controllers, eliminating potential runtime exceptions from malformed claims
- **Named constants for business rules** — Clinic opening hours extracted into `private const int` fields in `AppointmentService`, removing magic numbers
- **Unit tests for conflict detection** — xUnit tests covering doctor slot conflicts, patient double-booking, and boundary conditions (e.g. adjacent slots that should not block each other)
- **Centralised frontend error handling** — All API calls go through a shared `fetchJson` wrapper that returns user-friendly messages for network failures, non-JSON responses, and HTTP error codes (401, 403, 500+)
- **Professional UI redesign** — Complete CSS rewrite using design tokens (custom properties), clean healthcare colour palette, consistent component library including loading spinners, empty states, and toast notifications
- **Redundant query cleanup** — Removed a redundant `ClinicId` filter from doctor conflict queries, since `DoctorId` already implies a single clinic by foreign key constraint

---

## Features

- JWT authentication with role-based authorization (`Patient`, `Admin`)
- Guest appointment booking without registration
- Doctor search with clinic and speciality information
- Real-time slot availability based on clinic hours and existing bookings
- Appointment conflict prevention (doctor and patient level)
- Patient appointment management (book, reschedule, cancel)
- Admin dashboard for managing clinics, doctors, categories, and specialities
- Global exception middleware with structured request logging
- Health check endpoint
- Swagger / OpenAPI documentation with XML comments

---

## Tech Stack

### Backend
- ASP.NET Core Web API (.NET 9)
- Entity Framework Core — Code-First migrations
- MySQL (Azure Database for MySQL)
- JWT Bearer authentication
- xUnit + EF Core InMemory (unit testing)
- Swagger / Swashbuckle

### Frontend
- React 18 + TypeScript
- Vite
- React Router v6
- Fetch API with centralised error handling
- ESLint (strict TypeScript configuration)

### Cloud & DevOps
- Azure App Service (backend)
- Azure Static Web Apps (frontend)
- Azure Database for MySQL
- GitHub Actions (CI/CD)
- Custom domain via Namecheap DNS

---

## Architecture

The application follows a standard three-layer architecture:

| Layer | Technology |
|---|---|
| Client | React + TypeScript (Azure Static Web Apps) |
| API | ASP.NET Core Web API (Azure App Service) |
| Data | MySQL via EF Core (Azure Database for MySQL) |

Frontend communicates with the backend over HTTPS REST APIs with JSON responses. Sensitive configuration (connection strings, JWT keys, admin seed credentials) is stored as Azure environment variables — nothing sensitive is committed to the repository.

![System Architecture](docs/architecture-diagram.png)

---

## Database Design

The schema is built around the `Appointments` entity, which links patients, doctors, clinics, and appointment categories.

Key relationships:
- A **Doctor** belongs to one **Clinic** and has one **Speciality**
- A **Patient** can have multiple **Appointments**
- Each **Appointment** records the **Doctor**, **Clinic**, **Category**, date, and duration
- **Admin** accounts are stored in a dedicated **Users** table, separate from patients

![ER Diagram](docs/er-diagram.png)

---

## Screenshots

| Doctor Search | Booking Form |
|---|---|
| ![Doctor Search](docs/screenshots/doctor-search.png) | ![Booking Form](docs/screenshots/booking-form.png) |

| Patient Dashboard | Admin Management |
|---|---|
| ![Patient Dashboard](docs/screenshots/patient-dashboard.png) | ![Admin Management](docs/screenshots/admin-management.png) |

---

## Quick Start

### Prerequisites
- .NET 9 SDK
- Node.js 18+
- MySQL Server

### 1. Clone

```bash
git clone https://github.com/ranjitwn/clinic-appointment-system.git
cd clinic-appointment-system
```

### 2. Database

Create a local MySQL database:

```sql
CREATE DATABASE clinicappointmentdb;
```

Add your connection string to `Backend/ClinicAppointment.API/appsettings.Development.json`:

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

### 3. Backend

```bash
cd Backend/ClinicAppointment.API
dotnet restore
dotnet ef database update
dotnet run
```

Runs at `http://localhost:5108` — Swagger at `http://localhost:5108/doc`

### 4. Frontend

Create `Frontend/.env.local`:

```
VITE_API_BASE_URL=http://localhost:5108
```

```bash
cd Frontend
npm install
npm run dev
```

Runs at `http://localhost:5173`

---

## Testing

Unit tests cover the appointment service layer using EF Core InMemory — no external database required.

Tested scenarios include:
- Doctor slot conflict (exact time, partial overlap, adjacent boundary)
- Patient double-booking prevention
- Auth service registration and login logic

```bash
dotnet test
```

Tests run automatically in the CI/CD pipeline before each deployment.

---

## CI/CD

GitHub Actions deploys automatically on push to `main`:

1. Build and test backend
2. Deploy API to Azure App Service
3. Build frontend
4. Deploy to Azure Static Web Apps

![Deployment Architecture](docs/deployment-diagram.png)

---

## Project Structure

```
clinic-appointment-system/
├── Backend/
│   └── ClinicAppointment.API/     # ASP.NET Core Web API
│   └── ClinicAppointment.Tests/   # xUnit test project
├── Frontend/                      # React + TypeScript (Vite)
├── docs/                          # Diagrams and screenshots
└── README.md
```

Detailed documentation:
- [Backend README](Backend/ClinicAppointment.API/README.md)
- [Frontend README](Frontend/README.md)
