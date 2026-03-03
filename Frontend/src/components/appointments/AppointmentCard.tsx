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
  return (
    <div className="card">
      <p>
        <strong>Date:</strong>{" "}
        {new Date(appointment.appointmentDate).toLocaleString()}
      </p>

      <p>
        <strong>Duration:</strong> {appointment.durationMinutes} minutes
      </p>

      <p>
        <strong>Clinic:</strong> {appointment.clinicName}
      </p>

      <p>
        <strong>Doctor:</strong> {appointment.doctorName}
      </p>

      <p>
        <strong>Category:</strong> {appointment.categoryName}
      </p>

      <div className="card-actions">
        <Button className="cancel-btn" onClick={() => {onCancel(appointment.id);}}>
          Cancel Appointment
        </Button>

        <Button onClick={() => {onEdit(appointment);}}>
          Edit Appointment
        </Button>
      </div>
    </div>
  );
}
