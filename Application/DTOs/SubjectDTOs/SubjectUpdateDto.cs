namespace Application.DTOs.SubjectDTOs
{
    public class SubjectUpdateDto
    {
        public string Id { get; set; } = string.Empty;
        public string SubjectName { get; set; } = string.Empty;
        public string GradeId { get; set; } = string.Empty; // CHANGED: From ClassYear to GradeId
        public double CreditHours { get; set; } = 3.0; // ADDED: Missing property
        // REMOVED: ClassId, TeacherId (these are in separate relationships)
    }
}
