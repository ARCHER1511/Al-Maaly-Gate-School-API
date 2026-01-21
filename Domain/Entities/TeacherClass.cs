namespace Domain.Entities
{
    public class TeacherClass : BaseEntity
    {
        public string TeacherId { get; set; } = string.Empty;
        public Teacher Teacher { get; set; } = null!;

        public string ClassId { get; set; } = string.Empty;
        public Class Class { get; set; } = null!;

        public DateTime AssignedAt { get; set; } = DateTime.Now;
        public string Notes { get; set; } = string.Empty;
    }
}
