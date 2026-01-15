namespace Application.DTOs.TeacherDTOs
{
    public class BulkAssignTeachersDto
    {
        public List<string> ClassIds { get; set; } = new();
        public List<string> TeacherIds { get; set; } = new();
    }
}
