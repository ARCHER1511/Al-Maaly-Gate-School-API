using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.ExamDTOS
{
    public class CreateExamDto
    {
        public string Title { get; init; } = string.Empty;
        public DateTime? Date { get; init; }
        public int DurationMinutes { get; init; }
        public IEnumerable<int> QuestionIds { get; init; } = Enumerable.Empty<int>();
    }
}
