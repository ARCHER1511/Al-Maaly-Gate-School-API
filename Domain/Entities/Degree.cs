using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        // New: Detailed components for degree calculation
        public double? OralScore { get; set; }
        public double? OralMaxScore { get; set; }
        public double? ExamScore { get; set; }
        public double? ExamMaxScore { get; set; }
        public double? PracticalScore { get; set; }
        public double? PracticalMaxScore { get; set; }

        // Method to calculate total score from components
        public void CalculateTotalScore()
        {
            if (OralScore.HasValue || ExamScore.HasValue || PracticalScore.HasValue)
            {
                Score = (OralScore ?? 0) + (ExamScore ?? 0) + (PracticalScore ?? 0);
                MaxScore = (OralMaxScore ?? 0) + (ExamMaxScore ?? 0) + (PracticalMaxScore ?? 0);
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