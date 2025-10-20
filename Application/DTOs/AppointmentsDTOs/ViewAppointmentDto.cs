namespace Application.DTOs.AppointmentsDTOs
{
    public class ViewAppointmentDto
    {
        public string? Id { get; set; }
        public DateTime? Appointment { get; set; }
        public string ClassId { get; set; } = string.Empty;
        public string? Link { get; set; }
    }
}
