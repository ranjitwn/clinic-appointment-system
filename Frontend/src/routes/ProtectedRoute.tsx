import { Navigate } from "react-router-dom";
import { useAuth } from "../context/useAuth";

interface Props {
  children: React.ReactNode;
  adminOnly?: boolean;
}

export default function ProtectedRoute({ children, adminOnly }: Props) {
  const { token, role } = useAuth();

  // Not logged in
  if (!token) {
    return <Navigate to="/login" replace />;
  }

  // Admin-only route check
  if (adminOnly && role !== "Admin") {
    return <Navigate to="/" replace />;
  }

  return children;
}
