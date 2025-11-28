namespace Domain.Entities
{
    public class Class : BaseEntity
    {
        public string ClassName { get; set; } = string.Empty;

        public string GradeId { get; set; } = string.Empty ;
        public Grade Grade { get; set; }

        public List<TeacherClass> TeacherClasses { get; set; } = new();
        public List<Student>? Students { get; set; }
        public List<ClassAssets>? ClassAssets { get; set; }
        public List<ClassAppointment>? ClassAppointments { get; set; }
        public List<Exam>? Exams { get; set; }
    }
}
