namespace Application.DTOs.AppointmentsDTOs
{
    public class AppointmentDto
    {
        public string Id { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string ClassId { get; set; } = string.Empty;
        public string? Link { get; set; }
        public string Status { get; set; } = "Upcoming";
    }
}
