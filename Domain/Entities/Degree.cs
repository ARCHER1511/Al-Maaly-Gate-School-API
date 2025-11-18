using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Degree : BaseEntity
    {
        public string StudentId { get; set; }
        public Student Student { get; set; } = null!;

        // Link to Subject to get CreditHours
        public string SubjectId { get; set; } = string.Empty;
        public Subject Subject { get; set; } = null!;

        public DegreeType DegreeType { get; set; } // MidTerm1 / Final1 / MidTerm2 / Final2

        public double Score { get; set; }
        public double MaxScore { get; set; } // example: 20 or 80

        // optional convenience property (duplicate of Subject.SubjectName)
        public string SubjectName { get; set; } = string.Empty;
    }

    public enum DegreeType
    {
        MidTerm1 = 1,
        Final1 = 2,
        MidTerm2 = 3,
        Final2 = 4
    }
}
