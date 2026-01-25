namespace Application.DTOs.ParentDTOs
{
    public class ParentApprovalBulkDto
    {
        public string ParentId { get; set; } = string.Empty;
        public List<ParentStudentApprovalDto> StudentApprovals { get; set; } = new();
    }
}
