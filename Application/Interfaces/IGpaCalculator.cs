using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IGpaCalculator
    {
        double CalculateGpa(List<Degree> degrees);
        double GetSubjectGpa(double score, double maxScore);
        string GetLetterGrade(double gpa);
    }
}
