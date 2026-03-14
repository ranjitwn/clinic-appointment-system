namespace ClinicAppointment.API.Models
{
    public class Clinic
    {
        public int Id { get; set; }
        public required string Name { get; set; }

        public ICollection<Doctor>? Doctors { get; set; }
        public ICollection<Appointment>? Appointments { get; set; }
    }
}
