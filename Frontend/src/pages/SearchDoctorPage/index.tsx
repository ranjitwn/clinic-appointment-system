import { useState, useEffect } from "react";
import { searchDoctors } from "../../services/doctorService";
import type { DoctorSearchDTO } from "../../types/DoctorSearchDTO";
import DoctorCard from "../../components/DoctorCard";

export default function SearchDoctorPage() {
  const [query, setQuery] = useState("");
  const [doctors, setDoctors] = useState<DoctorSearchDTO[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");

  useEffect(() => {
  if (!query.trim()) {
    setDoctors([]);
    return;
  }

  const timeout = setTimeout(async () => {
    setLoading(true);
    setError("");

    try {
      const result = await searchDoctors(query);
      setDoctors(result);
    } catch (err: unknown) {
      setError(
        err instanceof Error
          ? err.message
          : "Failed to fetch doctors"
      );
    } finally {
      setLoading(false);
    }
  }, 400);

  return () => {
    clearTimeout(timeout);
  };

}, [query]);

  return (
    <main>
      <div className="page-container">
        <div className="card">
          <h1>Search Doctors</h1>

          <div className="search-form">
            <input
              placeholder="Search doctors by name..."
              value={query}
              onChange={(e) => {
                setQuery(e.target.value);
              }}
            />
          </div>

          {loading && <p>Searching...</p>}
          {error && <p>{error}</p>}

          {!loading && query && doctors.length === 0 && !error && (
            <p className="no-appointments">
              No doctors found for your search.
            </p>
          )}

          <div className="search-results">
            {doctors.map((doc) => (
              <DoctorCard key={doc.doctorId} doctor={doc} />
            ))}
          </div>
        </div>
      </div>
    </main>
  );
}