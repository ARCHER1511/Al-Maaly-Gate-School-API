using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.StudentExamAnswerDTOs
{
    public class StudentAnswerWithQuestionDto
    {
        public string ExamId { get; set; } = string.Empty;
        public string StudentId { get; set; } = string.Empty;
        public string QuestionId { get; set; } = string.Empty;

        public string QuestionContent { get; set; } = string.Empty;
        public string QuestionType { get; set; } = string.Empty;
        public decimal QuestionDegree { get; set; }

        public string? StudentChoiceText { get; set; }
        public bool? StudentTrueFalseAnswer { get; set; }
        public string? StudentTextAnswer { get; set; }
        public string? StudentConnectionTexts { get; set; }

        public string? CorrectChoiceText { get; set; }
        public bool? CorrectTrueFalseAnswer { get; set; }
        public string? CorrectTextAnswer { get; set; }
        public string? CorrectConnectionTexts { get; set; }

        public decimal? StudentMark { get; set; }
        public bool IsCorrect { get; set; }
    }
}
