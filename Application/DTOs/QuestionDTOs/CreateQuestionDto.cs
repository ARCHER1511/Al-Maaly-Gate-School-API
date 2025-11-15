using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.QuestionDTOs
{
    public class CreateQuestionDto
    {
        public string Content { get; set; } = string.Empty;
        public decimal Degree { get; set; }
        public QuestionTypes Type { get; set; }
        public string TeacherId { get; set; } = string.Empty;
        public List<ChoiceDto>? Choices { get; set; }
        public string? CorrectChoiceId { get; set; }
        public string? CorrectTextAnswer { get; set; }
        public bool? TrueAndFalses { get; set; }
    }

    public class ChoiceDto
    {
        public string Text { get; set; } = string.Empty;
        public bool IsCorrect { get; set; } = false;
    }
}
