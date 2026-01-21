namespace Domain.Entities
{
    public class Subject : BaseEntity
    {
        public string SubjectName { get; set; } = string.Empty;

        public string GradeId { get; set; } = string.Empty;
        public Grade Grade { get; set; } = null!;

        public double CreditHours { get; set; } = 3.0;

        public ICollection<TeacherSubject>? TeacherSubjects { get; set; } = new HashSet<TeacherSubject>();

        public ICollection<Exam>? Exams { get; set; } = new HashSet<Exam>();
        public ICollection<ClassAppointment>? ClassAppointments { get; set; } = new HashSet<ClassAppointment>();

        public ICollection<Degree>? Degrees { get; set; } = new HashSet<Degree>();
        public ICollection<DegreeComponentType> ComponentTypes { get; set; } = new HashSet<DegreeComponentType>();
    }
}