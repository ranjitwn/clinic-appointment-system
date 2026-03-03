import { Routes, Route } from "react-router-dom";
import { useState } from "react";

import Layout from "./components/layout/Layout";
import Popup from "./components/ui/Popup";

import HomePage from "./pages/HomePage";
import SearchDoctorPage from "./pages/SearchDoctorPage";
import LoginPage from "./pages/LoginPage";
import RegisterPage from "./pages/RegisterPage";
import MyAppointmentsPage from "./pages/MyAppointmentsPage";
import AdminPage from "./pages/AdminPage";

import ProtectedRoute from "./routes/ProtectedRoute";

function App() {
  const [popupMessage, setPopupMessage] = useState("");
  const [popupType, setPopupType] = useState<"success" | "error">("success");

  // store optional confirm action
  const [popupConfirm, setPopupConfirm] = useState<(() => void) | null>(null);

  function closePopup() {
    setPopupMessage("");
    setPopupConfirm(null);
  }

  return (
    <>
      <Popup
        message={popupMessage}
        type={popupType}
        onClose={closePopup}
        onConfirm={popupConfirm ?? undefined}
        confirmText="Yes"
        cancelText="No"
      />

      <Layout>
        <Routes>
          <Route
            path="/"
            element={
              <HomePage
                setPopupMessage={setPopupMessage}
                setPopupType={setPopupType}
                />
            }
          />

          <Route
            path="/book"
            element={
              <HomePage
                setPopupMessage={setPopupMessage}
                setPopupType={setPopupType}
              />
            }
          />

          <Route path="/search" element={<SearchDoctorPage />} />

          <Route
            path="/login"
            element={
              <LoginPage
                setPopupMessage={setPopupMessage}
                setPopupType={setPopupType}
              />
            }
          />

          <Route
            path="/register"
            element={
              <RegisterPage
                setPopupMessage={setPopupMessage}
                setPopupType={setPopupType}
              />
            }
          />

          <Route
            path="/appointments"
            element={
              <ProtectedRoute>
                <MyAppointmentsPage
                  setPopupMessage={setPopupMessage}
                  setPopupType={setPopupType}
                  setPopupConfirm={setPopupConfirm}
                />
              </ProtectedRoute>
            }
          />

          <Route
            path="/admin"
            element={
              <ProtectedRoute adminOnly>
                <AdminPage
                  setPopupMessage={setPopupMessage}
                  setPopupType={setPopupType}
                  setPopupConfirm={setPopupConfirm}
                />
              </ProtectedRoute>
            }
          />
        </Routes>
      </Layout>
    </>
  );
}

export default App;
