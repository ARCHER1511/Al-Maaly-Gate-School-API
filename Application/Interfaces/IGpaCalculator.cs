using Domain.Entities;

namespace Application.Interfaces
{
    public interface IGpaCalculator
    {
        double CalculateGpa(List<Degree> degrees);
        double GetSubjectGpa(double score, double maxScore);
        string GetLetterGrade(double gpa);
    }
}
