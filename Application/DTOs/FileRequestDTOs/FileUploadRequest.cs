using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Application.DTOs.FileRequestDTOs
{
    public class FileUploadRequest
    {
        [FromForm(Name = "File")]
        public IFormFile File { get; set; } = default!;
    }
}
