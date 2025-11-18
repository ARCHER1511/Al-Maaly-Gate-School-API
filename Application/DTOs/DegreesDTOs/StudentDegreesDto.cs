using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.DegreesDTOs
{
    public class StudentDegreesDto
    {
        public string StudentId { get; set; }
        public string StudentName { get; set; }
        public string ClassId { get; set; }
        public string ClassName { get; set; }
        public List<DegreeItemDto> Degrees { get; set; } = new();
    }
}
