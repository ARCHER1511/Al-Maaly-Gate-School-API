using Application.Interfaces;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class GpaCalculator : IGpaCalculator
    {
        public double CalculateGpa(List<Degree> degrees)
        {
            if (degrees == null || degrees.Count == 0)
                return 0.0;

            double totalWeighted = 0.0;
            double totalCredits = 0.0;

            // Group by subject
            var grouped = degrees.GroupBy(d => d.SubjectId);

            foreach (var group in grouped)
            {
                var degree = group.First(); // assume total marks per subject
                double totalScore = group.Sum(x => x.Score);
                double totalMax = group.Sum(x => x.MaxScore);
                double percentage = totalMax > 0 ? (totalScore / totalMax) * 100.0 : 0.0;

                double subjectGpa = GetSubjectGpa(totalScore, totalMax);

                double creditHours = degree.Subject?.CreditHours ?? 3.0;

                totalWeighted += subjectGpa * creditHours;
                totalCredits += creditHours;
            }

            if (totalCredits == 0) return 0.0;

            return Math.Round(totalWeighted / totalCredits, 2);
        }

        public double GetSubjectGpa(double score, double maxScore)
        {
            double percent = maxScore > 0 ? (score / maxScore) * 100.0 : 0.0;

            // School-specific GPA scale
            if (percent >= 97) return 4.0;
            if (percent >= 93) return 3.7;
            if (percent >= 89) return 3.4;
            if (percent >= 85) return 3.1;
            if (percent >= 81) return 2.8;
            if (percent >= 77) return 2.5;
            if (percent >= 73) return 2.2;
            if (percent >= 69) return 1.9;
            if (percent >= 65) return 1.6;
            if (percent >= 61) return 1.3;
            if (percent >= 57) return 1.0;
            if (percent >= 50) return 0.7;
            return 0.0;
        }

        public string GetLetterGrade(double gpa)
        {
            if (gpa >= 4.0) return "A+";
            if (gpa >= 3.7) return "A";
            if (gpa >= 3.4) return "A-";
            if (gpa >= 3.1) return "B+";
            if (gpa >= 2.8) return "B";
            if (gpa >= 2.5) return "B-";
            if (gpa >= 2.2) return "C+";
            if (gpa >= 1.9) return "C";
            if (gpa >= 1.6) return "C-";
            if (gpa >= 1.3) return "D+";
            if (gpa >= 1.0) return "D";
            if (gpa >= 0.7) return "D-";
            return "F";
        }
    }
}
