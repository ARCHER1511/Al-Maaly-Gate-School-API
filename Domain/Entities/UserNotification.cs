namespace Domain.Entities
{
    public class UserNotification
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string NotificationId { get; set; } = string.Empty;
        public Notification Notification { get; set; } = null!;
        public string UserId { get; set; } = string.Empty;
        public AppUser? User { get; set; }
        public bool IsRead { get; set; } = false;
        public bool IsDelivered { get; set; } = false; // delivered to connected client at least once
        public DateTime? DeliveredAt { get; set; }
        public DateTime? ReadAt { get; set; }
    }
}
