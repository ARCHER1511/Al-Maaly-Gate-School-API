using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.TeacherDTOs
{
    public class BulkAssignTeachersDto
    {
        public List<string> ClassIds { get; set; } = new();
        public List<string> TeacherIds { get; set; } = new();
    }
}
