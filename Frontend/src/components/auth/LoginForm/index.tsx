import { useState } from "react";
import { loginUser } from "../../../services/authService";
import Button from "../../Button";
import { useNavigate } from "react-router-dom";
import { useAuth } from "../../../context/useAuth";

interface Props {
  setPopupMessage: React.Dispatch<React.SetStateAction<string>>;
  setPopupType: React.Dispatch<React.SetStateAction<"success" | "error">>;
};

export default function LoginForm({
  setPopupMessage,
  setPopupType,
}: Props) {
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [loggingIn, setLoggingIn] = useState(false);

  const navigate = useNavigate();
  const { login } = useAuth();

  async function handleSubmit(e: React.SyntheticEvent<HTMLFormElement>) {
    e.preventDefault();
    if (loggingIn) return;

    try {
      setLoggingIn(true);

      const result = await loginUser({ email, password }) as {
        token: string;
        role: string;
      };

      login(result.token, result.role);

      setPopupMessage("Login successful");
      setPopupType("success");

      // Redirect based on role
      if (result.role === "Admin") {
        navigate("/admin");
      } else {
        navigate("/");
      }

    } catch (error) {
      const message =
      error instanceof Error ? error.message : "Login failed";

      setPopupMessage(message);
      setPopupType("error");
    } finally {
      setLoggingIn(false);
    }  
  }

  return (
    <form onSubmit={(e) => { void handleSubmit(e); }}>
      <input
        type="email"
        placeholder="Email"
        value={email}
        onChange={(e) => { setEmail(e.target.value); }}
        required
      />

      <input
        type="password"
        placeholder="Password"
        value={password}
        onChange={(e) => { setPassword(e.target.value); }}
        required
      />

      <Button type="submit" disabled={loggingIn}>
        {loggingIn ? "Logging in..." : "Login"}
      </Button>
    </form>
  );
}
