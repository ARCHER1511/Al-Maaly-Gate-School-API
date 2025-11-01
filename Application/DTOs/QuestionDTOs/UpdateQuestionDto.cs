using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.QuestionDTOs
{
    public class UpdateQuestionDto
    {
        public string Content { get; set; } = string.Empty;
        public decimal Degree { get; set; }
    }
}
