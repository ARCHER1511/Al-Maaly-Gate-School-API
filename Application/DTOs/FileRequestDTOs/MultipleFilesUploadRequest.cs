using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Application.DTOs.FileRequestDTOs
{
    public class MultipleFilesUploadRequest
    {
        [FromForm]
        public IEnumerable<IFormFile> Files { get; set; } = new List<IFormFile>();
    }
}
