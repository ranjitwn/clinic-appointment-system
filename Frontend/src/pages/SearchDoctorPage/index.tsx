import { useState, useEffect } from "react";
import { searchDoctors } from "../../services/doctorService";
import type { DoctorSearchDTO } from "../../types/DoctorSearchDTO";
import DoctorCard from "../../components/DoctorCard";
import EmptyState from "../../components/ui/EmptyState";
import Spinner from "../../components/ui/Spinner";

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

          {loading && <Spinner inline text="Searching…" />}
          {error && <p>{error}</p>}

          {!loading && query && doctors.length === 0 && !error && (
            <EmptyState
              icon={
                <svg width="32" height="32" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="1.5" strokeLinecap="round" strokeLinejoin="round">
                  <circle cx="11" cy="11" r="8"/>
                  <line x1="21" y1="21" x2="16.65" y2="16.65"/>
                  <line x1="8" y1="11" x2="14" y2="11"/>
                </svg>
              }
              title="No doctors found"
              message={`No results for "${query}". Try a different name.`}
            />
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