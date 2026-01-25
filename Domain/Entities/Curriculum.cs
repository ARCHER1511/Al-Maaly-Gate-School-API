namespace Domain.Entities
{
    public class Curriculum : BaseEntity
    {
        public string Name { get; set; } = string.Empty; // e.g., "Egyptian Curriculum", "British Curriculum"
        public string Code { get; set; } = string.Empty; // e.g., "EGY", "UK"
        public string? Description { get; set; } = string.Empty;

        // Add more curriculum details
        public string? Country { get; set; } // Country of origin
        public string? Language { get; set; } // Primary language
        public bool IsActive { get; set; } = true;
        public int SortOrder { get; set; } // For ordering in UI

        // Academic structure
        public int? TotalGrades { get; set; } // How many grades/years in this curriculum
        public string? AcademicSystem { get; set; } // e.g., "12-year", "13-year", "K-12"

        // Template configuration
        public string? DefaultTemplatePath { get; set; }
        public string? CertificateHeader { get; set; }
        public string? CertificateFooter { get; set; }

        // Grading system specific to curriculum
        public string? GradingScale { get; set; } // e.g., "Percentage", "LetterGrade", "GPA"
        public double? PassingGrade { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Each curriculum has its own grades
        public ICollection<Grade> Grades { get; set; } = new HashSet<Grade>();

        // Students enrolled in this curriculum
        public ICollection<Student> Students { get; set; } = new HashSet<Student>();

        // Teachers specialized in this curriculum
        public ICollection<Teacher> Teachers { get; set; } = new HashSet<Teacher>();

        // Certificates issued for this curriculum
        public ICollection<Certificate> Certificates { get; set; } = new HashSet<Certificate>();
    }
}
