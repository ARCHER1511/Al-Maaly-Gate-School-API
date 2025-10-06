namespace Application.DTOs.NotificationDTOs
{
    public class CreateNotificationDto
    {
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string CreatorUserId { get; set; } = string.Empty;
        public IEnumerable<string> TargetUserIds { get; set; } = new List<string>();
        public string Url { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public bool IsBroadcast { get; set; }
    }
}
