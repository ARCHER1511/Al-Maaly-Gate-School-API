namespace Domain.Entities
{
    public class Notification
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;                 // optional deep-link
        public string NotificationType { get; set; } = string.Empty;    // e.g., "TaskUpdated", "NewComment"
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string? CreatorUserId { get; set; } = string.Empty;    // who triggered it (optional)
        public AppUser? CreatorUser { get; set; }          // navigation property
        public string? Role { get; set; }               // e.g., "Admin", "Manager"
        public bool IsBroadcast { get; set; } = false;  // to all users

        public List<UserNotification> UserNotifications { get; set; } = new List<UserNotification>();
    }
}
