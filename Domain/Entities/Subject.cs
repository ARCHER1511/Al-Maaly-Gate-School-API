namespace Domain.Entities
{
    public class Subject : BaseEntity
    {
        public string SubjectName { get; set; } = string.Empty;
        public string ClassYear { get; set; } = string.Empty;

        public string ClassId { get; set; } = string.Empty;
        public Class Class { get; set; } = null!;

        public string TeacherId { get; set; } = string.Empty;
        public Teacher? Teacher { get; set; }

        public List<Exam>? Exams { get; set; }
        public List<ClassAppointment>? ClassAppointments { get; set; } 
    }
}