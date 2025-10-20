namespace Application.DTOs.AppointmentsDTOs
{
    public class AppointmentDto
    {
        public string Id { get; set; } = string.Empty;
        public DateTime? Appointment { get; set; }
        public string ClassId { get; set; } = string.Empty;
        public string? Link { get; set; }
    }
}
