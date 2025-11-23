namespace Domain.Entities
{
    public class TeacherClass : BaseEntity
    {
        public string TeacherId { get; set; } = default!;
        public Teacher Teacher { get; set; } = default!;

        public string ClassId { get; set; } = default!;
        public Class Class { get; set; } = default!;

        public DateTime AssignedAt { get; set; } = DateTime.Now;
        public string Notes { get; set; } = string.Empty;
    }
}
