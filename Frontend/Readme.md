# Clinic Appointment Frontend (React + TypeScript + Vite)

This project is the **frontend application for the Clinic Appointment Booking System**. It provides the user interface for patients and administrators and communicates with the **ASP.NET Core Web API backend**.

The system demonstrates a modern full‑stack architecture using **React, TypeScript, and Vite on the frontend**, combined with an **ASP.NET Core REST API and MySQL database on the backend**, deployed to **Microsoft Azure**.

---

# 1. Application Setup Instructions

1. Open terminal inside the frontend folder:

```bash
cd Frontend
```

2. Install dependencies:

```bash
npm install
```

3. Start development server:

```bash
npm run dev
```

4. The application will run on:

```
http://localhost:5173
```

⚠️ Ensure the **backend API is running** before starting the frontend.

---

# 2. Environment Configuration

The frontend uses an environment variable to define the backend API base URL.

File:

```
.env.local
```

Example configuration for local development:

```
VITE_API_BASE_URL=http://localhost:5108
```

This must match the running backend API URL.

For production deployment, the variable should point to the deployed API endpoint.

---

# 3. Technologies Used

## Frontend Framework

* React 18
* TypeScript
* Vite

## Routing

* React Router DOM

## Backend Communication

* Fetch API

## Development Tooling

* ESLint with strict TypeScript configuration
* Vite build tooling
* @vitejs/plugin-react for React Fast Refresh and JSX support

---

# 4. Project Structure

```
Frontend/
│
├── src/
│   ├── components/
│   │   ├── appointments/
│   │   │   ├── AppointmentCard.tsx
│   │   │   └── AppointmentForm.tsx
│   │   ├── auth/
│   │   │   ├── AuthCard/
│   │   │   │   └── index.tsx
│   │   │   ├── LoginForm/
│   │   │   │   └── index.tsx
│   │   │   └── RegisterForm/
│   │   │       └── index.tsx
│   │   ├── Button/
│   │   │   └── index.tsx
│   │   ├── DoctorCard/
│   │   │   └── index.tsx
│   │   ├── layout/
│   │   │   ├── Footer/
│   │   │   │   └── index.tsx
│   │   │   ├── Header/
│   │   │   │   └── index.tsx
│   │   │   └── Layout.tsx
│   │   └── ui/
│   │       └── Popup.tsx
│   │
│   ├── context/
│   │   ├── AuthContext.tsx
│   │   ├── AuthProvider.tsx
│   │   └── useAuth.ts
│   │
│   ├── pages/
│   │   ├── AdminPage/
│   │   │   └── index.tsx
│   │   ├── HomePage/
│   │   │   └── index.tsx
│   │   ├── LoginPage/
│   │   │   └── index.tsx
│   │   ├── MyAppointmentsPage/
│   │   │   └── index.tsx
│   │   ├── RegisterPage/
│   │   │   └── index.tsx
│   │   └── SearchDoctorPage/
│   │       └── index.tsx
│   │
│   ├── routes/
│   │   └── ProtectedRoute.tsx
│   │
│   ├── services/
│   │   ├── api.ts
│   │   ├── appointmentService.ts
│   │   ├── authHeaders.ts
│   │   ├── authService.ts
│   │   ├── categoryService.ts
│   │   ├── clinicService.ts
│   │   ├── doctorService.ts
│   │   └── specialityService.ts
│   │
│   ├── types/
│   │   ├── AppointmentCreateDTO.ts
│   │   ├── AppointmentDTO.ts
│   │   ├── CategoryCreateDTO.ts
│   │   ├── CategoryDTO.ts
│   │   ├── ClinicCreateDTO.ts
│   │   ├── ClinicDTO.ts
│   │   ├── DoctorCreateDTO.ts
│   │   ├── DoctorDTO.ts
│   │   ├── DoctorSearchDTO.ts
│   │   ├── PatientLoginDTO.ts
│   │   ├── PatientRegisterDTO.ts
│   │   ├── SpecialityCreateDTO.ts
│   │   └── SpecialityDTO.ts
│   │
│   ├── App.css
│   ├── App.tsx
│   ├── index.css
│   └── main.tsx
│
├── .env.local
├── .gitignore
├── eslint.config.js
├── index.html
├── package.json
├── package-lock.json
├── tsconfig.json
├── tsconfig.app.json
├── tsconfig.node.json
├── vite.config.ts
└── Readme.md
```

---

# 5. Authentication Flow

## Guest User

* Can book appointments without creating an account.
* Must provide personal details during booking.
* Cannot manage appointments unless they later register using the same email.
* If a guest later registers using the same email address, the existing guest record is upgraded to a registered patient and previous appointments become accessible.

## Registered Patient

* Can register and log in.
* JWT token stored in localStorage.
* Can view, update, and cancel their own appointments.

## Admin User

* Uses the same authentication system as patients.
* Role‑based UI access is enabled.
* Admin dashboard allows managing clinics, doctors, categories, specialities, and viewing appointments.

---

# 6. Routing Overview

Routes implemented:

* `/` → Home page
* `/book` → Appointment booking page
* `/search` → Doctor search
* `/login` → Patient login
* `/register` → Patient registration
* `/appointments` → Patient appointment management (protected)
* `/admin` → Admin dashboard (role protected)

Protected routes use role‑based authorization derived from the stored JWT token.

---

# 7. UI Layout

Common layout components:

* Header navigation displayed on all pages
* Footer displaying the current year
* Shared popup notification component
* Reusable button components

This ensures consistent UI across the application.

---

# 8. Backend Communication

All API communication is handled inside the `services` folder.

Typical service responsibilities include:

* Fetch clinics, doctors, and categories
* Appointment booking and updates
* Authentication requests
* Admin CRUD operations

Authorization headers are automatically included for protected endpoints.

---

# 9. System Architecture

The frontend communicates with the backend through REST API calls.

```
React Frontend
      ↓
API Services (Fetch)
      ↓
ASP.NET Core Controllers
      ↓
Service Layer
      ↓
Entity Framework Core
      ↓
MySQL Database
```

This separation keeps the frontend responsible for **user interaction and presentation**, while the backend handles **business logic, validation, authentication, and data persistence**.

---

# 10. Validation & User Feedback

Frontend validation includes:

* Required field validation
* Date of birth validation
* Appointment time selection validation
* Popup error and success messages

Loading indicators are displayed during API calls.

---

# 11. CORS Integration

During development the backend allows requests from:

```
http://localhost:5173
```

This enables local frontend–backend communication.

---

# 12. Security Notes

* JWT tokens stored in localStorage for authentication.
* No sensitive secrets stored in the frontend application.
* API base URL managed through environment variables.

For production deployments:

* HTTPS must be enforced.
* Secure environment variables should be configured.

---

# 13. Summary

This frontend application provides:

* Clinic appointment booking interface
* Guest and registered patient workflows
* JWT‑based authentication UI
* Role‑based admin dashboard
* Doctor search functionality
* Responsive UI with reusable components
* Integration with the ASP.NET Core backend API

Together with the backend API, the system demonstrates a **full‑stack clinic appointment booking platform built with ASP.NET Core, React, TypeScript, and MySQL and deployed to Microsoft Azure**.
