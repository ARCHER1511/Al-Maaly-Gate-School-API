using Application.Interfaces;
using Domain.Entities;
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
        private readonly IUnitOfWork _unitOfWork; // Add this field

        public CertificateController(
            ICertificateService certificateService,
            IWebHostEnvironment env,
            IUnitOfWork unitOfWork) // Add this parameter
        {
            _certificateService = certificateService;
            _env = env;
            _unitOfWork = unitOfWork; // Initialize it
        }

        [HttpGet("{studentId}/{degreeType}")]
        public async Task<IActionResult> Generate(string studentId, string degreeType, [FromQuery] bool saveToDb = false)
        {
            try
            {
                if (!Enum.TryParse<DegreeType>(degreeType, true, out var typeEnum))
                    return BadRequest("Invalid certificate type. Allowed: MidTerm1, Final1, MidTerm2, Final2");

                string templatePath = typeEnum switch
                {
                    DegreeType.MidTerm1 => Path.Combine(_env.ContentRootPath, "Templates", "MidTerm1.pdf"),
                    DegreeType.Final1 => Path.Combine(_env.ContentRootPath, "Templates", "Final1.pdf"),
                    DegreeType.MidTerm2 => Path.Combine(_env.ContentRootPath, "Templates", "MidTerm2.pdf"),
                    DegreeType.Final2 => Path.Combine(_env.ContentRootPath, "Templates", "Final2.pdf"),
                    _ => throw new Exception("Unknown certificate type")
                };

                if (!System.IO.File.Exists(templatePath))
                    return NotFound($"Template not found: {templatePath}");

                byte[] pdfBytes;

                if (saveToDb)
                {
                    // Generate and save to database
                    var certificate = await _certificateService.GenerateAndSaveCertificateAsync(studentId, typeEnum, templatePath);
                    pdfBytes = certificate.PdfData;
                }
                else
                {
                    // Generate without saving to database (existing behavior)
                    pdfBytes = await _certificateService.GenerateCertificateForStudentAsync(studentId, typeEnum, templatePath);
                }

                string fileName = $"{studentId}_{degreeType}_certificate.pdf";
                return File(pdfBytes, "application/pdf", fileName);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{studentId}/{degreeType}/from-db")]
        public async Task<IActionResult> GetFromDatabase(string studentId, string degreeType)
        {
            try
            {
                if (!Enum.TryParse<DegreeType>(degreeType, true, out var typeEnum))
                    return BadRequest("Invalid certificate type. Allowed: MidTerm1, Final1, MidTerm2, Final2");

                var certificate = await _certificateService.GetCertificateAsync(studentId, typeEnum);

                if (certificate == null)
                    return NotFound("Certificate not found in database");

                return File(certificate.PdfData, "application/pdf", certificate.FileName);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("{studentId}/{degreeType}/save")]
        public async Task<IActionResult> SaveToDatabase(string studentId, string degreeType)
        {
            try
            {
                if (!Enum.TryParse<DegreeType>(degreeType, true, out var typeEnum))
                    return BadRequest("Invalid certificate type. Allowed: MidTerm1, Final1, MidTerm2, Final2");

                string templatePath = typeEnum switch
                {
                    DegreeType.MidTerm1 => Path.Combine(_env.ContentRootPath, "Templates", "MidTerm1.pdf"),
                    DegreeType.Final1 => Path.Combine(_env.ContentRootPath, "Templates", "Final1.pdf"),
                    DegreeType.MidTerm2 => Path.Combine(_env.ContentRootPath, "Templates", "MidTerm2.pdf"),
                    DegreeType.Final2 => Path.Combine(_env.ContentRootPath, "Templates", "Final2.pdf"),
                    _ => throw new Exception("Unknown certificate type")
                };

                if (!System.IO.File.Exists(templatePath))
                    return NotFound($"Template not found: {templatePath}");

                var certificate = await _certificateService.GenerateAndSaveCertificateAsync(studentId, typeEnum, templatePath);

                return Ok(new
                {
                    Message = "Certificate saved to database successfully",
                    CertificateId = certificate.Id,
                    StudentId = certificate.StudentId,
                    DegreeType = certificate.DegreeType,
                    GPA = certificate.GPA,
                    FileSize = certificate.FileSize,
                    IssuedDate = certificate.IssuedDate
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("student/{studentId}")]
        public async Task<IActionResult> GetStudentCertificates(string studentId)
        {
            try
            {
                var certificates = await _unitOfWork.Certificates
                    .AsQueryable() // This returns IQueryable<Certificate>
                    .Where(c => c.StudentId == studentId)
                    .OrderByDescending(c => c.IssuedDate)
                    .ToListAsync(); // You need to call ToListAsync() here

                return Ok(new
                {
                    Data = certificates,
                    Success = true,
                    Message = "Certificates retrieved successfully"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Data = new List<Certificate>(),
                    Success = false,
                    Message = ex.Message
                });
            }
        }
    }
}