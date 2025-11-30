using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.ClassDTOs
{
    public class ClassStatisticsDto
    {
        public string ClassId { get; set; } = string.Empty;
        public string ClassName { get; set; } = string.Empty;
        public double AverageGpa { get; set; }
        public double AttendanceRate { get; set; }
        public int CompletedExams { get; set; }
        public int PendingAssignments { get; set; }
        public int TotalStudents { get; set; }
        public int TotalTeachers { get; set; }
    }
}
