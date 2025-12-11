using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class BulkProgress
    {
        public int Total { get; set; }
        public int Processed { get; set; }
        public string CurrentStudent { get; set; } = string.Empty;
        public int Percentage { get; set; }
    }
}
