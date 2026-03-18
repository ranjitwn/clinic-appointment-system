# Frontend — React + TypeScript

React 18 + TypeScript frontend for the Clinic Appointment Booking System, built with Vite and deployed on Azure Static Web Apps.

> For full project context, deployment details, and screenshots see the [root README](../README.md).

---

## Responsibilities

- Patient registration, login, and session management
- Guest and registered appointment booking with real-time slot selection
- Patient appointment management (reschedule, cancel)
- Debounced doctor search
- Admin dashboard for managing clinics, doctors, categories, and specialities
- Role-based route protection (`Patient`, `Admin`)
- User-friendly error feedback across all API interactions

---

## Project Structure

```
src/
├── pages/
│   ├── HomePage/             # Booking form (guest + registered patient)
│   ├── SearchDoctorPage/     # Debounced doctor search
│   ├── MyAppointmentsPage/   # Appointment list + inline reschedule
│   ├── AdminPage/            # Tabbed admin dashboard
│   ├── LoginPage/
│   └── RegisterPage/
├── components/
│   ├── appointments/         # AppointmentCard, AppointmentForm
│   ├── auth/                 # AuthCard (shared login/register wrapper)
│   ├── layout/               # Header, Footer, Layout
│   ├── ui/                   # Spinner, EmptyState, Popup, Button
│   └── DoctorCard/
├── services/
│   ├── apiHelpers.ts         # Shared fetch wrapper + error mapping
│   ├── appointmentService.ts
│   ├── authService.ts
│   ├── doctorService.ts
│   └── ...                   # One file per backend resource
├── context/                  # AuthContext — token + role
├── routes/                   # ProtectedRoute
└── types/                    # TypeScript DTOs matching backend responses
```

---

## Authentication and Routing

Auth state (JWT token and role) is stored in `localStorage` and exposed application-wide via `AuthContext`. The context provides `login()` and `logout()` actions used across the login page, register page, and header nav.

Route protection is handled by `ProtectedRoute`:

```tsx
<ProtectedRoute>            // requires any valid token
<ProtectedRoute adminOnly>  // requires role === "Admin"
```

Unauthenticated users are redirected to `/login`. Non-admin users accessing `/admin` are redirected to `/`. Role checks use the value stored in context — no additional API call is made.

---

## API Communication

All HTTP calls go through a shared `fetchJson` wrapper in `services/apiHelpers.ts` rather than calling `fetch` directly. It handles three failure cases that would otherwise surface raw browser errors to users:

| Failure | Raw browser error | Message shown to user |
|---|---|---|
| Server unreachable / offline | `TypeError: Failed to fetch` | "Unable to connect. Please check your internet connection." |
| Non-JSON response (502/504) | `SyntaxError: Unexpected token '<'` | Status-mapped message |
| 401 with no body | Silent fallback string | "Your session has expired. Please log in again." |

Each service file calls `fetchJson` and passes through the server's `message` field when the API returns one, falling back to `friendlyStatusError(status)` when it does not.

The JWT token is attached via `getAuthHeaders()` in `authHeaders.ts`, which appends `Authorization: Bearer` only when a token is present in `localStorage`.

---

## State Management

Local `useState` / `useEffect` — no global state library. Each page owns its loading, error, and data state.

The only shared state is auth (token + role), managed through `AuthContext`.

Popup/toast notifications are lifted to `App.tsx` and passed down as setter props (`setPopupMessage`, `setPopupType`, `setPopupConfirm`). Pages trigger them directly without needing to own the Popup component. The `popupConfirm` prop supports a confirm/cancel flow used for cancellations and deletions.

---

## UI and Design

Custom CSS built on design tokens (CSS custom properties) — no CSS framework:

```css
--clr-primary:       #1565a8
--clr-accent:        #0ea5a4
--clr-bg:            #f0f4f8
--clr-surface:       #ffffff
--clr-danger:        #dc2626
```

Spacing, border-radius, and shadow values are defined as tokens in `index.css` and referenced consistently across all components.

**Reusable UI components:**

| Component | Purpose |
|---|---|
| `Spinner` | Full-page and inline loading states; `inline` prop for in-form use; `aria-label` for accessibility |
| `EmptyState` | Consistent zero-data views with icon, title, and optional message |
| `Popup` | Toast with success/error variants and optional confirm action (used for cancellations) |
| `Button` | Primary and danger-outline variants |

Every data-fetching view has both a loading state (`Spinner`) and an empty state (`EmptyState`) — doctor search, appointment list, admin appointment view, and slot selection.

---

## Pages

**`HomePage` — Booking form**
Cascading form: clinic → doctor → date → duration → available slot. Slots are fetched from the API whenever date or duration changes. Shows guest fields (name, email, date of birth) when the user is not logged in; hides them for authenticated patients.

**`MyAppointmentsPage` — Patient appointments**
Lists appointments for the logged-in patient. Each card has an inline reschedule form that expands in place — new slots are fetched on date or duration change. Cancel shows a confirm popup before the delete request is sent.

**`SearchDoctorPage` — Doctor search**
Input is debounced (400 ms) before triggering the search API call. Results render as doctor cards showing clinic and speciality badges. An empty state with a contextual message is shown when the query returns no results.

**`AdminPage` — Admin dashboard**
Tabbed layout: Appointments (filterable by clinic or doctor), Clinics, Doctors, Categories, Specialities. Each tab has inline add forms and per-item edit/delete with confirmation popups.

---

## Routes

| Path | Access | Page |
|---|---|---|
| `/` | Public | Booking form |
| `/search` | Public | Doctor search |
| `/login` | Public | Login |
| `/register` | Public | Register |
| `/appointments` | Patient | Appointment management |
| `/admin` | Admin | Admin dashboard |

---

## Running Locally

**Prerequisites:** Node.js 18+, backend API running at `http://localhost:5108`

```bash
cd Frontend
npm install
```

Create a `.env.local` file:

```
VITE_API_BASE_URL=http://localhost:5108
```

```bash
npm run dev
```

Runs at `http://localhost:5173`

In production, `VITE_API_BASE_URL` is set to `https://api.ranjitnair.dev` via Azure Static Web Apps environment configuration.
