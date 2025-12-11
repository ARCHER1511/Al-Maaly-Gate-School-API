using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class BulkJobStatus
    {
        public string JobId { get; set; } = string.Empty;
        public string Status { get; set; } = "Pending"; // Pending, Running, Completed, Failed
        public int TotalStudents { get; set; }
        public int Processed { get; set; }
        public int Successful { get; set; }
        public int Failed { get; set; }
        public List<string> Errors { get; set; } = new();
        public DateTime StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public List<Certificate> Certificates { get; set; } = new();

        public string ClassId { get; set; } = string.Empty;
        public DegreeType DegreeType { get; set; }
        public string? AcademicYear { get; set; }
    }
}
