using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class ExamQuestion : BaseEntity
    {
        public string ExamId { get; set; } = string.Empty;
        public Exam Exam { get; set; } = null!;
        
        public string QuestionId { get; set; } = string.Empty;
        public Question Question { get; set; } = null!;
    }
}
