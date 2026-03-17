import Button from "../Button";
import type { AppointmentDTO } from "../../types/AppointmentDTO";

interface Props {
  appointment: AppointmentDTO;
  onCancel: (id: number) => void;
  onEdit: (appointment: AppointmentDTO) => void;
};

export default function AppointmentCard({
  appointment,
  onCancel,
  onEdit,
}: Props) {
  const date = new Date(appointment.appointmentDate);

  const month   = date.toLocaleString("default", { month: "short" }).toUpperCase();
  const day     = date.getDate();
  const time    = date.toLocaleTimeString([], { hour: "2-digit", minute: "2-digit" });
  const weekday = date.toLocaleString("default", { weekday: "long" });

  return (
    <div className="card appointment-card">
      <div className="appointment-header">
        <div className="appt-date-block">
          <span className="appt-month">{month}</span>
          <span className="appt-day">{day}</span>
        </div>

        <div className="appt-datetime">
          <span className="appt-time">{time}</span>
          <span className="appt-weekday">{weekday}</span>
        </div>

        <span className="appt-duration-badge">{appointment.durationMinutes} min</span>
      </div>

      <div className="appointment-body">
        <div className="appt-detail">
          <span className="appt-detail-label">Doctor</span>
          <span className="appt-detail-value">{appointment.doctorName}</span>
        </div>

        <div className="appt-detail">
          <span className="appt-detail-label">Clinic</span>
          <span className="appt-detail-value">{appointment.clinicName}</span>
        </div>

        <div className="appt-detail">
          <span className="appt-detail-label">Category</span>
          <span className="appt-detail-value">{appointment.categoryName}</span>
        </div>
      </div>

      <div className="card-actions">
        <Button className="cancel-btn" onClick={() => { onCancel(appointment.id); }}>
          Cancel
        </Button>
        <Button onClick={() => { onEdit(appointment); }}>
          Reschedule
        </Button>
      </div>
    </div>
  );
}
