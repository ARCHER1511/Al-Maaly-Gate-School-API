namespace Application.DTOs.FileRequestDTOs
{
    public class DocumentInfo
    {
        public string Id { get; set; } = null!;
        public string Path { get; set; } = null!;
        public string Type { get; set; } = null!;
        public string OriginalFileName { get; set; } = null!;
        public long FileSize { get; set; }
        public DateTime UploadedAt { get; set; }
    }
}
