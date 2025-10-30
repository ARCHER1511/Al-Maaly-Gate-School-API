namespace Domain.Entities
{
    public class Class : BaseEntity
    {
        public string ClassName { get; set; } = string.Empty;
        public string ClassYear { get; set; } = string.Empty;
        public List<Student>? Students { get; set; } = new();
        public List<ClassAssets>? ClassAssets { get; set; } = new();
        public List<ClassAppointment>? ClassAppointments { get; set; } = new();
        public List<Subject>? Subjects { get; set; } = new();
        public List<Exam>? Exams { get; set; } = new();
    }
}
