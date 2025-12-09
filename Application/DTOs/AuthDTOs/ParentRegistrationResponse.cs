using Application.DTOs.FileRequestDTOs;
using Application.DTOs.ParentDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.AuthDTOs
{
    public class ParentRegistrationResponse : AuthResponse
    {
        public List<DocumentInfo> UploadedDocuments { get; set; } = new();
        public ParentProfileDto ParentProfile { get; set; } = null!;
    }
}
