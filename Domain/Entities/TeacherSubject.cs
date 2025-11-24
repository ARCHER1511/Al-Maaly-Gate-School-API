namespace Domain.Entities
{
    public class TeacherSubject : BaseEntity
    {
        public string TeacherId { get; set; } = string.Empty;
        public Teacher Teacher { get; set; } = default!;

        public string SubjectId { get; set; } = string.Empty;
        public Subject Subject { get; set; } = default!;
    }
}
