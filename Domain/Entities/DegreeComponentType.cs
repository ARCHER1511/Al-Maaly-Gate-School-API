namespace Domain.Entities
{
    public class DegreeComponentType : BaseEntity
    {
        public string SubjectId { get; set; } = string.Empty;
        public Subject Subject { get; set; } = null!;

        public string ComponentName { get; set; } = string.Empty; // e.g., "Oral", "Exam", "Practical", "Project", "Assignment"
        public int Order { get; set; } = 1; // Display order
        public double MaxScore { get; set; } = 0; // Maximum score for this component
        public bool IsActive { get; set; } = true;

        // Navigation
        public ICollection<DegreeComponent> Components { get; set; } = new HashSet<DegreeComponent>();
    }
}
