namespace Domain.Entities
{
    public class Grade : BaseEntity
    {
        public string GradeName { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;

        // Link to Curriculum
        public string CurriculumId { get; set; } = string.Empty;
        public Curriculum Curriculum { get; set; } = default!;

        public List<Class> Classes { get; set; } = new List<Class>();
        public List<Subject> Subjects { get; set; } = new List<Subject>();
    }
}
