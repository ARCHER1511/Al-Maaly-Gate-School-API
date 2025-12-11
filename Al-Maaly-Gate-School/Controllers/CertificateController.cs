using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Domain.Wrappers;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Al_Maaly_Gate_School.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class CertificateController : ControllerBase
    {
        private readonly ICertificateService _certificateService;
        private readonly IWebHostEnvironment _env;
        private readonly IUnitOfWork _unitOfWork;

        public CertificateController(
            ICertificateService certificateService,
            IWebHostEnvironment env,
            IUnitOfWork unitOfWork)
        {
            _certificateService = certificateService;
            _env = env;
            _unitOfWork = unitOfWork;
        }

        [HttpGet("{studentId}/{degreeType}")]
        public async Task<IActionResult> Generate(string studentId, string degreeType, [FromQuery] bool saveToDb = false)
        {
            try
            {
                if (!Enum.TryParse<DegreeType>(degreeType, true, out var typeEnum))
                    return BadRequest(ApiResponse<bool>.Fail("Invalid certificate type"));

                var student = await _unitOfWork.Students.FirstOrDefaultAsync(
                    s => s.Id == studentId,
                    include: q => q
                        .Include(s => s.Class)
                        .ThenInclude(c => c.Grade)
                        .ThenInclude(g => g.Curriculum)
                );

                if (student == null)
                    return NotFound(ApiResponse<bool>.Fail("Student not found"));

                var curriculumCode = student.Class?.Grade?.Curriculum?.Code;
                var templatePath = _certificateService.GetTemplatePath(typeEnum, curriculumCode);

                if (!System.IO.File.Exists(templatePath))
                    return NotFound(ApiResponse<bool>.Fail("Template not found"));

                if (saveToDb)
                {
                    var result = await _certificateService.GenerateAndSaveCertificateAsync(studentId, typeEnum, templatePath);
                    if (!result.Success)
                        return BadRequest(ApiResponse<bool>.Fail(result.Message!));

                    return Ok(ApiResponse<bool>.Ok(true, "Certificate generated and saved successfully"));
                }
                else
                {
                    var pdfBytes = await _certificateService.GenerateCertificateForStudentAsync(studentId, typeEnum, templatePath);
                    return File(pdfBytes, "application/pdf", $"{studentId}_{degreeType}_certificate.pdf");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<bool>.Fail(ex.Message));
            }
        }

        [HttpGet("{studentId}/{degreeType}/from-db")]
        public async Task<IActionResult> GetFromDatabase(string studentId, string degreeType)
        {
            try
            {
                if (!Enum.TryParse<DegreeType>(degreeType, true, out var typeEnum))
                    return BadRequest(ApiResponse<bool>.Fail("Invalid certificate type"));

                var certificate = await _certificateService.GetCertificateAsync(studentId, typeEnum);

                if (certificate == null || certificate.PdfData == null || certificate.PdfData.Length == 0)
                    return NotFound(ApiResponse<bool>.Fail("Certificate not found in database"));

                return File(certificate.PdfData, "application/pdf", certificate.FileName);
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<bool>.Fail(ex.Message));
            }
        }

        [HttpPost("{studentId}/{degreeType}/save")]
        public async Task<IActionResult> SaveToDatabase(string studentId, string degreeType)
        {
            try
            {
                if (!Enum.TryParse<DegreeType>(degreeType, true, out var typeEnum))
                    return BadRequest(ApiResponse<bool>.Fail("Invalid certificate type"));

                var student = await _unitOfWork.Students.FirstOrDefaultAsync(
                    s => s.Id == studentId,
                    include: q => q
                        .Include(s => s.Class)
                        .ThenInclude(c => c.Grade)
                        .ThenInclude(g => g.Curriculum)
                );

                if (student == null)
                    return NotFound(ApiResponse<bool>.Fail("Student not found"));

                var curriculumCode = student.Class?.Grade?.Curriculum?.Code;
                var templatePath = _certificateService.GetTemplatePath(typeEnum, curriculumCode);

                if (!System.IO.File.Exists(templatePath))
                    return NotFound(ApiResponse<bool>.Fail("Template not found"));

                var result = await _certificateService.GenerateAndSaveCertificateAsync(studentId, typeEnum, templatePath);

                if (!result.Success)
                    return BadRequest(ApiResponse<bool>.Fail(result.Message!));

                return Ok(ApiResponse<bool>.Ok(true, "Certificate saved to database successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<bool>.Fail(ex.Message));
            }
        }

        [HttpGet("student/{studentId}")]
        public async Task<IActionResult> GetStudentCertificates(string studentId)
        {
            try
            {
                var certificates = await _unitOfWork.Certificates
                    .AsQueryable()
                    .Where(c => c.StudentId == studentId)
                    .OrderByDescending(c => c.IssuedDate)
                    .ToListAsync();

                return Ok(ApiResponse<List<Certificate>>.Ok(certificates, "Certificates retrieved successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<List<Certificate>>.Fail(ex.Message));
            }
        }

        [HttpPost("bulk/class/{classId}/{degreeType}")]
        public async Task<IActionResult> BulkGenerateForClass(
            string classId,
            string degreeType,
            [FromQuery] string? academicYear = null)
        {
            try
            {
                if (!Enum.TryParse<DegreeType>(degreeType, true, out var typeEnum))
                    return BadRequest(ApiResponse<bool>.Fail("Invalid certificate type"));

                var result = await _certificateService.BulkGenerateForClassAsync(
                    classId, typeEnum, academicYear);

                if (!result.Success)
                    return BadRequest(ApiResponse<bool>.Fail(result.Message!));

                return Ok(ApiResponse<bool>.Ok(true, result.Message!));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<bool>.Fail(ex.Message));
            }
        }

        [HttpGet("curriculum/{curriculumId}")]
        public async Task<IActionResult> GetByCurriculum(string curriculumId, [FromQuery] string? academicYear = null)
        {
            try
            {
                var certificates = await _certificateService.GetCertificatesByCurriculumAsync(curriculumId, academicYear);
                return Ok(ApiResponse<List<Certificate>>.Ok(certificates, $"Found {certificates.Count} certificates"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<List<Certificate>>.Fail(ex.Message));
            }
        }

        [HttpGet("grade/{gradeId}")]
        public async Task<IActionResult> GetByGrade(string gradeId, [FromQuery] string? academicYear = null)
        {
            try
            {
                var certificates = await _certificateService.GetCertificatesByGradeAsync(gradeId, academicYear);
                return Ok(ApiResponse<List<Certificate>>.Ok(certificates, $"Found {certificates.Count} certificates"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<List<Certificate>>.Fail(ex.Message));
            }
        }

        [HttpGet("class/{classId}")]
        public async Task<IActionResult> GetByClass(string classId, [FromQuery] string? academicYear = null)
        {
            try
            {
                var certificates = await _certificateService.GetCertificatesByClassAsync(classId, academicYear);
                return Ok(ApiResponse<List<Certificate>>.Ok(certificates, $"Found {certificates.Count} certificates"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<List<Certificate>>.Fail(ex.Message));
            }
        }

        [HttpGet("bulk/class/{classId}/{degreeType}/download")]
        public async Task<IActionResult> DownloadBulkForClass(string classId, string degreeType, [FromQuery] string? academicYear = null)
        {
            try
            {
                if (!Enum.TryParse<DegreeType>(degreeType, true, out var typeEnum))
                    return BadRequest(ApiResponse<bool>.Fail("Invalid certificate type"));

                var certificates = await _certificateService.GetCertificatesByClassAsync(classId, academicYear);
                certificates = certificates.Where(c => c.DegreeType == typeEnum).ToList();

                if (!certificates.Any())
                    return NotFound(ApiResponse<bool>.Fail("No certificates found"));

                var zipBytes = await _certificateService.DownloadCertificatesAsZipAsync(certificates);
                return File(zipBytes, "application/zip", $"certificates_class_{classId}_{degreeType}.zip");
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<bool>.Fail(ex.Message));
            }
        }

        [HttpPost("verify/{certificateId}")]
        public async Task<IActionResult> VerifyCertificate(string certificateId, [FromBody] VerifyCertificateRequest request)
        {
            try
            {
                var result = await _certificateService.VerifyCertificateAsync(certificateId, request.VerifiedBy);

                if (!result)
                    return NotFound(ApiResponse<bool>.Fail("Certificate not found"));

                return Ok(ApiResponse<bool>.Ok(true, "Certificate verified successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<bool>.Fail(ex.Message));
            }
        }

        [HttpPost("archive/{certificateId}")]
        public async Task<IActionResult> ArchiveCertificate(string certificateId)
        {
            try
            {
                var result = await _certificateService.ArchiveCertificateAsync(certificateId);

                if (!result)
                    return NotFound(ApiResponse<bool>.Fail("Certificate not found"));

                return Ok(ApiResponse<bool>.Ok(true, "Certificate archived successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<bool>.Fail(ex.Message));
            }
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchCertificates(
            [FromQuery] string? studentName = null,
            [FromQuery] string? certificateNumber = null,
            [FromQuery] string? curriculumId = null,
            [FromQuery] string? gradeId = null,
            [FromQuery] string? classId = null,
            [FromQuery] DegreeType? degreeType = null,
            [FromQuery] string? academicYear = null,
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null)
        {
            try
            {
                var certificates = await _certificateService.SearchCertificatesAsync(
                    studentName, certificateNumber, curriculumId, gradeId, classId,
                    degreeType, academicYear, fromDate, toDate);

                return Ok(ApiResponse<List<Certificate>>.Ok(certificates, $"Found {certificates.Count} certificates"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<List<Certificate>>.Fail(ex.Message));
            }
        }
    }

    public class VerifyCertificateRequest
    {
        public string VerifiedBy { get; set; } = string.Empty;
    }
}