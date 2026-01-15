using Domain.Entities;

namespace Application.DTOs
{
    public class JobStatus
    {
        public string JobId { get; set; } = string.Empty;
        public string ClassId { get; set; } = string.Empty;
        public DegreeType DegreeType { get; set; }
        public string? AcademicYear { get; set; }
        public string Status { get; set; } = "Pending";
        public string? Message { get; set; }
        public int CertificatesGenerated { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
    }
}
