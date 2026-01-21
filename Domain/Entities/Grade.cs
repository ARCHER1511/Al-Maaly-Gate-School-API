namespace Domain.Entities
{
    public class Grade : BaseEntity
    {
        public string GradeName { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;

        // Link to Curriculum
        public string CurriculumId { get; set; } = string.Empty;
        public Curriculum Curriculum { get; set; } = null!;

        public ICollection<Class> Classes { get; set; } = new HashSet<Class>();
        public ICollection<Subject> Subjects { get; set; } = new HashSet<Subject>();
    }
}
