using Application.DTOs.ClassDTOs;
using Application.DTOs.CurriculumDTOs;
using Application.DTOs.SubjectDTOs;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.TeacherDTOs
{
    public class TeacherDetailsDto
    {
        public string Id { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string ContactInfo { get; set; } = string.Empty;
        public string AppUserId { get; set; } = string.Empty;
        public ProfileStatus ProfileStatus { get; set; }
        public List<string> Subjects { get; set; } = new();
        public List<string> ClassNames { get; set; } = new();
        public List<CurriculumDto> SpecializedCurricula { get; set; } = new(); // Full curriculum objects
        public List<ClassDto> AssignedClasses { get; set; } = new();
        public List<SubjectViewDto> AssignedSubjects { get; set; } = new();
    }
}
