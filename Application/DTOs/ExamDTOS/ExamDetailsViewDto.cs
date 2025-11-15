using Application.DTOs.QuestionDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.ExamDTOS
{
    public class ExamDetailsViewDto
    {
        public string Id { get; set; }
        public string ExamName { get; set; }

        public string SubjectId { get; set; }
        public string SubjectName { get; set; }

        public string ClassId { get; set; }
        public string ClassName { get; set; }

        public string TeacherId { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public decimal MinMark { get; set; }
        public decimal FullMark { get; set; }

        public List<QuestionViewDto> Questions { get; set; }
    }

}
