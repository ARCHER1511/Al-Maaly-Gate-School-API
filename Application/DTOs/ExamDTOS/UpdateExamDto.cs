using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.ExamDTOS
{
    public class UpdateExamDto
    {
        public string ExamName { get; set; } = string.Empty;
        public DateTimeOffset Start { get; set; }
        public DateTimeOffset End { get; set; }
        public decimal MinMark { get; set; }
        public decimal FullMark { get; set; }
    }
}
