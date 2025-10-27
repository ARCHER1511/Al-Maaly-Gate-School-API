namespace Domain.Entities
{
    public class Subject : BaseEntity
    {
        public string ClassYear { get; set; } = string.Empty;

        public string ClassId { get; set; } = string.Empty;
        public Class Class { get; set; } = null!;

        public string TeacherId { get; set; } = string.Empty;
        public Teacher? Teacher { get; set; } = new();

        public List<Exam> Exams { get; set; } = new();
        public List<ClassAppointment> ClassAppointments { get; set; } = new();
    }
}