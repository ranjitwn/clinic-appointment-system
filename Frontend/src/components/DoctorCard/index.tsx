import type { DoctorSearchDTO } from "../../types/DoctorSearchDTO";

interface Props {
  doctor: DoctorSearchDTO;
}

export default function DoctorCard({ doctor }: Props) {
  const initials = doctor.fullName
    .split(" ")
    .map((n) => n[0])
    .slice(0, 2)
    .join("")
    .toUpperCase();

  return (
    <div className="card doctor-card">
      <div className="doctor-avatar">{initials}</div>

      <p className="doctor-name">{doctor.fullName}</p>

      <div className="doctor-meta">
        <span className="meta-label">Clinic</span>
        {doctor.clinic}
      </div>

      <div className="doctor-meta">
        <span className="meta-label">Speciality</span>
        {doctor.speciality}
      </div>
    </div>
  );
}
