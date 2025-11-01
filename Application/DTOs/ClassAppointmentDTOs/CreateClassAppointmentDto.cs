using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.ClassAppointmentDTOs
{
    public class CreateClassAppointmentDto
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string? Link { get; set; }
        public string ClassId { get; set; } = string.Empty;
        public string TeacherId { get; set; } = string.Empty;
        public string SubjectId { get; set; } = string.Empty;
    }
}
