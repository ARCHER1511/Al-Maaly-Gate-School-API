using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.DegreesDTOs
{
    public class DegreeItemDto
    {
        public string DegreeId { get; set; } = string.Empty;
        public string SubjectId { get; set; } = string.Empty;
        public string SubjectName { get; set; } = string.Empty;
        public string DegreeType { get; set; } = string.Empty;

        // Total values
        public double Score { get; set; }
        public double MaxScore { get; set; }

        // Component details (if available)
        public double? OralScore { get; set; }
        public double? OralMaxScore { get; set; }
        public double? ExamScore { get; set; }
        public double? ExamMaxScore { get; set; }
        public double? PracticalScore { get; set; }
        public double? PracticalMaxScore { get; set; }

        // Flag to indicate if components are present
        public bool HasComponents => OralScore.HasValue || ExamScore.HasValue || PracticalScore.HasValue;
    }

}
