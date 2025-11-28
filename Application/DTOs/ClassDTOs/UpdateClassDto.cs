using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.ClassDTOs
{
    public class UpdateClassDto
    {
        public string Id { get; set; } = string.Empty;
        public string ClassName { get; set; } = string.Empty;
        public string GradeId { get; set; } = string.Empty;
    }
}
