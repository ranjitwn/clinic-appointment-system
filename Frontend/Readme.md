# Clinic Appointment Frontend (React + TypeScript + Vite)

This project is the frontend application for **Exam Project 2 – Clinic Appointment Booking System**.
It communicates with the ASP.NET Core backend API and provides patient booking, authentication, search, and admin management interfaces.

---

## 1. Application Setup Instructions

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

> Make sure backend API is running before starting the frontend.

---

## 2. Environment Configuration

Frontend uses an environment variable for backend connection.

File:

```
.env.local
```

Example:

```
VITE_API_BASE_URL=http://localhost:5108
```

This must match the backend API URL.

---

## 3. Technologies Used

Core technologies:

* React 18
* TypeScript
* Vite
* React Router DOM
* Fetch API for backend communication

Development tooling:

* ESLint (strict TypeScript configuration)
* Vite build tooling (@vitejs/plugin-react for React Fast Refresh and JSX support)

---

## 4. Project Structure

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

## 5. Authentication Flow

### Guest User

* Can book appointments without login.
* Must provide personal details during booking.
* Cannot manage appointments unless they later register using the same email.
* If the guest registers with the same email address, the existing guest record is upgraded to a registered patient and previous appointments become accessible.

### Registered Patient

* Can register and login.
* JWT token stored in localStorage.
* Can view, update, and cancel their own appointments.

### Admin User

* Admin login uses same authentication system as patients.
* Role-based UI access enabled.
* Admin dashboard allows managing clinics, doctors, categories, specialities, and viewing appointments.

---

## 6. Routing Overview

Routes implemented:

* `/` → Appointment booking page
* `/book` → Booking page
* `/search` → Doctor search
* `/login` → Patient login
* `/register` → Patient registration
* `/appointments` → Patient appointment management (protected)
* `/admin` → Admin dashboard (role protected)

Protected routes use role-based authorization from stored JWT token.

---

## 7. UI Layout

Common layout components:

* Header navigation on all pages
* Footer displaying current year
* Shared popup notification component
* Reusable button components

This ensures consistent UI across the application.

---

## 8. Backend Communication

All API communication is handled inside the `services` folder.

Typical service responsibilities:

* Fetch clinics, doctors, categories
* Appointment booking and updates
* Authentication requests
* Admin CRUD operations

Authorization headers are automatically included for protected endpoints.

---

## 9. Validation & User Feedback

Frontend validation includes:

* Required field validation
* Date of birth validation
* Appointment time selection validation
* Popup error/success messages

Loading indicators are shown during API calls.

---

## 10. CORS Integration

Frontend communicates with backend using configured CORS policy.

Backend allows:

```
http://localhost:5173
```

This enables frontend-backend communication during development.

---

## 11. Security Notes

* JWT tokens stored in localStorage for authentication.
* No sensitive secrets stored in frontend.
* API base URL controlled via environment variables.

For production:

* Use secure token storage strategy.
* Use HTTPS.
* Configure secure environment variables.

---

## 12. Summary

This frontend provides:

* Clinic appointment booking interface
* Guest and registered patient workflows
* JWT-based authentication UI
* Role-based admin dashboard
* Doctor search functionality
* Responsive UI with reusable components
* Integration with ASP.NET Core backend API

---
