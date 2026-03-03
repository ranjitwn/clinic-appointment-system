import AppointmentForm from "../../components/appointments/AppointmentForm";

interface Props {
  setPopupMessage: React.Dispatch<React.SetStateAction<string>>;
  setPopupType: React.Dispatch<React.SetStateAction<"success" | "error">>;
};

export default function HomePage({
  setPopupMessage,
  setPopupType,
}: Props) {
  return (
    <AppointmentForm
      setPopupMessage={setPopupMessage}
      setPopupType={setPopupType}
    />
  );
}

