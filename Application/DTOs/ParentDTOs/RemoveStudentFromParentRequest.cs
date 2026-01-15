namespace Application.DTOs.ParentDTOs
{
    public class RemoveStudentFromParentRequest
    {
        public string ParentId { get; set; } = string.Empty;
        public string StudentId { get; set; } = string.Empty;
    }
}
