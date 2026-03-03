import AuthCard from "../../components/auth/AuthCard";
import LoginForm from "../../components/auth/LoginForm";

interface Props {
  setPopupMessage: React.Dispatch<React.SetStateAction<string>>;
  setPopupType: React.Dispatch<React.SetStateAction<"success" | "error">>;
};

export default function LoginPage(props: Props) {
  return (
    <AuthCard title="Login">
      <LoginForm
        setPopupMessage={props.setPopupMessage}
        setPopupType={props.setPopupType}
      />
    </AuthCard>
  );
}
