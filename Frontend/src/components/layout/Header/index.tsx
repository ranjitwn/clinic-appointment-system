import { Link, useNavigate } from "react-router-dom";
import Button from "../../Button";
import { useAuth } from "../../../context/useAuth";

export default function Header() {
  const { token, role, logout } = useAuth();
  const navigate = useNavigate();

  const isLoggedIn = !!token;

  function handleLogout() {
    logout();
    navigate("/");
  }

  return (
    <header>
      <h2>Clinic Booking System</h2>

      <nav>
        <Link to="/">Home</Link>
        <Link to="/search">Search Doctors</Link>

        {!isLoggedIn && (
          <>
            <Link to="/login">Login</Link>
            <Link to="/register">Register</Link>
          </>
        )}

        {isLoggedIn && (
          <>
            <Link to="/appointments">My Appointments</Link>

            {role === "Admin" && (
              <Link to="/admin">Admin</Link>
            )}

            <Button className="nav-link" onClick={handleLogout}>
              Logout
            </Button>
          </>
        )}
      </nav>
    </header>
  );
}
