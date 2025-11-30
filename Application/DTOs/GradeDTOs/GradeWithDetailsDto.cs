using Application.DTOs.ClassDTOs;
using Application.DTOs.SubjectDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.GradeDTOs
{
    public class GradeWithDetailsDto
    {
        public string Id { get; set; } = string.Empty;
        public string GradeName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<ClassViewDto> Classes { get; set; } = new();
        public List<SubjectViewDto> Subjects { get; set; } = new();
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
