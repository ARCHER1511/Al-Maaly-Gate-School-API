namespace Domain.Entities
{
    public class Student : UserBase
    {
        public string ClassYear { get; set; } = string.Empty;
        public int Age { get; set; }
        public string Nationality { get; set; } = string.Empty;
        public string IqamaNumber { get; set; } = string.Empty;
        public string PassportNumber { get; set; } = string.Empty;

        public string? ClassId { get; set; }
        public Class? Class { get; set; }

        // Add Curriculum reference
        public string? CurriculumId { get; set; }
        public Curriculum? Curriculum { get; set; }

        public ICollection<ParentStudent> Parents { get; set; } = new HashSet<ParentStudent>();
        public ICollection<Degree> Degrees { get; set; } = new HashSet<Degree>();
        public ICollection<Certificate> Certificates { get; set; } = new HashSet<Certificate>();
    }
}
