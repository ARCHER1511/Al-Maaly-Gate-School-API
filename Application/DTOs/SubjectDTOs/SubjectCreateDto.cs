namespace Application.DTOs.SubjectDTOs
{
    public class SubjectCreateDto
    {
        public string SubjectName { get; set; } = string.Empty;
        public string GradeId { get; set; } = string.Empty; // ADDED: Grade reference
        public double CreditHours { get; set; } = 3.0;
    }
}
