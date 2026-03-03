namespace ExamProject2.API.Models
{
    public class Clinic
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<Doctor>? Doctors { get; set; }
        public ICollection<Appointment>? Appointments { get; set; }
    }
}
