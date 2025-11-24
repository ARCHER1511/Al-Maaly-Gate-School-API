using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.DegreesDTOs
{
    public class AddDegreesDto
    {
        public string StudentId { get; set; } = string.Empty;
        public List<DegreeInput> Degrees { get; set; } = new();
    }
}
