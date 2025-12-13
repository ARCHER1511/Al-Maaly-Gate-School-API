using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.ParentDTOs
{
    public class SearchTermDto
    {
        public string SearchTerm { get; set; } = string.Empty;
        public string ParentId { get; set; } = string.Empty;
    }
}
