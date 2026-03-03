import AuthCard from "../../components/auth/AuthCard";
import RegisterForm from "../../components/auth/RegisterForm";

interface Props {
  setPopupMessage: React.Dispatch<React.SetStateAction<string>>;
  setPopupType: React.Dispatch<React.SetStateAction<"success" | "error">>;
};

export default function RegisterPage(props: Props) {
  return (
    <AuthCard title="Register">
      <RegisterForm
        setPopupMessage={props.setPopupMessage}
        setPopupType={props.setPopupType}
      />
    </AuthCard>
  );
}
