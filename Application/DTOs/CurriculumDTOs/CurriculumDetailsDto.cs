using Application.DTOs.GradeDTOs;
using Application.DTOs.StudentDTOs;
using Application.DTOs.TeacherDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.CurriculumDTOs
{
    public class CurriculumDetailsDto : CurriculumDto
    {
        public List<GradeViewDto> Grades { get; set; } = new();
        public List<StudentViewDto> Students { get; set; } = new();
        public List<TeacherViewDto> Teachers { get; set; } = new();
    }
}
