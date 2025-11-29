using Domain.Entities;

namespace Application.Interfaces
{
    public interface ICertificateService
    {
        Task<byte[]> GenerateCertificateForStudentAsync(string studentId, DegreeType degreeType, string templatePath);
        Task<Certificate> GenerateAndSaveCertificateAsync(string studentId, DegreeType degreeType, string templatePath);
        Task<Certificate?> GetCertificateAsync(string studentId, DegreeType degreeType);
    }
}
