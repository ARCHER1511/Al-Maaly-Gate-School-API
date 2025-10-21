using Domain.Entities;

namespace Application.DTOs.ClassDTOs
{
    public class ClassAppointmentsDTo
    {
        public DateTime? Appointment { get; set; }
        public string ClassId { get; set; } = string.Empty;
        public Class? Class { get; set; }
    }
}
