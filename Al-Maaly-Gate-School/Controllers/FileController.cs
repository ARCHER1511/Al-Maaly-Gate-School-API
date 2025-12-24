using Application.DTOs.FileRequestDTOs;
using Application.Interfaces;
using Domain.Entities;
using Domain.Wrappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Al_Maaly_Gate_School.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // 🔒 User must be authenticated
    public class FileController : ControllerBase
    {
        private readonly IFileService _fileService;

        private string UserId =>
            User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        public FileController(IFileService fileService)
        {
            _fileService = fileService;
        }

        // ===================== UPLOAD =====================

        [HttpPost("upload/{controllerName}")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadFile(
            [FromForm] FileUploadRequest request,
            [FromRoute] string controllerName)
        {
            var result = await _fileService.UploadFileAsync(
                request.File,
                controllerName,
                UserId);

            if (!result.Success)
                return BadRequest(ApiResponse<string>.Fail(result.Message!));

            return Ok(ApiResponse<string>.Ok(result.Data!, result.Message));
        }

        [HttpPost("upload-multiple/{controllerName}")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadFiles(
            [FromForm] MultipleFilesUploadRequest request,
            [FromRoute] string controllerName)
        {
            var result = await _fileService.UploadFilesAsync(
                request.Files,
                controllerName,
                UserId);

            if (!result.Success)
                return BadRequest(ApiResponse<List<string>>.Fail(result.Message!));

            return Ok(ApiResponse<List<string>>.Ok(result.Data!, result.Message));
        }

        // ===================== READ =====================

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var result = await _fileService.GetFileByIdAsync(id, UserId);

            if (!result.Success || result.Data == null)
                return NotFound(ApiResponse<FileRecord?>.Fail(result.Message!));

            return Ok(ApiResponse<FileRecord?>.Ok(result.Data, result.Message));
        }

        [HttpGet("path")]
        public async Task<IActionResult> GetByPath([FromQuery] string relativePath)
        {
            var result = await _fileService.GetFileByPathAsync(
                relativePath,
                UserId);

            if (!result.Success || result.Data == null)
                return NotFound(ApiResponse<FileRecord?>.Fail(result.Message!));

            return Ok(ApiResponse<FileRecord?>.Ok(result.Data, result.Message));
        }

        [HttpGet("my-files")]
        public async Task<IActionResult> GetMyFiles()
        {
            var result = await _fileService.GetFilesByUserAsync(UserId);

            return Ok(ApiResponse<IEnumerable<FileRecord>>.Ok(
                result.Data!,
                result.Message));
        }

        // ===================== DELETE =====================

        [HttpDelete]
        public async Task<IActionResult> Delete([FromQuery] string filePath)
        {
            var result = await _fileService.DeleteFileAsync(filePath, UserId);

            if (!result.Success)
                return BadRequest(ApiResponse<bool>.Fail(result.Message!));

            return Ok(ApiResponse<bool>.Ok(result.Data!, result.Message));
        }

        // ===================== DOWNLOAD =====================

        [HttpGet("download")]
        public async Task<IActionResult> Download([FromQuery] string filePath)
        {
            var result = await _fileService.DownloadFileAsync(
                filePath,
                UserId);

            if (!result.Success)
                return NotFound(ApiResponse<string>.Fail(result.Message!));

            var (fileBytes, fileName, contentType) = result.Data!;
            return File(fileBytes, contentType, fileName);
        }

        // ===================== STORAGE =====================

        [HttpGet("storage/total")]
        public async Task<IActionResult> GetTotalStorage()
        {
            var result = await _fileService.GetTotalStorageUsedAsync(UserId);

            return Ok(ApiResponse<long>.Ok(result.Data!, result.Message));
        }
    }
}
