import { useState } from "react";
import { searchDoctors } from "../../services/doctorService";
import type { DoctorSearchDTO } from "../../types/DoctorSearchDTO";
import DoctorCard from "../../components/DoctorCard";

export default function SearchDoctorPage() {
  const [query, setQuery] = useState("");
  const [doctors, setDoctors] = useState<DoctorSearchDTO[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");
  const [hasSearched, setHasSearched] = useState(false);

  async function handleSearch(e: React.SyntheticEvent<HTMLFormElement>) {
    e.preventDefault();

    setHasSearched(true);
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
  }

  return (
    <main>
      <div className="page-container">
        <div className="card">
          <h1>Search Doctors</h1>

          <form
            onSubmit={(e) => {
              void handleSearch(e);
            }}
            className="search-form"
          >
            <input
              placeholder="Search by doctor first or last name"
              value={query}
              onChange={(e) => {
                setQuery(e.target.value);
              }}
              required
            />
            <button type="submit">Search</button>
          </form>

          {loading && <p>Searching...</p>}
          {error && <p>{error}</p>}

          {hasSearched && !loading && doctors.length === 0 && !error && (
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