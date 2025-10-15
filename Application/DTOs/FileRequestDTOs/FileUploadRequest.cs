using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Application.DTOs.FileRequestDTOs
{
    public class FileUploadRequest
    {
        [FromForm]
        public IFormFile File { get; set; } = default!;
    }
}
