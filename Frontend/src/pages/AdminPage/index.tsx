import { useEffect, useMemo, useState } from "react";

import Button from "../../components/Button";
import EmptyState from "../../components/ui/EmptyState";
import Spinner from "../../components/ui/Spinner";

import {getAppointmentsByClinic, getAppointmentsByDoctor,} from "../../services/appointmentService";
import {getClinics, createClinic, updateClinic, deleteClinic,} from "../../services/clinicService";
import {getCategories, createCategory, updateCategory, deleteCategory,} from "../../services/categoryService";
import {getSpecialities, createSpeciality, updateSpeciality, deleteSpeciality,} from "../../services/specialityService";
import {getDoctors, createDoctor, updateDoctor, deleteDoctor,} from "../../services/doctorService";

import type { AppointmentDTO } from "../../types/AppointmentDTO";
import type { ClinicDTO } from "../../types/ClinicDTO";
import type { CategoryDTO } from "../../types/CategoryDTO";
import type { SpecialityDTO } from "../../types/SpecialityDTO";
import type { DoctorDTO } from "../../types/DoctorDTO";
import type { DoctorCreateDTO } from "../../types/DoctorCreateDTO";

interface Props {
  setPopupMessage: React.Dispatch<React.SetStateAction<string>>;
  setPopupType: React.Dispatch<React.SetStateAction<"success" | "error">>;
  setPopupConfirm: React.Dispatch<React.SetStateAction<(() => void) | null>>;
};

type Tab = "appointments" | "clinics" | "categories" | "specialities" | "doctors";
type AppointmentFilter = "all" | "clinic" | "doctor";

