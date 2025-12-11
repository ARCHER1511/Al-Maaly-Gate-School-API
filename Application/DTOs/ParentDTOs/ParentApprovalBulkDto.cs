using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.ParentDTOs
{
    public class ParentApprovalBulkDto
    {
        public string ParentId { get; set; } = string.Empty;
        public List<ParentStudentApprovalDto> StudentApprovals { get; set; } = new();
    }
}
