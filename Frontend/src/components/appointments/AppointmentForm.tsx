import { useEffect, useMemo, useState } from "react";

import Button from "../../components/Button";

import { bookAppointment, getAvailableSlots } from "../../services/appointmentService";
import { getClinics } from "../../services/clinicService";
import { getCategories } from "../../services/categoryService";
import { getDoctors } from "../../services/doctorService";
import { useAuth } from "../../context/useAuth";

import type { ClinicDTO } from "../../types/ClinicDTO";
import type { CategoryDTO } from "../../types/CategoryDTO";
import type { DoctorDTO } from "../../types/DoctorDTO";

interface Props {
  setPopupMessage: React.Dispatch<React.SetStateAction<string>>;
  setPopupType: React.Dispatch<React.SetStateAction<"success" | "error">>;
}

export default function AppointmentForm({
  setPopupMessage,
  setPopupType,
}: Props) {
  const { token } = useAuth();

  const [durationMinutes, setDurationMinutes] = useState(0);

  const [clinicId, setClinicId] = useState(0);
  const [doctorId, setDoctorId] = useState(0);
  const [categoryId, setCategoryId] = useState(0);
  const [dateOfBirth, setDateOfBirth] = useState("");

  const [clinics, setClinics] = useState<ClinicDTO[]>([]);
  const [categories, setCategories] = useState<CategoryDTO[]>([]);
  const [allDoctors, setAllDoctors] = useState<DoctorDTO[]>([]);

  const [firstName, setFirstName] = useState("");
  const [lastName, setLastName] = useState("");
  const [email, setEmail] = useState("");

  const [selectedDate, setSelectedDate] = useState("");
  const [availableSlots, setAvailableSlots] = useState<string[]>([]);
  const [selectedSlot, setSelectedSlot] = useState("");
  const [loadingSlots, setLoadingSlots] = useState(false);

  const [booking, setBooking] = useState(false);


  // Load dropdown data 
  useEffect(() => {
    const loadData = async () => {
      try {
        const [c, cat, d] = await Promise.all([
          getClinics(),
          getCategories(),
          getDoctors(),
        ]);

        setClinics(c);
        setCategories(cat);
        setAllDoctors(d);
      } catch {
        setPopupMessage("Failed to load form data.");
        setPopupType("error");
      }
    };

    void loadData();
  }, [setPopupMessage, setPopupType]);

  // Filter doctors per clinic
  const filteredDoctors = useMemo(() => {
    if (!clinicId) return allDoctors;
    return allDoctors.filter((d) => d.clinicId === clinicId);
  }, [clinicId, allDoctors]);

  // Fetch slots
  useEffect(() => {
    if (!doctorId || !selectedDate) return;

    const fetchSlots = async () => {
      const today = new Date();
      const selected = new Date(selectedDate);

      // Prevent past date
      if (selected < new Date(today.toDateString())) {
        setAvailableSlots([]);
        setSelectedSlot("");
        setPopupMessage("Cannot select past dates.");
        setPopupType("error");
        return;
      }

      try {
        setLoadingSlots(true);

        const slots = await getAvailableSlots(
          doctorId,
          selectedDate,
          durationMinutes
        );

        setAvailableSlots(slots);

        if (slots.length === 0) {
          setPopupMessage("No available time slots for the selected date. Please choose a weekday.");
          setPopupType("error");
        }

        setLoadingSlots(false);
      } catch {
        setLoadingSlots(false);
        setAvailableSlots([]);
        setPopupMessage("Failed to load time slots.");
        setPopupType("error");
      }
    };

    void fetchSlots();
  }, [doctorId, selectedDate, durationMinutes, setPopupMessage, setPopupType]);


  // Submit appointment
 async function handleSubmit(e: React.SyntheticEvent<HTMLFormElement>) {
    e.preventDefault();
    // Basic frontend validation
    if (!clinicId) {
    setPopupMessage("Please select a clinic.");
    setPopupType("error");
    return;
  }

  if (!doctorId) {
    setPopupMessage("Please select a doctor.");
    setPopupType("error");
    return;
  }

  if (!categoryId) {
    setPopupMessage("Please select a category.");
    setPopupType("error");
    return;
  }

  if (!selectedDate) {
    setPopupMessage("Please select a date.");
    setPopupType("error");
    return;
  }

    if (!selectedSlot) {
      setPopupMessage("Please select a time slot.");
      setPopupType("error");
      return;
    }
    try {
      setBooking(true);

      await bookAppointment({
        appointmentDate: selectedSlot,
        durationMinutes,
        clinicId,
        doctorId,
        categoryId,
        ...(token
          ? {}
          : {
              firstName,
              lastName,
              email,
              dateOfBirth,
            }),
      });

      setPopupMessage("Appointment booked successfully");
      setPopupType("success");

      // Reset form
      setSelectedDate("");
      setSelectedSlot("");
      setAvailableSlots([]);
      setDurationMinutes(30);
      setClinicId(0);
      setDoctorId(0);
      setCategoryId(0);
      setFirstName("");
      setLastName("");
      setEmail("");
      setDateOfBirth("");
    } catch (err: unknown) {
      if (err instanceof Error) {
        setPopupMessage(err.message);
      } else {
        setPopupMessage("Booking failed");
      }
      setPopupType("error");
    } finally {
      setBooking(false);
    }
  }

  return (
    <div className="card">
      <h2>Book Appointment</h2>

      <form
        onSubmit={(e) => { void handleSubmit(e); }}>
        {!token && (
          <>
            <input
              placeholder="First Name"
              value={firstName}
              onChange={(e) => { setFirstName(e.target.value); }}
              required
            />

            <input
              placeholder="Last Name"
              value={lastName}
              onChange={(e) => { setLastName(e.target.value); }}
              required
            />

            <input
              type="email"
              placeholder="Email"
              value={email}
              onChange={(e) => { setEmail(e.target.value); }}
              required
            />

            <div>
              <small>Date of Birth</small>
              <input
                type="date"
                value={dateOfBirth}
                onChange={(e) => { setDateOfBirth(e.target.value);}}
                required
              />
            </div>
          </>
        )}

        <div>
          <small>Appointment Duration (minutes)</small>
          <select
            value={durationMinutes}
            onChange={(e) => { setDurationMinutes(Number(e.target.value));}}
            required
          >
            <option value={0}>Select Duration</option>  
            <option value={15}>15 minutes</option>
            <option value={30}>30 minutes</option>
            <option value={45}>45 minutes</option>
            <option value={60}>60 minutes</option>
          </select>
        </div>

        <select
          value={clinicId}
          onChange={(e) => {
            const id = Number(e.target.value);
            setClinicId(id);
            setDoctorId(0);
          }}
          required
        >
          <option value={0}>Select Clinic</option>
          {clinics.map((c) => (
            <option key={c.id} value={c.id}>
              {c.name}
            </option>
          ))}
        </select>

        <select
          value={doctorId}
          onChange={(e) => { setDoctorId(Number(e.target.value)); }}
          required
        >
          <option value={0}>
            {clinicId ? "Select Doctor (from this clinic)" : "Select Doctor"}
          </option>
          {filteredDoctors.map((d) => (
            <option key={d.id} value={d.id}>
              {d.firstName} {d.lastName}
            </option>
          ))}
        </select>

        <div>
          <small>Select Appointment Date</small>
          <input
            type="date"
            value={selectedDate}
            onChange={(e) => {
              setSelectedDate(e.target.value);
              setSelectedSlot("");
            }}
            required
          />
        </div>

        <div>
          <small>Available Time Slots</small>
          {loadingSlots && <p>Loading slots...</p>}

          <select
            value={selectedSlot}
            onChange={(e) => { setSelectedSlot(e.target.value); }}
            required
          >
            <option value="">Select Time</option>
            {availableSlots.map((slot) => (
              <option key={slot} value={slot}>
                {new Date(slot).toLocaleTimeString([], {
                  hour: "2-digit",
                  minute: "2-digit",
                })}
              </option>
            ))}
          </select>
        </div>

        <select
          value={categoryId}
          onChange={(e) => { setCategoryId(Number(e.target.value)); }}
          required
        >
          <option value={0}>Select Category</option>
          {categories.map((c) => (
            <option key={c.id} value={c.id}>
              {c.name}
            </option>
          ))}
        </select>

        <Button type="submit" disabled={booking}>
          {booking ? "Booking..." : "Book Appointment"}
        </Button>

      </form>
    </div>
  );
}
