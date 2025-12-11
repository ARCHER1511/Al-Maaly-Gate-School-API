using Domain.Entities;
using Domain.Wrappers;

namespace Application.Interfaces
{
    public interface ICertificateService
    {
        Task<byte[]> GenerateCertificateForStudentAsync(string studentId, DegreeType degreeType, string templatePath);
        Task<ServiceResult<string>> GenerateAndSaveCertificateAsync(string studentId, DegreeType degreeType, string templatePath);
        Task<ServiceResult<string>> BulkGenerateForClassAsync(string classId, DegreeType degreeType, string? academicYear = null, int maxStudents = 50);

        Task<Certificate?> GetCertificateAsync(string studentId, DegreeType degreeType, string? academicYear = null);
        Task<List<Certificate>> GetCertificatesByCurriculumAsync(string curriculumId, string? academicYear = null);
        Task<List<Certificate>> GetCertificatesByGradeAsync(string gradeId, string? academicYear = null);
        Task<List<Certificate>> GetCertificatesByClassAsync(string classId, string? academicYear = null);

        string GetTemplatePath(DegreeType degreeType, string? curriculumCode = null, string templateRootPath = "Templates");
        Task<byte[]> DownloadCertificatesAsZipAsync(List<Certificate> certificates);

        Task<bool> VerifyCertificateAsync(string certificateId, string verifiedBy);
        Task<bool> ArchiveCertificateAsync(string certificateId);
        Task<Certificate?> GetCertificateByIdAsync(string certificateId);

        Task<List<Certificate>> SearchCertificatesAsync(
            string? studentName = null,
            string? certificateNumber = null,
            string? curriculumId = null,
            string? gradeId = null,
            string? classId = null,
            DegreeType? degreeType = null,
            string? academicYear = null,
            DateTime? fromDate = null,
            DateTime? toDate = null);
    }
}