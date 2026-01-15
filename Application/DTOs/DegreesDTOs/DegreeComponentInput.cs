namespace Application.DTOs.DegreesDTOs
{
    public class DegreeComponentInput
    {
        public string ComponentTypeId { get; set; } = string.Empty;
        public string ComponentName { get; set; } = string.Empty;
        public double Score { get; set; }
        public double? MaxScore { get; set; } // Optional - will use ComponentType's MaxScore if null
    }
}
