using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class Certificate : BaseEntity
    {
        public string StudentId { get; set; } = string.Empty;
        public Student Student { get; set; } = null!;

        public double GPA { get; set; }
        public DateTime IssuedDate { get; set; } = DateTime.Now;

        public string CurriculumId { get; set; } = string.Empty;
        public Curriculum? Curriculum { get; set; }

        public string TemplateName { get; set; } = "Default";
        public DegreeType DegreeType { get; set; }

        // Add Grade and Class references
        public string? GradeId { get; set; }
        public Grade? Grade { get; set; }

        public string? ClassId { get; set; }
        public Class? Class { get; set; }

        // Add School Year/Semester information
        public string? AcademicYear { get; set; }
        public string? Semester { get; set; }

        // Add status flags
        public bool IsArchived { get; set; }
        public bool IsVerified { get; set; }
        public DateTime? VerifiedDate { get; set; }
        public string? VerifiedBy { get; set; }

        // PDF storage
        [Column(TypeName = "varbinary(max)")]
        public byte[] PdfData { get; set; } = null!;

        public string FileName { get; set; } = null!;
        public string ContentType { get; set; } = "application/pdf";
        public long FileSize { get; set; }

        // Add certificate number for official records
        public string? CertificateNumber { get; set; }
    }
}
