import { useEffect, useState } from "react";
import { getMyAppointments, deleteAppointment, updateAppointment, getAvailableSlots,} from "../../services/appointmentService";

import AppointmentCard from "../../components/appointments/AppointmentCard";
import Button from "../../components/Button";
import type { AppointmentDTO } from "../../types/AppointmentDTO";

interface Props {
  setPopupMessage: React.Dispatch<React.SetStateAction<string>>;
  setPopupType: React.Dispatch<React.SetStateAction<"success" | "error">>;
  setPopupConfirm: React.Dispatch<React.SetStateAction<(() => void) | null>>;
};

export default function MyAppointmentsPage({
  setPopupMessage,
  setPopupType,
  setPopupConfirm,
}: Props) {
  const [appointments, setAppointments] = useState<AppointmentDTO[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const [editingId, setEditingId] = useState<number | null>(null);
  const [newDate, setNewDate] = useState("");
  const [newDuration, setNewDuration] = useState(30);

  const [availableSlots, setAvailableSlots] = useState<string[]>([]);
  const [selectedSlot, setSelectedSlot] = useState("");
  const [updating, setUpdating] = useState(false);
  const [loadingSlots, setLoadingSlots] = useState(false);


  // Load appointments
  useEffect(() => {
    const fetchAppointments = async () => {
      try {
        const data = await getMyAppointments();
        setAppointments(data);
      } catch (err: unknown) {
        setError(err instanceof Error ? err.message : "Failed to load appointments");
      } finally {
        setLoading(false);
      }
    };

    void fetchAppointments();
  }, []);

  // Fetch slots when editing
   useEffect(() => {
    if (!editingId || !newDate) return;

    const doctorId = appointments.find((a) => a.id === editingId)?.doctorId;
    if (!doctorId) return;

    const fetchSlots = async () => {
      try {
        setLoadingSlots(true);
        const slots = await getAvailableSlots(
          doctorId,
          newDate,
          newDuration
        );

        setAvailableSlots(slots);
      } catch (err: unknown) {
        setPopupMessage(
          err instanceof Error ? err.message : "Failed to load slots"
        );
        setPopupType("error");
        setAvailableSlots([]);
      } finally {
        setLoadingSlots(false);
      }
    };

    void fetchSlots();
  }, [editingId, newDate, newDuration, appointments, setPopupMessage, setPopupType]);


  // Cancel appointment (popup confirm)
  function handleCancel(id: number) {
    setPopupMessage("Cancel this appointment?");
    setPopupType("error");

    setPopupConfirm(() => async () => {
      try {
        await deleteAppointment(id);
        setAppointments(prev => prev.filter(a => a.id !== id));

        setPopupConfirm(null);
        setPopupMessage("Appointment cancelled");
        setPopupType("success");
      } catch (err: unknown) {
        setPopupMessage(err instanceof Error ? err.message : "Cancel failed");
        setPopupType("error");
      }
    });
  }

  function startEdit(a: AppointmentDTO) {
    setEditingId(a.id);

    const fullDate = new Date(a.appointmentDate);

    setNewDate(fullDate.toISOString().split("T")[0]);
    setNewDuration(a.durationMinutes);
    setSelectedSlot(a.appointmentDate);
  }

  async function handleUpdate(id: number) {    
    if (!selectedSlot) {
    setPopupMessage("Please select a time slot.");
    setPopupType("error");
    return;
  }

    try {
      setUpdating(true);

      await updateAppointment(id, {
        appointmentDate: selectedSlot || newDate,
        durationMinutes: newDuration,
      });

      setAppointments(prev =>
        prev.map(a =>
          a.id === id
            ? {
                ...a,
                appointmentDate: selectedSlot || newDate,
                durationMinutes: newDuration,
              }
            : a
        )
      );

      setEditingId(null);

      setPopupMessage("Appointment updated");
      setPopupType("success");
    } catch (err: unknown) {
      setPopupMessage(err instanceof Error ? err.message : "Update failed");
      setPopupType("error");
    } finally {
      setUpdating(false);
    }
  } 

  if (loading) return <p>Loading appointments...</p>;
  if (error) return <p>{error}</p>;

  return (
    <main>
      <div className="page-container">
        <h1>My Appointments</h1>

        {appointments.length === 0 && <p className="no-appointments"> No appointments found.</p>}

        {appointments.map((a) => (
          <div key={a.id}>
            <AppointmentCard
              appointment={a}
              onCancel={handleCancel}
              onEdit={startEdit}
            />

            {editingId === a.id && (
              <div className="card">
                {loadingSlots && <p>Loading available slots...</p>}
                <input
                  type="date"
                  value={newDate}
                  onChange={(e) => {
                    setNewDate(e.target.value);
                    setSelectedSlot("");
                  }}
                />

                <select
                  value={selectedSlot}
                  onChange={(e) => {setSelectedSlot(e.target.value);
                  }}
                >
                  <option value="">
                    Current appointment time: {new Date(a.appointmentDate).toLocaleTimeString([], {
                      hour: "2-digit",
                      minute: "2-digit",
                    })} — select new time
                  </option>
                  {availableSlots.map((slot) => (
                    <option key={slot} value={slot}>
                      {new Date(slot).toLocaleTimeString([], {
                        hour: "2-digit",
                        minute: "2-digit",
                      })}
                    </option>
                  ))}
                </select>

                <select
                  value={newDuration}
                  onChange={(e) => {
                    setNewDuration(Number(e.target.value));
                  }}
                >
                  <option value={15}>15 minutes</option>
                  <option value={30}>30 minutes</option>
                  <option value={45}>45 minutes</option>
                  <option value={60}>60 minutes</option>
                </select>

                <div className="card-actions">
                   <Button onClick={() => { void handleUpdate(a.id); }}
                    disabled={updating}
                    >
                    {updating ? "Saving..." : "Save"}
                    </Button>
                </div>
              </div>
            )}
          </div>
        ))}
      </div>
    </main>
  );
}
