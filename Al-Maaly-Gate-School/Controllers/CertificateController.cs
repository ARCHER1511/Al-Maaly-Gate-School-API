using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Al_Maaly_Gate_School.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class CertificateController : ControllerBase
    {
        private readonly ICertificateService _certificateService;

        public CertificateController(ICertificateService certificateService)
        {
            _certificateService = certificateService;
        }

        [HttpGet("{studentId}")]
        public async Task<IActionResult> Generate(string studentId)
        {
            try
            {
                var pdf = await _certificateService.GenerateCertificatePdfAsync(studentId);
                return File(pdf, "application/pdf", $"{studentId}_certificate.pdf");
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

    }
}
