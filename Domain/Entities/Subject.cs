namespace Domain.Entities
{
    public class Subject : BaseEntity
    {
        public string SubjectName { get; set; } = string.Empty;

        public string GradeId { get; set; } = string.Empty;
        public Grade Grade { get; set; } = null!;

        public double CreditHours { get; set; } = 3.0;

        public List<TeacherSubject>? TeacherSubjects { get; set; } = new();

        public List<Exam>? Exams { get; set; }
        public List<ClassAppointment>? ClassAppointments { get; set; }

        public List<Degree>? Degrees { get; set; } = new();
    }
}