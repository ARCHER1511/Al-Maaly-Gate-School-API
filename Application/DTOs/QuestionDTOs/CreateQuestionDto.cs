using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.QuestionDTOs
{
    public class CreateQuestionDto
    {
        public string? Type { get; init; }
        public string? Content { get; init; }
        public string? CorrectAnswer { get; init; }
        public decimal Degree { get; init; }
        public bool IsRequired { get; init; }
    }
}
