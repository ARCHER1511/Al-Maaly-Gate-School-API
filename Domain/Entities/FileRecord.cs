namespace Domain.Entities
{
    public class FileRecord : BaseEntity
    {
        public string FileName { get; set; } = string.Empty;
        public string RelativePath { get; set; } = string.Empty;
        public string ControllerName { get; set; } = string.Empty;
        public string FileType { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public DateTime UploadedAt { get; set; } = DateTime.Now;
        public string UserId { get; set; } = string.Empty;
        public AppUser User { get; set; } = null!;
    }
}
