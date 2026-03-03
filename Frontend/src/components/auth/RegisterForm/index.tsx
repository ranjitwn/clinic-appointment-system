import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { registerUser } from "../../../services/authService";
import Button from "../../Button";

// Popup props
interface Props {
  setPopupMessage: React.Dispatch<React.SetStateAction<string>>;
  setPopupType: React.Dispatch<React.SetStateAction<"success" | "error">>;
};

export default function RegisterForm({
  setPopupMessage,
  setPopupType,
}: Props) {
  const [firstName, setFirstName] = useState("");
  const [lastName, setLastName] = useState("");
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [dateOfBirth, setDateOfBirth] = useState("");
  const [gender, setGender] = useState("");
  const [registering, setRegistering] = useState(false);

  const navigate = useNavigate();

  async function handleSubmit(e: React.SyntheticEvent<HTMLFormElement>) {
    e.preventDefault();
    if (registering) return;

    try {
      setRegistering(true);

      await registerUser({
        firstName,
        lastName,
        email,
        password,
        dateOfBirth,
        gender: gender || undefined,
      });

      setPopupMessage("Registration successful");
      setPopupType("success");

      navigate("/login");
    } catch (error) {
      const message =
        error instanceof Error
          ? error.message
          : "Registration failed";

      setPopupMessage(message);
      setPopupType("error");
    } finally {
      setRegistering(false);
    }
  }

  return (
    <form onSubmit={(e) => { void handleSubmit(e); }}>
      <input
        placeholder="First Name"
        value={firstName}
        onChange={(e) => {setFirstName(e.target.value); }}
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

      <input
        type="password"
        placeholder="Password"
        value={password}
        onChange={(e) => { setPassword(e.target.value); }}
        required
      />

      <div>
        <small>Date of Birth</small>
        <input
          type="date"
          value={dateOfBirth}
          onChange={(e) => { setDateOfBirth(e.target.value); }}
          required
        />
      </div>

      <select
        value={gender}
        onChange={(e) => { setGender(e.target.value); }}
      >
        <option value="">Select Gender (Optional)</option>
        <option value="Male">Male</option>
        <option value="Female">Female</option>
        <option value="Other">Other</option>
      </select>

      <Button type="submit" disabled={registering}>
        {registering ? "Registering..." : "Register"}
      </Button>
    </form>
  );
}
