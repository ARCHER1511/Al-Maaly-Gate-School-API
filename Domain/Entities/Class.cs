namespace Domain.Entities
{
    public class Class : BaseEntity
    {
        public string ClassName { get; set; } = string.Empty;
        public string ClassYear { get; set; } = string.Empty;
        public List<Student>? Students { get; set; }
        public List<ClassAssets>? ClassAssets { get; set; }
        public List<ClassAppointment>? ClassAppointments { get; set; }
        public List<Subject>? Subjects { get; set; }
        public List<Exam>? Exams { get; set; }
    }
}
