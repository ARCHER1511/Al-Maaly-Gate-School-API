using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class QuestionExamTeacher
    {
        public int QuestionId { get; set; };
        public Question? Question { get; set; }
        public string TeacherId { get; set; } = string.Empty;
        public Teacher? Teacher { get; set; }

        public string ExamId { get; set; } = string.Empty;
        public Exam? Exam { get; set; }
    }
}
