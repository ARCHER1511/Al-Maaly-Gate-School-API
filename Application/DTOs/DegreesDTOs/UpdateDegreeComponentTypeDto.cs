namespace Application.DTOs.DegreesDTOs
{
    public class UpdateDegreeComponentTypeDto
    {
        public string ComponentName { get; set; } = string.Empty;
        public int Order { get; set; } = 1;
        public double MaxScore { get; set; } = 0;
        public bool IsActive { get; set; } = true;
    }
}
