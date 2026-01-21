namespace Domain.Entities
{
    public class Class : BaseEntity
    {
        public string ClassName { get; set; } = string.Empty;

        public string GradeId { get; set; } = string.Empty ;
        public Grade Grade { get; set; } = null!;

        public ICollection<TeacherClass> TeacherClasses { get; set; } = new HashSet<TeacherClass>();
        public ICollection<Student>? Students { get; set; } = new HashSet<Student>();
        public ICollection<ClassAssets>? ClassAssets { get; set; } = new HashSet<ClassAssets>();
        public ICollection<ClassAppointment>? ClassAppointments { get; set; } = new HashSet<ClassAppointment>();
        public ICollection<Exam>? Exams { get; set; } = new HashSet<Exam>();
    }
}
