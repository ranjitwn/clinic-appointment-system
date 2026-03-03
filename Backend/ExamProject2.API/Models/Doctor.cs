namespace ExamProject2.API.Models
{
    public class Doctor
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public int ClinicId { get; set; }
        public Clinic? Clinic { get; set; }

        public int SpecialityId { get; set; }
        public Speciality? Speciality { get; set; }

        public ICollection<Appointment>? Appointments { get; set; }
    }
}
