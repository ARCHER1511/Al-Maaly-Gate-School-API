using Application.DTOs.FileRequestDTOs;
using Application.Interfaces;
using Domain.Entities;
using Domain.Wrappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Al_Maaly_Gate_School.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class FileController : ControllerBase
    {
        private readonly IFileService _fileService;

        public FileController(IFileService fileService)
        {
            _fileService = fileService;
        }

        [HttpPost("upload/{controllerName}")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadFile([FromForm] FileUploadRequest request, [FromRoute] string controllerName)
        {
            var result = await _fileService.UploadFileAsync(request.File, controllerName);
            if (!result.Success)
                return BadRequest(ApiResponse<string>.Fail(result.Message!));

            return Ok(ApiResponse<string>.Ok(result.Data!, result.Message));
        }


        [HttpPost("upload-multiple/{controllerName}")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadFiles(MultipleFilesUploadRequest request, [FromRoute] string controllerName)
        {
            var result = await _fileService.UploadFilesAsync(request.Files, controllerName);
            if (!result.Success)
                return BadRequest(ApiResponse<List<string>>.Fail(result.Message!));

            return Ok(ApiResponse<List<string>>.Ok(result.Data!, result.Message));
        }


        [HttpPut("replace/{controllerName}")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> ReplaceFile([FromForm] FileUploadRequest newRequest, [FromQuery] string existingFilePath, [FromRoute] string controllerName)
        {
            var result = await _fileService.ReplaceFileAsync(newRequest.File, existingFilePath, controllerName);
            if (!result.Success)
                return BadRequest(ApiResponse<string>.Fail(result.Message!));

            return Ok(ApiResponse<string>.Ok(result.Data!, result.Message));
        }


        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteFile([FromQuery] string filePath)
        {
            var result = await _fileService.DeleteFileAsync(filePath);
            if (!result.Success)
                return BadRequest(ApiResponse<bool>.Fail(result.Message!));

            return Ok(ApiResponse<bool>.Ok(result.Data!, result.Message));
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var result = await _fileService.GetFileByIdAsync(id);
            if (!result.Success || result.Data == null)
                return NotFound(ApiResponse<FileRecord?>.Fail("File not found"));

            return Ok(ApiResponse<FileRecord?>.Ok(result.Data, result.Message));
        }


        [HttpGet("path")]
        public async Task<IActionResult> GetByPath([FromQuery] string relativePath)
        {
            var result = await _fileService.GetFileByPathAsync(relativePath);
            if (!result.Success || result.Data == null)
                return NotFound(ApiResponse<FileRecord?>.Fail("File not found"));

            return Ok(ApiResponse<FileRecord?>.Ok(result.Data, result.Message));
        }


        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _fileService.GetAllFilesAsync();
            return Ok(ApiResponse<IEnumerable<FileRecord>>.Ok(result.Data!, result.Message));
        }

        [HttpGet("controller/{controllerName}")]
        public async Task<IActionResult> GetByController(string controllerName)
        {
            var result = await _fileService.GetFilesByControllerAsync(controllerName);
            return Ok(ApiResponse<IEnumerable<FileRecord>>.Ok(result.Data!, result.Message));
        }

        [HttpGet("type/{fileType}")]
        public async Task<IActionResult> GetByType(string fileType)
        {
            var result = await _fileService.GetFilesByTypeAsync(fileType);
            return Ok(ApiResponse<IEnumerable<FileRecord>>.Ok(result.Data!, result.Message));
        }

        [HttpGet("recent")]
        public async Task<IActionResult> GetRecent([FromQuery] int days = 7)
        {
            var result = await _fileService.GetRecentFilesAsync(days);
            return Ok(ApiResponse<IEnumerable<FileRecord>>.Ok(result.Data!, result.Message));
        }

        [HttpGet("storage/total")]
        public async Task<IActionResult> GetTotalStorage()
        {
            var result = await _fileService.GetTotalStorageUsedAsync();
            return Ok(ApiResponse<long>.Ok(result.Data!, result.Message));
        }

        [HttpDelete("controller/{controllerName}")]
        public async Task<IActionResult> DeleteByController(string controllerName)
        {
            var result = await _fileService.DeleteFilesByControllerAsync(controllerName);
            if (!result.Success)
                return BadRequest(ApiResponse<int>.Fail(result.Message!));

            return Ok(ApiResponse<int>.Ok(result.Data!, result.Message));
        }

        [HttpGet("download")]
        public async Task<IActionResult> Download([FromQuery] string filePath)
        {
            var result = await _fileService.DownloadFileAsync(filePath);
            if (!result.Success)
                return NotFound(ApiResponse<string>.Fail(result.Message!));

            var (fileBytes, fileName, contentType) = result.Data!;
            return File(fileBytes, contentType, fileName);
        }
    }
}
