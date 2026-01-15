namespace Application.DTOs
{
    public class BulkProgress
    {
        public int Total { get; set; }
        public int Processed { get; set; }
        public string CurrentStudent { get; set; } = string.Empty;
        public int Percentage { get; set; }
    }
}
