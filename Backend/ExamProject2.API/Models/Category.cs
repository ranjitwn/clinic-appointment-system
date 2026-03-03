namespace ExamProject2.API.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<Appointment>? Appointments { get; set; }
    }
}
