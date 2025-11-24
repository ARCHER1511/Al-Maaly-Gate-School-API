namespace Domain.Entities
{
    public class Subject : BaseEntity
    {
        public string SubjectName { get; set; } = string.Empty;
        public string ClassYear { get; set; } = string.Empty;

        public string ClassId { get; set; } = string.Empty;
        public Class Class { get; set; } = null!;

        public double CreditHours { get; set; } = 3.0;

        public List<TeacherSubject>? TeacherSubjects { get; set; } = new();

        public List<Exam>? Exams { get; set; }
        public List<ClassAppointment>? ClassAppointments { get; set; }

        public List<Degree>? Degrees { get; set; } = new();
    }
}