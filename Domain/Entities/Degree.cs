namespace Domain.Entities
{
    public class Degree : BaseEntity
    {
        public string StudentId { get; set; } = string.Empty;
        public Student Student { get; set; } = null!;

        public string SubjectId { get; set; } = string.Empty;
        public Subject Subject { get; set; } = null!;

        public DegreeType DegreeType { get; set; }

        public double Score { get; set; }
        public double MaxScore { get; set; }

        public string SubjectName { get; set; } = string.Empty;


        // New: Dynamic components
        public ICollection<DegreeComponent> Components { get; set; } = new HashSet<DegreeComponent>();

        // Method to calculate total score from components
        public void CalculateTotalScore()
        {
            if (Components != null && Components.Any())
            {
                Score = Components.Sum(c => c.Score);
                MaxScore = Components.Sum(c => c.MaxScore);
            }
        }
    }

    public enum DegreeType
    {
        MidTerm1 = 1,
        Final1 = 2,
        MidTerm2 = 3,
        Final2 = 4
    }
}