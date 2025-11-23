using Application.Services;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface ICertificateService
    {
        Task<byte[]> GenerateCertificateForStudentAsync(string studentId, DegreeType degreeType, string templatePath);
        Task<Certificate> GenerateAndSaveCertificateAsync(string studentId, DegreeType degreeType, string templatePath);
        Task<Certificate?> GetCertificateAsync(string studentId, DegreeType degreeType);
    }
}
