namespace Domain.Entities
{
    public class Class : BaseEntity
    {
        public string ClassYear { get; set; } = string.Empty;
        public string? TeacherId { get; set; } = string.Empty;
        public Teacher? Teacher { get; set; }
        
        public List<Student> Students { get; set; } = new List<Student>();
        public List<ClassSubject> ClassSubjects { get; set; } = new List<ClassSubject>();
        public List<ClassAssets> ClassAssets { get; set; } = new List<ClassAssets>();
        public List<ClassAppointment> ClassAppointments { get; set; } = new List<ClassAppointment>();

    }
}
