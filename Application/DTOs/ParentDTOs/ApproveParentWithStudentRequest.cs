using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.ParentDTOs
{
    public class RelationParentWithStudentRequest
    {
        public string ParentId { get; set; } = string.Empty;
        public string StudentId { get; set; } = string.Empty;
        public string Relation { get; set; } = string.Empty;
    }
}
