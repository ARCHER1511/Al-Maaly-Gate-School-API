using Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.AppointmentsDTOs
{
    public class ClassAppointmentDto
    {
        public string Id { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string? Link { get; set; }
        public string Status { get; set; } = "Upcoming";
        public string ClassId { get; set; } = string.Empty;
        public string TeacherId { get; set; } = string.Empty;
        public string SubjectId { get; set; } = string.Empty;
    }
}
