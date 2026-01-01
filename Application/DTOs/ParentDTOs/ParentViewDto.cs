using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.ParentDTOs
{
    public class ParentViewDto
    {
        public string Id { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string ContactInfo { get; set; } = string.Empty;
        public string AppUserId { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string? Gender { get; set; }
        public AccountStatus AccountStatus { get; set; }
    }
}