export default function AdminPage({
  setPopupMessage,
  setPopupType,
  setPopupConfirm,
  }: Props) {
  const [activeTab, setActiveTab] = useState<Tab>("appointments");

  // Shared lookup data
  const [clinics, setClinics] = useState<ClinicDTO[]>([]);
  const [categories, setCategories] = useState<CategoryDTO[]>([]);
  const [specialities, setSpecialities] = useState<SpecialityDTO[]>([]);
  const [doctors, setDoctors] = useState<DoctorDTO[]>([]);

  const [loadingLookups, setLoadingLookups] = useState(true);

  // Appointments overview 
  const [appointmentFilter, setAppointmentFilter] = useState<AppointmentFilter>("all");
  const [selectedClinicId, setSelectedClinicId] = useState(0);
  const [selectedDoctorId, setSelectedDoctorId] = useState(0);

  const [appointments, setAppointments] = useState<AppointmentDTO[]>([]);
  const [loadingAppointments, setLoadingAppointments] = useState(false);
  const [saving, setSaving] = useState(false);

  // Clinics 
  const [newClinicName, setNewClinicName] = useState("");
  const [editingClinicId, setEditingClinicId] = useState<number | null>(null);
  const [editingClinicName, setEditingClinicName] = useState("");

  // Categories 
  const [newCategoryName, setNewCategoryName] = useState("");
  const [editingCategoryId, setEditingCategoryId] = useState<number | null>(null);
  const [editingCategoryName, setEditingCategoryName] = useState("");

  // Specialities 
  const [newSpecialityName, setNewSpecialityName] = useState("");
  const [editingSpecialityId, setEditingSpecialityId] = useState<number | null>(null);
  const [editingSpecialityName, setEditingSpecialityName] = useState("");

  //  Doctors 
  const [doctorForm, setDoctorForm] = useState<DoctorCreateDTO>({
    firstName: "",
    lastName: "",
    clinicId: 0,
    specialityId: 0,
  });

  const [editingDoctorId, setEditingDoctorId] = useState<number | null>(null);
  const [editingDoctorForm, setEditingDoctorForm] = useState<DoctorCreateDTO>({
    firstName: "",
    lastName: "",
    clinicId: 0,
    specialityId: 0,
  });

  // Load lookup data for Admin page
  useEffect(() => {
    const loadLookups = async () => {
      try {
        const [c, cat, spec, d] = await Promise.all([
          getClinics(),
          getCategories(),
          getSpecialities(),
          getDoctors(),
        ]);

        setClinics(c);
        setCategories(cat);
        setSpecialities(spec);
        setDoctors(d);
      } catch (err: unknown) {
        setPopupMessage(err instanceof Error ? err.message : "Failed to load admin data.");
        setPopupType("error");
      } finally {
        setLoadingLookups(false);
      }
    };

    void loadLookups();
    }, [setPopupMessage, setPopupType]);

    const doctorsForSelectedClinic = useMemo(() => {
        if (!selectedClinicId) return doctors;
        return doctors.filter((d) => d.clinicId === selectedClinicId);
    }, [doctors, selectedClinicId]);

  async function refreshLookups() {
    try {
      const [c, cat, spec, d] = await Promise.all([
        getClinics(),
        getCategories(),
        getSpecialities(),
        getDoctors(),
      ]);

      setClinics(c);
      setCategories(cat);
      setSpecialities(spec);
      setDoctors(d);
    } catch (err: unknown) {
      setPopupMessage(err instanceof Error ? err.message : "Refresh failed." );
      setPopupType("error");
    }
  }

  //Appointments actions 

  async function loadAppointments() {
    setLoadingAppointments(true);
    try {
      let result: AppointmentDTO[] = [];

      if (appointmentFilter === "clinic") {
        if (!selectedClinicId) {
          setPopupMessage("Please select a clinic.");
          setPopupType("error");
          return;
        }

        result = await getAppointmentsByClinic(selectedClinicId);
      }

      if (appointmentFilter === "doctor") {
        if (!selectedDoctorId) {
          setPopupMessage("Please select a doctor.");
          setPopupType("error");
          return;
        }

        result = await getAppointmentsByDoctor(selectedDoctorId);
      }

      if (appointmentFilter === "all") {
        const all: AppointmentDTO[] = [];

        for (const clinic of clinics) {
          const clinicAppointments = await getAppointmentsByClinic(clinic.id);
          all.push(...clinicAppointments);
        }

        const unique = Array.from(new Map(all.map(a => [a.id, a])).values());
        result = unique;
      }

      // Sort by date (soonest first)
      result.sort((a, b) =>
        new Date(a.appointmentDate).getTime() - new Date(b.appointmentDate).getTime()
      );

      setAppointments(result);
    } catch (err: unknown) {
      setPopupMessage(err instanceof Error ? err.message : "Failed to load appointments." );
      setPopupType("error");
      setAppointments([]);
    } finally {
      setLoadingAppointments(false);
    }
  }

  // Clinics actions 

  async function handleCreateClinic() {
    if (!newClinicName.trim()) {
      setPopupMessage("Clinic name is required.");
      setPopupType("error");
      return;
    }

    try {
      setSaving(true);

      await createClinic({ name: newClinicName.trim() });
      setNewClinicName("");
      setPopupMessage("Clinic created");
      setPopupType("success");
      await refreshLookups();
    } catch (err: unknown) {
      setPopupMessage(err instanceof Error ? err.message : "Create clinic failed");
      setPopupType("error");
    } finally {
      setSaving(false);
    }
  }

  function startEditClinic(c: ClinicDTO) {
    setEditingClinicId(c.id);
    setEditingClinicName(c.name);
  }

  async function handleUpdateClinic() {
    if (!editingClinicId) return;

    if (!editingClinicName.trim()) {
      setPopupMessage("Clinic name is required.");
      setPopupType("error");
      return;
    }

    try {
      setSaving(true);
      
      await updateClinic(editingClinicId, { name: editingClinicName.trim() });
      setEditingClinicId(null);
      setEditingClinicName("");
      setPopupMessage("Clinic updated");
      setPopupType("success");
      await refreshLookups();
    } catch {
      setPopupMessage("Update clinic failed");
      setPopupType("error");
    } finally {
      setSaving(false);
    }
  }

  function handleDeleteClinic(id: number) {
    setPopupMessage("Delete this clinic?");
    setPopupType("error");

    setPopupConfirm(() => async () => {
      try {
        setSaving(true);
        await deleteClinic(id);
        setPopupConfirm(null);
        setPopupMessage("Clinic deleted");
        setPopupType("success");
        await refreshLookups();
      } catch {
        setPopupMessage("Delete failed (check dependencies)");
        setPopupType("error");
      } finally {
        setSaving(false);
      }
    });
  }

  // Categories actions

  async function handleCreateCategory() {
    if (!newCategoryName.trim()) {
      setPopupMessage("Category name is required.");
      setPopupType("error");
      return;
    }

    try {
      setSaving(true);
      
      await createCategory({ name: newCategoryName.trim() });
      setNewCategoryName("");
      setPopupMessage("Category created");
      setPopupType("success");
      await refreshLookups();
    } catch (err: unknown) {
      setPopupMessage(err instanceof Error ? err.message : "Create category failed");
      setPopupType("error");
    } finally {
      setSaving(false);
    }
  }

  function startEditCategory(c: CategoryDTO) {
    setEditingCategoryId(c.id);
    setEditingCategoryName(c.name);
  }

  async function handleUpdateCategory() {
    if (!editingCategoryId) return;

    if (!editingCategoryName.trim()) {
      setPopupMessage("Category name is required.");
      setPopupType("error");
      return;
    }

    try {
      setSaving(true);
      await updateCategory(editingCategoryId, { name: editingCategoryName.trim() });
      setEditingCategoryId(null);
      setEditingCategoryName("");
      setPopupMessage("Category updated");
      setPopupType("success");
      await refreshLookups();
    } catch {
      setPopupMessage("Update category failed");
      setPopupType("error");
    } finally {
      setSaving(false);
    }
  }

  function handleDeleteCategory(id: number) {
    setPopupMessage("Delete this category?");
    setPopupType("error");

    setPopupConfirm(() => async () => {
      try {
        setSaving(true);

        await deleteCategory(id);
        setPopupConfirm(null);
        setPopupMessage("Category deleted");
        setPopupType("success");
        await refreshLookups();
      } catch {
        setPopupMessage("Delete failed (check dependencies)");
        setPopupType("error");
      } finally {
        setSaving(false);
      }
    });
  }

  // Specialities actions 

  async function handleCreateSpeciality() {
    if (!newSpecialityName.trim()) {
      setPopupMessage("Speciality name is required.");
      setPopupType("error");
      return;
    }

    try {
      setSaving(true);
      await createSpeciality({ name: newSpecialityName.trim() });
      setNewSpecialityName("");
      setPopupMessage("Speciality created");
      setPopupType("success");
      await refreshLookups();
    } catch (err: unknown) {
      setPopupMessage(err instanceof Error ? err.message : "Create speciality failed");
      setPopupType("error");
    } finally {
      setSaving(false);
    }
  }

  function startEditSpeciality(s: SpecialityDTO) {
    setEditingSpecialityId(s.id);
    setEditingSpecialityName(s.name);
  }

  async function handleUpdateSpeciality() {
    if (!editingSpecialityId) return;

    if (!editingSpecialityName.trim()) {
      setPopupMessage("Speciality name is required.");
      setPopupType("error");
      return;
    }

    try {
      setSaving(true);

      await updateSpeciality(editingSpecialityId, { name: editingSpecialityName.trim() });
      setEditingSpecialityId(null);
      setEditingSpecialityName("");
      setPopupMessage("Speciality updated");
      setPopupType("success");
      await refreshLookups();
    } catch (err: unknown) {
      setPopupMessage(err instanceof Error ? err.message : "Update failed");
      setPopupType("error");
    } finally {
      setSaving(false);
    }
  }

  function handleDeleteSpeciality(id: number) {
    setPopupMessage("Delete this speciality?");
    setPopupType("error");

    setPopupConfirm(() => async () => {
      try {
        setSaving(true);
        await deleteSpeciality(id);
        setPopupConfirm(null);
        setPopupMessage("Speciality deleted");
        setPopupType("success");
        await refreshLookups();
      } catch (err: unknown) {
        setPopupMessage(err instanceof Error ? err.message : "Delete failed");
        setPopupType("error");
      } finally {
        setSaving(false);
      }
    });
  }

  // Doctors actions

  function validateDoctorForm(data: DoctorCreateDTO) {
    if (!data.firstName.trim() || !data.lastName.trim()) {
      setPopupMessage("Doctor first and last name are required.");
      setPopupType("error");
      return false;
    }

    if (!data.clinicId) {
      setPopupMessage("Please select a clinic.");
      setPopupType("error");
      return false;
    }

    if (!data.specialityId) {
      setPopupMessage("Please select a speciality.");
      setPopupType("error");
      return false;
    }

    return true;
  }

  async function handleCreateDoctor() {
    if (!validateDoctorForm(doctorForm)) return;

    try {
      setSaving(true);
      await createDoctor({
        firstName: doctorForm.firstName.trim(),
        lastName: doctorForm.lastName.trim(),
        clinicId: doctorForm.clinicId,
        specialityId: doctorForm.specialityId,
      });

      setDoctorForm({ firstName: "", lastName: "", clinicId: 0, specialityId: 0 });
      setPopupMessage("Doctor created");
      setPopupType("success");
      await refreshLookups();
    } catch (err: unknown) {
      setPopupMessage(err instanceof Error ? err.message : "Create doctor failed");
      setPopupType("error");
    } finally {
      setSaving(false);
    }
  }

  function startEditDoctor(d: DoctorDTO) {
    setEditingDoctorId(d.id);
    setEditingDoctorForm({
      firstName: d.firstName,
      lastName: d.lastName,
      clinicId: d.clinicId ,
      specialityId: d.specialityId,
    });
  }

  async function handleUpdateDoctor() {
    if (!editingDoctorId) return;
    if (!validateDoctorForm(editingDoctorForm)) return;

    try {
      setSaving(true);
      await updateDoctor(editingDoctorId, {
        firstName: editingDoctorForm.firstName.trim(),
        lastName: editingDoctorForm.lastName.trim(),
        clinicId: editingDoctorForm.clinicId,
        specialityId: editingDoctorForm.specialityId,
      });

      setEditingDoctorId(null);
      setEditingDoctorForm({ firstName: "", lastName: "", clinicId: 0, specialityId: 0 });
      setPopupMessage("Doctor updated");
      setPopupType("success");
      await refreshLookups();
    } catch {
      setPopupMessage("Update doctor failed");
      setPopupType("error");
    } finally {
      setSaving(false);
    }
  }

  function handleDeleteDoctor(id: number) {
    setPopupMessage("Delete this doctor?");
    setPopupType("error");

    setPopupConfirm(() => async () => {
      try {
        setSaving(true);
        await deleteDoctor(id);
        setPopupConfirm(null);
        setPopupMessage("Doctor deleted");
        setPopupType("success");
        await refreshLookups();
      } catch (err: unknown) {
          setPopupMessage(err instanceof Error ? err.message : "Delete failed");
          setPopupType("error");
        } finally {
          setSaving(false);
        }
    });
  }

  if (loadingLookups) return <Spinner text="Loading admin dashboard…" />;

  return (
    <main>
      <div className="page-container">
        <h1>Admin</h1>
        {saving && <p>Processing...</p>}

        <div className="card">
          <div className="tab-bar">
            <Button type="button" className={`tab-btn${activeTab === "appointments" ? " tab-active" : ""}`} onClick={() => { setActiveTab("appointments"); }}>Appointments</Button>
            <Button type="button" className={`tab-btn${activeTab === "clinics" ? " tab-active" : ""}`} onClick={() => { setActiveTab("clinics"); }}>Clinics</Button>
            <Button type="button" className={`tab-btn${activeTab === "categories" ? " tab-active" : ""}`} onClick={() => { setActiveTab("categories"); }}>Categories</Button>
            <Button type="button" className={`tab-btn${activeTab === "specialities" ? " tab-active" : ""}`} onClick={() => { setActiveTab("specialities"); }}>Specialities</Button>
            <Button type="button" className={`tab-btn${activeTab === "doctors" ? " tab-active" : ""}`} onClick={() => { setActiveTab("doctors"); }}>Doctors</Button>
          </div>
        </div>

        {activeTab === "appointments" && (
          <>
            <div className="card">
              <h2>Appointments Overview</h2>

              <div className="form-stack">
                <div>
                  <small>Filter</small>
                  <select
                    value={appointmentFilter}
                    onChange={(e) => {
                      const value = e.target.value as AppointmentFilter;
                      setAppointmentFilter(value);
                      setSelectedClinicId(0);
                      setSelectedDoctorId(0);
                      setAppointments([]);
                    }}
                  >
                    <option value="all">All (clinic-by-clinic)</option>
                    <option value="clinic">By Clinic</option>
                    <option value="doctor">By Doctor</option>
                  </select>
                </div>

                {appointmentFilter === "clinic" && (
                  <div>
                    <small>Clinic</small>
                    <select
                      value={selectedClinicId}
                      onChange={(e) => {
                        setSelectedClinicId(Number(e.target.value));
                        setAppointments([]);
                      }}
                    >
                      <option value={0}>Select Clinic</option>
                      {clinics.map((c) => (
                        <option key={c.id} value={c.id}>
                          {c.name}
                        </option>
                      ))}
                    </select>
                  </div>
                )}

                {appointmentFilter === "doctor" && (
                  <>
                    <div>
                      <small>Clinic (optional helper)</small>
                      <select
                        value={selectedClinicId}
                        onChange={(e) => {
                          setSelectedClinicId(Number(e.target.value));
                          setSelectedDoctorId(0);
                          setAppointments([]);
                        }}
                      >
                        <option value={0}>All Clinics</option>
                        {clinics.map((c) => (
                          <option key={c.id} value={c.id}>
                            {c.name}
                          </option>
                        ))}
                      </select>
                    </div>

                    <div>
                      <small>Doctor</small>
                      <select
                        value={selectedDoctorId}
                        onChange={(e) => {
                          setSelectedDoctorId(Number(e.target.value));
                          setAppointments([]);
                        }}
                      >
                        <option value={0}>Select Doctor</option>
                        {doctorsForSelectedClinic.map((d) => (
                          <option key={d.id} value={d.id}>
                            {d.firstName} {d.lastName}
                          </option>
                        ))}
                      </select>
                    </div>
                  </>
                )}

                <Button type="button" onClick={() => { void loadAppointments(); }}>
                Load Appointments
              </Button>

                {loadingAppointments && <Spinner inline text="Loading appointments…" />}
              </div>
            </div>

            {appointments.length === 0 && !loadingAppointments && (
              <EmptyState
                icon={
                  <svg width="32" height="32" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="1.5" strokeLinecap="round" strokeLinejoin="round">
                    <rect x="3" y="4" width="18" height="18" rx="2"/>
                    <line x1="16" y1="2" x2="16" y2="6"/>
                    <line x1="8" y1="2" x2="8" y2="6"/>
                    <line x1="3" y1="10" x2="21" y2="10"/>
                    <line x1="8" y1="14" x2="16" y2="14"/>
                  </svg>
                }
                title="No appointments to display"
                message="Select a filter above and click Load Appointments."
              />
            )}

            {appointments.map((a) => (
              <div key={a.id} className="card">
                <p>
                  <strong>Date:</strong> {new Date(a.appointmentDate).toLocaleString()}
                </p>
                <p>
                  <strong>Duration:</strong> {a.durationMinutes} minutes
                </p>
                <p>
                  <strong>Clinic:</strong> {a.clinicName}
                </p>
                <p>
                  <strong>Doctor:</strong> {a.doctorName}
                </p>
                <p>
                  <strong>Category:</strong> {a.categoryName}
                </p>
                <p>
                  <strong>PatientId:</strong> {a.patientId ?? "(guest)"}
                </p>
              </div>
            ))}
          </>
        )}

        {activeTab === "clinics" && (
          <>
            <div className="card">
              <h2>Manage Clinics</h2>

              <div className="form-row">
                <input
                  placeholder="Clinic name"
                  value={newClinicName}
                  onChange={(e) => { setNewClinicName(e.target.value); }}
                />
                <Button type="button" onClick={() => {void handleCreateClinic();}}>Add Clinic</Button>
              </div>
            </div>

            {clinics.map((c) => (
              <div key={c.id} className="card">
                {editingClinicId === c.id ? (
                  <>
                    <input
                      value={editingClinicName}
                      onChange={(e) => {setEditingClinicName(e.target.value);}}
                    />
                    <div className="card-actions">
                      <Button type="button" onClick={() => { void handleUpdateClinic(); }}>Save</Button>
                      <Button
                        type="button"
                        onClick={() => {
                          setEditingClinicId(null);
                          setEditingClinicName("");
                        }}
                      >
                        Cancel
                      </Button>
                    </div>
                  </>
                ) : (
                  <>
                    <p>
                      <strong>{c.name}</strong>
                    </p>

                    <div className="card-actions">
                      <Button type="button" onClick={() => {startEditClinic(c);}}>Edit</Button>
                      <Button type="button" className="cancel-btn" onClick={() => {handleDeleteClinic(c.id);}}>Delete</Button>
                    </div>
                  </>
                )}
              </div>
            ))}
          </>
        )}

        {activeTab === "categories" && (
          <>
            <div className="card">
              <h2>Manage Categories</h2>

              <div className="form-row">
                <input
                  placeholder="Category name"
                  value={newCategoryName}
                  onChange={(e) => { setNewCategoryName(e.target.value); }}
                />
                <Button type="button" onClick={() => { void handleCreateCategory(); }}>Add Category</Button>
              </div>
            </div>

            {categories.map((c) => (
              <div key={c.id} className="card">
                {editingCategoryId === c.id ? (
                  <>
                    <input
                      value={editingCategoryName}
                      onChange={(e) => {setEditingCategoryName(e.target.value);}}
                    />
                    <div className="card-actions">
                      <Button type="button" onClick={() => { void handleUpdateCategory(); }}>Save</Button>
                      <Button
                        type="button"
                        onClick={() => {
                          setEditingCategoryId(null);
                          setEditingCategoryName("");
                        }}
                      >
                        Cancel
                      </Button>
                    </div>
                  </>
                ) : (
                  <>
                    <p>
                      <strong>{c.name}</strong>
                    </p>
                    <div className="card-actions">
                      <Button type="button" onClick={() => {startEditCategory(c);}}>Edit</Button>
                      <Button type="button" className="cancel-btn" onClick={() => {handleDeleteCategory(c.id);}}>Delete</Button>
                    </div>
                  </>
                )}
              </div>
            ))}
          </>
        )}

        {activeTab === "specialities" && (
          <>
            <div className="card">
              <h2>Manage Specialities</h2>

              <div className="form-row">
                <input
                  placeholder="Speciality name"
                  value={newSpecialityName}
                  onChange={(e) => { setNewSpecialityName(e.target.value); }}
                />
                <Button type="button" onClick={() => {void handleCreateSpeciality();}}>Add Speciality</Button>
              </div>
            </div>

            {specialities.map((s) => (
              <div key={s.id} className="card">
                {editingSpecialityId === s.id ? (
                  <>
                    <input
                      value={editingSpecialityName}
                      onChange={(e) => {setEditingSpecialityName(e.target.value);}}
                    />
                    <div className="card-actions">
                      <Button type="button" onClick={() => { void handleUpdateSpeciality(); }}>Save</Button>
                      <Button
                        type="button"
                        onClick={() => {
                          setEditingSpecialityId(null);
                          setEditingSpecialityName("");
                        }}
                      >
                        Cancel
                      </Button>
                    </div>
                  </>
                ) : (
                  <>
                    <p>
                      <strong>{s.name}</strong>
                    </p>
                    <div className="card-actions">
                      <Button type="button" onClick={() => {startEditSpeciality(s);}}>Edit</Button>
                      <Button type="button" className="cancel-btn" onClick={() => {handleDeleteSpeciality(s.id);}}>Delete</Button>
                    </div>
                  </>
                )}
              </div>
            ))}
          </>
        )}

        {activeTab === "doctors" && (
          <>
            <div className="card">
              <h2>Add Doctor</h2>

              <div className="form-group">
                <input
                  placeholder="First name"
                  value={doctorForm.firstName}
                  onChange={(e) => {
                  setDoctorForm((p) => ({ ...p, firstName: e.target.value }));
                }}
                />

                <input
                  placeholder="Last name"
                  value={doctorForm.lastName}
                  onChange={(e) => {
                    setDoctorForm((p) => ({ ...p, lastName: e.target.value }));
                  }}
                />

                <div>
                  <small>Clinic</small>
                  <select
                    value={doctorForm.clinicId}
                    onChange={(e) => {
                      setDoctorForm((p) => ({ ...p, clinicId: Number(e.target.value) }))
                    }}
                  >
                    <option value={0}>Select Clinic</option>
                    {clinics.map((c) => (
                      <option key={c.id} value={c.id}>
                        {c.name}
                      </option>
                    ))}
                  </select>
                </div>

                <div>
                  <small>Speciality</small>
                  <select
                    value={doctorForm.specialityId}
                    onChange={(e) => {
                      setDoctorForm((p) => ({ ...p, specialityId: Number(e.target.value) }))
                    }}
                  >
                    <option value={0}>Select Speciality</option>
                    {specialities.map((s) => (
                      <option key={s.id} value={s.id}>
                        {s.name}
                      </option>
                    ))}
                  </select>
                </div>

                <Button type="button" onClick={() => { void handleCreateDoctor(); }}>Add Doctor</Button>
              </div>
            </div>

            <h2 style={{ textAlign: "center", marginTop: 20 }}>Doctors</h2>

            {doctors.map((d) => (
              <div key={d.id} className="card">
                {editingDoctorId === d.id ? (
                  <>
                    <input
                      placeholder="First name"
                      value={editingDoctorForm.firstName}
                      onChange={(e) => {
                        setEditingDoctorForm((p) => ({ ...p, firstName: e.target.value }));
                      }}
                    />
                    <input
                      placeholder="Last name"
                      value={editingDoctorForm.lastName}
                      onChange={(e) => {
                        setEditingDoctorForm((p) => ({ ...p, lastName: e.target.value }));
                      }}
                    />

                    <div>
                      <small>Clinic</small>
                      <select
                        value={editingDoctorForm.clinicId}
                        onChange={(e) => {
                          setEditingDoctorForm((p) => ({ ...p, clinicId: Number(e.target.value) }))
                        }}
                      >
                        <option value={0}>Select Clinic</option>
                        {clinics.map((c) => (
                          <option key={c.id} value={c.id}>
                            {c.name}
                          </option>
                        ))}
                      </select>
                    </div>

                    <div>
                      <small>Speciality</small>
                      <select
                        value={editingDoctorForm.specialityId}
                        onChange={(e) => {
                          setEditingDoctorForm((p) => ({ ...p, specialityId: Number(e.target.value) }))
                        }}
                      >
                        <option value={0}>Select Speciality</option>
                        {specialities.map((s) => (
                          <option key={s.id} value={s.id}>
                            {s.name}
                          </option>
                        ))}
                      </select>
                    </div>

                    <div className="card-actions">
                      <Button type="button" onClick={() => { void handleUpdateDoctor(); }}>Save</Button>
                      <Button
                        type="button"
                        onClick={() => {
                          setEditingDoctorId(null);
                          setEditingDoctorForm({ firstName: "", lastName: "", clinicId: 0, specialityId: 0 });
                        }}
                      >
                        Cancel
                      </Button>
                    </div>
                  </>
                ) : (
                  <>
                    <p>
                      <strong>{d.firstName} {d.lastName}</strong>
                    </p>
                    <p>
                      <small>
                        {clinics.find((c) => c.id === d.clinicId)?.name ?? `Clinic #${d.clinicId}`}
                        {" · "}
                        {specialities.find((s) => s.id === d.specialityId)?.name ?? `Speciality #${d.specialityId}`}
                      </small>
                    </p>

                    <div className="card-actions">
                      <Button type="button" onClick={() => { startEditDoctor(d); }}>Edit</Button>
                      <Button type="button" className="cancel-btn" onClick={() => { handleDeleteDoctor(d.id); }}>Delete</Button>
                    </div>
                  </>
                )}
              </div>
            ))}
          </>
        )}
      </div>
    </main>
  );
}
