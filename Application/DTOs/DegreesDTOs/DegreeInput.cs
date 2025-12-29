using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.DegreesDTOs
{
    public class DegreeInput
    {
        public string SubjectId { get; set; } = string.Empty;
        public DegreeType DegreeType { get; set; }

        // Either provide total score directly
        public double? Score { get; set; }
        public double? MaxScore { get; set; }

        // Or provide components (optional)
        public double? OralScore { get; set; }
        public double? OralMaxScore { get; set; }
        public double? ExamScore { get; set; }
        public double? ExamMaxScore { get; set; }
        public double? PracticalScore { get; set; }
        public double? PracticalMaxScore { get; set; }
    }
}
