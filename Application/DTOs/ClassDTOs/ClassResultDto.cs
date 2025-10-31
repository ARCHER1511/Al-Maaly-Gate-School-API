
namespace Application.DTOs.ClassDTOs
{
    public class ClassResultDto
    {
        public string ClassId { get; set; } = string.Empty;
        public string ClassName { get; set; } = string.Empty;
        public double AverageMark { get; set; }
        public int StudentCount { get; set; }
        public int ExamCount { get; set; }
    }
}