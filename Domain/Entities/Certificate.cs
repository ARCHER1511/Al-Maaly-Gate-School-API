using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Certificate : BaseEntity
    {
        public string StudentId { get; set; } = string.Empty;
        public Student Student { get; set; } = null!;

        public double GPA { get; set; }
        public DateTime IssuedDate { get; set; } = DateTime.UtcNow;
        
        public string TemplateName { get; set; } = "Default";
        public DegreeType DegreeType { get; set; } // Add this field
        
        // Add these properties for PDF storage
        public byte[] PdfData { get; set; } = null!;
        public string FileName { get; set; } = null!;
        public string ContentType { get; set; } = "application/pdf";
        
        // Optional: File size for tracking
        public long FileSize { get; set; }
    }
}
