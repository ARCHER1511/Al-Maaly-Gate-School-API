using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.ParentDTOs
{
    public class RemoveStudentFromParentRequest
    {
        public string ParentId { get; set; } = string.Empty;
        public string StudentId { get; set; } = string.Empty;
    }
}
