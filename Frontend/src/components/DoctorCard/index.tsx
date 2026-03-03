import type { DoctorSearchDTO } from "../../types/DoctorSearchDTO";

interface Props {
  doctor: DoctorSearchDTO;
}

export default function DoctorCard({ doctor }: Props) {
  return (
    <div className="card">
      <h3>Doctor: {doctor.fullName}</h3>
      <p>Clinic: {doctor.clinic}</p>
      <p>Speciality: {doctor.speciality}</p>
    </div>
  );
}
