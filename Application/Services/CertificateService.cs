using Application.DTOs;
using Application.Helpers;
using Application.Interfaces;
using Domain.Entities;
using Domain.Wrappers;
using Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PdfSharp.Drawing;
using PdfSharp.Fonts;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using System.Diagnostics;
using System.IO.Compression;

namespace Application.Services
{
    public class CertificateService : ICertificateService
    {
        private readonly IGpaCalculator _gpaCalculator;
        private readonly IWebHostEnvironment _env;
        private readonly IServiceProvider _serviceProvider;

        public CertificateService(
            IGpaCalculator gpaCalculator,
            IWebHostEnvironment env,
            IServiceProvider serviceProvider)
        {
            _gpaCalculator = gpaCalculator;
            _env = env;
            _serviceProvider = serviceProvider;

            //if (GlobalFontSettings.FontResolver == null)
            //{
            //    GlobalFontSettings.FontResolver = new Application.Helpers.FontResolver();
            //}

            if (GlobalFontSettings.FontResolver == null)
            {
                GlobalFontSettings.FontResolver = new FontResolver();
            }
        }

        public async Task<byte[]> GenerateCertificateForStudentAsync(string studentId, DegreeType degreeType, string templatePath)
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AlMaalyGateSchoolContext>();
            var student = await context.Students
                .Include(s => s.Degrees)
                    .ThenInclude(d => d.Subject)
                .Include(s => s.Class)
                    .ThenInclude(c => c.Grade)
                    .ThenInclude(g => g.Curriculum)
                .FirstOrDefaultAsync(s => s.Id == studentId);

            if (student == null)
                throw new KeyNotFoundException("Student not found");

            return await GeneratePdfForStudentAsync(student, degreeType, templatePath);
        }

        public async Task<ServiceResult<string>> GenerateAndSaveCertificateAsync(string studentId, DegreeType degreeType, string templatePath)
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AlMaalyGateSchoolContext>();

            try
            {
                var student = await context.Students
                    .Include(s => s.Class)
                        .ThenInclude(c => c.Grade)
                        .ThenInclude(g => g.Curriculum)
                    .Include(s => s.Degrees.Where(d =>
                        d.DegreeType == degreeType ||
                        (degreeType == DegreeType.Final1 && (d.DegreeType == DegreeType.MidTerm1 || d.DegreeType == DegreeType.Final1)) ||
                        (degreeType == DegreeType.Final2 && (d.DegreeType == DegreeType.MidTerm1 || d.DegreeType == DegreeType.Final1 || d.DegreeType == DegreeType.MidTerm2 || d.DegreeType == DegreeType.Final2))))
                        .ThenInclude(d => d.Subject)
                    .FirstOrDefaultAsync(s => s.Id == studentId);

                if (student == null)
                    return ServiceResult<string>.Fail("Student not found");

                if (!student.Degrees.Any())
                    return ServiceResult<string>.Fail("Student has no degrees");

                // Check if certificate already exists for this academic year
                var existingCertificate = await context.Certificates
                    .FirstOrDefaultAsync(c => c.StudentId == studentId &&
                                             c.DegreeType == degreeType &&
                                             c.AcademicYear == student.ClassYear);

                if (existingCertificate != null)
                {
                    // Delete existing certificate
                    context.Certificates.Remove(existingCertificate);
                    await context.SaveChangesAsync();
                }

                // Generate PDF - FIXED: Ensure PDF is generated BEFORE creating certificate
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Generating PDF for student {studentId}");
                var pdfBytes = await GeneratePdfForStudentAsync(student, degreeType, templatePath);
                var gpa = _gpaCalculator.CalculateGpa(student.Degrees.ToList());

                // Validate PDF before saving
                if (pdfBytes == null || pdfBytes.Length == 0)
                {
                    return ServiceResult<string>.Fail("Failed to generate PDF (empty result)");
                }

                // Generate certificate number
                var year = DateTime.Now.Year;
                var month = DateTime.Now.Month;
                var curriculumCode = student.Class?.Grade?.Curriculum?.Code ?? "GEN";
                var studentIdShort = student.Id.Substring(0, Math.Min(8, student.Id.Length));

                var count = await context.Certificates
                    .CountAsync(c => c.StudentId == student.Id && c.IssuedDate.Year == year);

                var certificateNumber = $"{curriculumCode}-{year}{month:D2}-{studentIdShort}-{degreeType}-{count + 1:D4}";

                // Create certificate
                var certificate = new Certificate
                {
                    StudentId = studentId,
                    DegreeType = degreeType,
                    GPA = gpa,
                    CurriculumId = student.CurriculumId ?? string.Empty,
                    GradeId = student.Class?.GradeId ?? string.Empty,
                    ClassId = student.ClassId ?? string.Empty,
                    PdfData = pdfBytes,
                    FileName = $"{student.FullName}_{degreeType}_{DateTime.Now:yyyyMMdd}.pdf",
                    ContentType = "application/pdf",
                    FileSize = pdfBytes.Length,
                    TemplateName = Path.GetFileNameWithoutExtension(templatePath),
                    IssuedDate = DateTime.Now,
                    CertificateNumber = certificateNumber,
                    AcademicYear = student.ClassYear ?? "Unknown",
                    IsVerified = false,
                    IsArchived = false
                };

                await context.Certificates.AddAsync(certificate);
                await context.SaveChangesAsync();

                return ServiceResult<string>.Ok("Certificate generated and saved successfully");
            }
            catch (Exception ex)
            {
                return ServiceResult<string>.Fail($"Failed to generate certificate: {ex.Message}");
            }
        }

        public async Task<ServiceResult<string>> BulkGenerateForClassAsync(
            string classId,
            DegreeType degreeType,
            string? academicYear = null,
            int maxStudents = 50)
        {
            var stopwatch = Stopwatch.StartNew();
            var certificatesGenerated = 0;
            var errors = new List<string>();

            try
            {
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Starting bulk generation for class: {classId}, degree: {degreeType}");

                // Get all student IDs - FIXED: Use more flexible query
                var studentIds = await GetStudentIdsForClassAsync(classId, academicYear, maxStudents);

                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Found {studentIds.Count} students in class {classId}");

                if (!studentIds.Any())
                {
                    // Try to get the class info to see if class exists
                    using var scope = _serviceProvider.CreateScope();
                    var context = scope.ServiceProvider.GetRequiredService<AlMaalyGateSchoolContext>();
                    var classExists = await context.Classes.AnyAsync(c => c.Id == classId);

                    if (!classExists)
                        return ServiceResult<string>.Fail($"Class with ID {classId} not found");

                    return ServiceResult<string>.Fail("No active students found in this class");
                }

                // Get template path for the class
                var templatePath = await GetTemplatePathForClassAsync(classId, degreeType);
                if (string.IsNullOrEmpty(templatePath))
                    return ServiceResult<string>.Fail("Template not found for this class");

                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Using template: {templatePath}");

                // Process students one by one
                for (int i = 0; i < studentIds.Count; i++)
                {
                    var studentId = studentIds[i];
                    try
                    {
                        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Processing student {i + 1}/{studentIds.Count}: {studentId}");

                        using var scope = _serviceProvider.CreateScope();
                        var context = scope.ServiceProvider.GetRequiredService<AlMaalyGateSchoolContext>();

                        var result = await GenerateAndSaveCertificateForStudentAsync(
                            context, studentId, degreeType, templatePath, academicYear);

                        if (result.Success)
                        {
                            certificatesGenerated++;
                            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] ✓ Certificate generated for student {studentId}");
                        }
                        else
                        {
                            errors.Add($"Student {studentId}: {result.Message}");
                            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] ✗ Error for student {studentId}: {result.Message}");
                        }
                    }
                    catch (Exception ex)
                    {
                        errors.Add($"Student {studentId}: {ex.Message}");
                        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] ✗ Exception for student {studentId}: {ex.Message}");
                    }

                    // Small delay to prevent resource exhaustion
                    await Task.Delay(100);
                }

                stopwatch.Stop();

                if (certificatesGenerated > 0)
                {
                    var message = $"Generated {certificatesGenerated} certificates in {stopwatch.Elapsed.TotalSeconds:F2}s";
                    if (errors.Any())
                        message += $". {errors.Count} errors occurred";

                    return ServiceResult<string>.Ok(message);
                }
                else
                {
                    return ServiceResult<string>.Fail(
                        errors.Any()
                        ? string.Join("; ", errors.Take(3))
                        : "No certificates generated - all students may not have degrees");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] FATAL ERROR: {ex.Message}");
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Stack trace: {ex.StackTrace}");
                return ServiceResult<string>.Fail($"Bulk generation failed: {ex.Message}");
            }
        }

        private async Task<List<string>> GetStudentIdsForClassAsync(string classId, string? academicYear, int maxStudents)
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AlMaalyGateSchoolContext>();

            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Getting students for class {classId}, academicYear: {academicYear ?? "not specified"}");

            // First, check if class exists
            var classExists = await context.Classes.AnyAsync(c => c.Id == classId);
            if (!classExists)
            {
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Class {classId} does not exist in database");
                return new List<string>();
            }

            // Get all students in the class
            var query = context.Students.Where(s => s.ClassId == classId);

            // If academic year is provided, try to match it
            if (!string.IsNullOrEmpty(academicYear))
            {
                // Try exact match first
                var exactMatchStudents = await query
                    .Where(s => s.ClassYear == academicYear)
                    .Select(s => s.Id)
                    .Take(maxStudents)
                    .ToListAsync();

                if (exactMatchStudents.Any())
                {
                    Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Found {exactMatchStudents.Count} students with exact academic year match: {academicYear}");
                    return exactMatchStudents;
                }

                // If no exact match, try students with null or empty academic year
                var studentsWithNoYear = await query
                    .Where(s => string.IsNullOrEmpty(s.ClassYear))
                    .Select(s => s.Id)
                    .Take(maxStudents)
                    .ToListAsync();

                if (studentsWithNoYear.Any())
                {
                    Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Found {studentsWithNoYear.Count} students without academic year specified");
                    return studentsWithNoYear;
                }

                // Return all students in class regardless of academic year
                var allStudents = await query
                    .Select(s => s.Id)
                    .Take(maxStudents)
                    .ToListAsync();

                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] No students with academic year {academicYear}, returning all {allStudents.Count} students in class");
                return allStudents;
            }
            else
            {
                // No academic year specified, return all students in class
                var allStudents = await query
                    .Select(s => s.Id)
                    .Take(maxStudents)
                    .ToListAsync();

                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] No academic year specified, returning all {allStudents.Count} students in class");
                return allStudents;
            }
        }

        private async Task<string?> GetTemplatePathForClassAsync(string classId, DegreeType degreeType)
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AlMaalyGateSchoolContext>();

            try
            {
                // First try to get curriculum from class directly
                var classWithCurriculum = await context.Classes
                    .Include(c => c.Grade)
                    .ThenInclude(g => g.Curriculum)
                    .FirstOrDefaultAsync(c => c.Id == classId);

                if (classWithCurriculum?.Grade?.Curriculum != null)
                {
                    var curriculumCode = classWithCurriculum.Grade.Curriculum.Code;
                    Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Found curriculum from class: {curriculumCode}");
                    return GetTemplatePath(degreeType, curriculumCode);
                }

                // If no curriculum found, try to get from any student in the class
                var student = await context.Students
                    .Include(s => s.Class)
                        .ThenInclude(c => c.Grade)
                        .ThenInclude(g => g.Curriculum)
                    .FirstOrDefaultAsync(s => s.ClassId == classId);

                if (student?.Class?.Grade?.Curriculum != null)
                {
                    var curriculumCode = student.Class.Grade.Curriculum.Code;
                    Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Found curriculum from student: {curriculumCode}");
                    return GetTemplatePath(degreeType, curriculumCode);
                }

                // Fallback to default template
                var defaultPath = GetTemplatePath(degreeType, null);
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Using default template: {defaultPath}");
                return defaultPath;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Error getting template path: {ex.Message}");
                return GetTemplatePath(degreeType, null);
            }
        }

        private async Task<ServiceResult<string>> GenerateAndSaveCertificateForStudentAsync(
            AlMaalyGateSchoolContext context,
            string studentId,
            DegreeType degreeType,
            string templatePath,
            string? academicYear = null)
        {
            try
            {
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Loading student data for {studentId}");

                var student = await context.Students
                    .Include(s => s.Class)
                        .ThenInclude(c => c.Grade)
                        .ThenInclude(g => g.Curriculum)
                    .Include(s => s.Degrees.Where(d =>
                        d.DegreeType == degreeType ||
                        (degreeType == DegreeType.Final1 && (d.DegreeType == DegreeType.MidTerm1 || d.DegreeType == DegreeType.Final1)) ||
                        (degreeType == DegreeType.Final2 && (d.DegreeType == DegreeType.MidTerm1 || d.DegreeType == DegreeType.Final1 || d.DegreeType == DegreeType.MidTerm2 || d.DegreeType == DegreeType.Final2))))
                        .ThenInclude(d => d.Subject)
                    .FirstOrDefaultAsync(s => s.Id == studentId);

                if (student == null)
                {
                    Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Student {studentId} not found in database");
                    return ServiceResult<string>.Fail("Student not found");
                }

                if (!student.Degrees.Any())
                {
                    Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Student {studentId} has no degrees for {degreeType}");
                    return ServiceResult<string>.Fail("Student has no degrees");
                }

                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Student {studentId} has {student.Degrees.Count} degrees");

                // Determine academic year to use
                var targetAcademicYear = academicYear ?? student.ClassYear;
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Using academic year: {targetAcademicYear ?? "Unknown"}");

                // Check if certificate already exists for this academic year
                var existingCertificate = await context.Certificates
                    .FirstOrDefaultAsync(c => c.StudentId == studentId &&
                                             c.DegreeType == degreeType &&
                                             (string.IsNullOrEmpty(targetAcademicYear) || c.AcademicYear == targetAcademicYear));

                if (existingCertificate != null)
                {
                    Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Deleting existing certificate for student {studentId}");
                    // Delete existing certificate
                    context.Certificates.Remove(existingCertificate);
                    await context.SaveChangesAsync();
                }

                // Generate PDF - FIXED: Ensure this completes BEFORE creating certificate
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Generating PDF for student {studentId}");
                var pdfBytes = await GeneratePdfForStudentAsync(student, degreeType, templatePath);

                // Validate PDF before proceeding
                if (pdfBytes == null || pdfBytes.Length == 0)
                {
                    Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] PDF generation returned empty data for student {studentId}");
                    return ServiceResult<string>.Fail("Failed to generate PDF (empty result)");
                }

                // Validate it's actually a PDF
                if (pdfBytes.Length < 5 || System.Text.Encoding.ASCII.GetString(pdfBytes, 0, 5) != "%PDF-")
                {
                    Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Generated data is not a valid PDF for student {studentId}");
                    return ServiceResult<string>.Fail("Generated data is not a valid PDF");
                }

                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] PDF generated successfully: {pdfBytes.Length} bytes");

                var gpa = _gpaCalculator.CalculateGpa(student.Degrees.ToList());
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] GPA calculated: {gpa:F2}");

                // Generate certificate number
                var year = DateTime.Now.Year;
                var month = DateTime.Now.Month;
                var curriculumCode = student.Class?.Grade?.Curriculum?.Code ?? "GEN";
                var studentIdShort = student.Id.Substring(0, Math.Min(8, student.Id.Length));

                var count = await context.Certificates
                    .CountAsync(c => c.StudentId == student.Id && c.IssuedDate.Year == year);

                var certificateNumber = $"{curriculumCode}-{year}{month:D2}-{studentIdShort}-{degreeType}-{count + 1:D4}";

                // Create certificate - FIXED: Only create AFTER PDF is successfully generated
                var certificate = new Certificate
                {
                    StudentId = studentId,
                    DegreeType = degreeType,
                    GPA = gpa,
                    CurriculumId = student.CurriculumId ?? string.Empty,
                    GradeId = student.Class?.GradeId ?? string.Empty,
                    ClassId = student.ClassId ?? string.Empty,
                    PdfData = pdfBytes,
                    FileName = $"{student.FullName}_{degreeType}_{DateTime.Now:yyyyMMdd}.pdf",
                    ContentType = "application/pdf",
                    FileSize = pdfBytes.Length,
                    TemplateName = Path.GetFileNameWithoutExtension(templatePath),
                    IssuedDate = DateTime.Now,
                    CertificateNumber = certificateNumber,
                    AcademicYear = targetAcademicYear ?? "Unknown",
                    IsVerified = false,
                    IsArchived = false
                };

                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Saving certificate to database for student {studentId}");
                await context.Certificates.AddAsync(certificate);
                await context.SaveChangesAsync();

                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] ✓ Certificate saved successfully for student {studentId}");
                return ServiceResult<string>.Ok("Certificate generated");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] ERROR generating certificate for {studentId}: {ex.Message}");
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Stack trace: {ex.StackTrace}");
                return ServiceResult<string>.Fail($"Failed to generate certificate: {ex.Message}");
            }
        }

        private async Task<byte[]> GeneratePdfForStudentAsync(
            Student student,
            DegreeType degreeType,
            string templatePath)
        {
            // Validate template exists
            if (!System.IO.File.Exists(templatePath))
            {
                throw new FileNotFoundException($"Template file not found: {templatePath}");
            }

            try
            {
                // Shape Arabic text
                student.FullName = ArabicHelper.ShapeArabic(student.FullName ?? "");
                student.Nationality = ArabicHelper.ShapeArabic(student.Nationality ?? "");
                student.PassportNumber = ArabicHelper.ShapeArabic(student.PassportNumber ?? "");
                student.IqamaNumber = ArabicHelper.ShapeArabic(student.IqamaNumber ?? "");

                if (student.Class?.Grade != null)
                    student.Class.Grade.GradeName = ArabicHelper.ShapeArabic(student.Class.Grade.GradeName);

                var allDegrees = student.Degrees.ToList();
                List<Degree> degreesToDisplay;

                if (degreeType == DegreeType.Final1 || degreeType == DegreeType.Final2)
                {
                    degreesToDisplay = CalculateCumulativeDegrees(allDegrees, degreeType);
                }
                else
                {
                    degreesToDisplay = degreeType switch
                    {
                        DegreeType.MidTerm1 => allDegrees.Where(d => d.DegreeType == DegreeType.MidTerm1).ToList(),
                        DegreeType.MidTerm2 => allDegrees.Where(d => d.DegreeType == DegreeType.MidTerm2).ToList(),
                        _ => allDegrees
                    };
                }

                // Shape subject names
                foreach (var d in degreesToDisplay)
                {
                    if (d.Subject != null)
                        d.Subject.SubjectName = ArabicHelper.ShapeArabic(d.Subject.SubjectName);
                    d.SubjectName = ArabicHelper.ShapeArabic(d.SubjectName ?? "");
                }

                // PDF Generation
                using var template = PdfReader.Open(templatePath, PdfDocumentOpenMode.Modify);
                var page = template.Pages[0];
                var gfx = XGraphics.FromPdfPage(page);

                // Fix page rotation
                switch (page.Rotate)
                {
                    case 90: gfx.RotateTransform(-90); gfx.TranslateTransform(-page.Height, 0); break;
                    case 180: gfx.RotateTransform(-180); gfx.TranslateTransform(-page.Width, -page.Height); break;
                    case 270: gfx.RotateTransform(-270); gfx.TranslateTransform(0, -page.Width); break;
                }
                page.Rotate = 0;

                //XFont GetAppropriateFont(string text, double size, XFontStyleEx style)
                //{
                //    if (FontResolver.ContainsArabic(text))
                //    {
                //        text = ArabicHelper.ShapeArabic(text);
                //        return new XFont("ArabicFont", size, style);
                //    }
                //    else
                //    {
                //        return new XFont("MyFont", size, style);
                //    }
                //}

                XFont GetAppropriateFont(string familyName, double size, XFontStyleEx style)
                {
                    string[] fontCandidates = null;

                    if (familyName.Equals("Arial", StringComparison.OrdinalIgnoreCase) ||
                        familyName.Equals("Helvetica", StringComparison.OrdinalIgnoreCase))
                    {
                        fontCandidates = new[]
                        {
                    "Liberation Sans",
                    "DejaVu Sans",
                    "Nimbus Sans",
                    "FreeSans",
                    "Ubuntu",
                    "Noto Sans",
                    "Arial"
                };
                    }
                    else if (familyName.Equals("Times New Roman", StringComparison.OrdinalIgnoreCase) ||
                             familyName.Equals("Times", StringComparison.OrdinalIgnoreCase))
                    {
                        fontCandidates = new[]
                        {
                    "Liberation Serif",
                    "DejaVu Serif",
                    "Nimbus Roman",
                    "FreeSerif",
                    "Noto Serif",
                    "Times New Roman"
                };
                    }
                    else if (familyName.Equals("Courier New", StringComparison.OrdinalIgnoreCase) ||
                             familyName.Equals("Courier", StringComparison.OrdinalIgnoreCase))
                    {
                        fontCandidates = new[]
                        {
                    "Liberation Mono",
                    "DejaVu Sans Mono",
                    "Nimbus Mono",
                    "FreeMono",
                    "Noto Mono",
                    "Courier New"
                };
                    }

                    // Try each candidate
                    if (fontCandidates != null)
                    {
                        foreach (var fontCandidate in fontCandidates)
                        {
                            try
                            {
                                return new XFont(fontCandidate, size, style);
                            }
                            catch
                            {
                                continue; // Try next candidate
                            }
                        }
                    }

                    // Last resort: try the original font
                    try
                    {
                        return new XFont(familyName, size, style);
                    }
                    catch (Exception ex)
                    {
                        // Ultimate fallback
                        return new XFont("DejaVu Sans", size, style);
                    }
                }

                var darkBlueBrush = new XSolidBrush(XColor.FromArgb(44, 62, 80));
                var mediumGrayBrush = new XSolidBrush(XColor.FromArgb(108, 117, 125));
                var lightGrayBrush = new XSolidBrush(XColor.FromArgb(248, 249, 250));

                //var headerFont = new XFont("MyFont", 11, XFontStyleEx.Bold);
                //var regularFont = new XFont("MyFont", 10, XFontStyleEx.Regular);

                var headerFont = GetAppropriateFont("HEADER", 11, XFontStyleEx.Bold);
                var regularFont = GetAppropriateFont("TEXT", 10, XFontStyleEx.Regular);


                double pageWidth = page.Width;
                double pageHeight = page.Height;
                double tableWidth = 480;
                double marginX = (pageWidth - tableWidth) / 2 + 100;
                double startY = 180;
                double rowHeight = 22;
                double currentY = startY;

                // Add curriculum header
                var curriculumName = student.Class?.Grade?.Curriculum?.Name ?? "General Curriculum";
                gfx.DrawString($"Curriculum: {ArabicHelper.ShapeArabic(curriculumName)}",
                    GetAppropriateFont(curriculumName, 12, XFontStyleEx.Bold),
                    darkBlueBrush, new XPoint(marginX, currentY - 30));

                // Student info
                gfx.DrawString($"Name: {ArabicHelper.ShapeArabic(student.FullName)}",
                    GetAppropriateFont(student.FullName, 11, XFontStyleEx.Bold),
                    darkBlueBrush, new XPoint(marginX, currentY));
                currentY += 18;

                gfx.DrawString("Grade:", headerFont, darkBlueBrush, new XPoint(marginX, currentY));

                string gradeName = student.Class?.Grade?.GradeName ?? "N/A";
                gfx.DrawString(ArabicHelper.ShapeArabic(gradeName),
                   GetAppropriateFont(gradeName, 11, XFontStyleEx.Bold),
                   darkBlueBrush, new XPoint(marginX + 50, currentY));
                currentY += 18;

                double rightSideX = marginX + 250;
                double rightSideY = startY;

                gfx.DrawString($"Nationality: {ArabicHelper.ShapeArabic(student.Nationality)}",
                    GetAppropriateFont(student.Nationality, 11, XFontStyleEx.Bold),
                    darkBlueBrush, new XPoint(rightSideX, rightSideY));
                rightSideY += 18;

                gfx.DrawString($"Iqama: {ArabicHelper.ShapeArabic(student.IqamaNumber)}",
                    GetAppropriateFont(student.IqamaNumber, 11, XFontStyleEx.Bold),
                    darkBlueBrush, new XPoint(rightSideX, rightSideY));
                rightSideY += 18;

                gfx.DrawString($"Passport: {ArabicHelper.ShapeArabic(student.PassportNumber)}",
                    GetAppropriateFont(student.PassportNumber, 11, XFontStyleEx.Bold),
                    darkBlueBrush, new XPoint(rightSideX, rightSideY));

                currentY = Math.Max(currentY, rightSideY) + 25;

                double[] columnWidths = { 180, 60, 60, 60, 80, 60 };
                double tableStartX = marginX;
                string[] headers = { "COURSE", "MAX", "MARKS", "GPA", "CREDIT HRS", "LETTER" };

                double xPos = tableStartX;
                for (int i = 0; i < headers.Length; i++)
                {
                    var format = new XStringFormat { Alignment = i == 0 ? XStringAlignment.Near : XStringAlignment.Center };
                    gfx.DrawString(headers[i], headerFont, darkBlueBrush, new XRect(xPos, currentY + 4, columnWidths[i], rowHeight), format);
                    xPos += columnWidths[i];
                }
                currentY += rowHeight;

                foreach (var d in degreesToDisplay)
                {
                    if (currentY + rowHeight + 80 > pageHeight)
                    {
                        page = template.AddPage();
                        gfx = XGraphics.FromPdfPage(page);
                        currentY = 80;

                        // Redraw header
                        xPos = tableStartX;
                        for (int i = 0; i < headers.Length; i++)
                        {
                            var format = new XStringFormat { Alignment = i == 0 ? XStringAlignment.Near : XStringAlignment.Center };
                            gfx.DrawString(headers[i], headerFont, darkBlueBrush, new XRect(xPos, currentY + 4, columnWidths[i], rowHeight), format);
                            xPos += columnWidths[i];
                        }
                        currentY += rowHeight;
                    }

                    double subjectGpa = _gpaCalculator.GetSubjectGpa(d.Score, d.MaxScore);
                    string letterGrade = _gpaCalculator.GetLetterGrade(subjectGpa);
                    double creditHours = d.Subject?.CreditHours ?? 3.0;

                    if (degreesToDisplay.IndexOf(d) % 2 == 1)
                    {
                        gfx.DrawRectangle(new XSolidBrush(XColor.FromArgb(252, 252, 252)),
                            tableStartX, currentY, tableWidth, rowHeight);
                    }

                    xPos = tableStartX;

                    var subjectNameFont = GetAppropriateFont(d.Subject?.SubjectName ?? d.SubjectName, 10, XFontStyleEx.Regular);

                    var rowValues = new[]
                    {
                        d.Subject?.SubjectName ?? d.SubjectName,
                        d.MaxScore.ToString(),
                        d.Score.ToString(),
                        subjectGpa.ToString("F2"),
                        creditHours.ToString("F1"),
                        letterGrade
                    };

                    var rowFonts = new[]
                    {
                        subjectNameFont,
                        regularFont,
                        regularFont,
                        regularFont,
                        regularFont,
                        regularFont
                    };

                    for (int i = 0; i < rowValues.Length; i++)
                    {
                        var format = new XStringFormat { Alignment = i == 0 ? XStringAlignment.Near : XStringAlignment.Center };
                        gfx.DrawString(rowValues[i], rowFonts[i], mediumGrayBrush, new XRect(xPos, currentY + 4, columnWidths[i], rowHeight), format);
                        xPos += columnWidths[i];
                    }

                    currentY += rowHeight;
                }

                // Summary section
                double gpa = _gpaCalculator.CalculateGpa(degreesToDisplay);
                double totalMarks = degreesToDisplay.Sum(d => d.Score);
                double totalMax = degreesToDisplay.Sum(d => d.MaxScore);

                currentY += 25;
                double summaryWidth = 300;
                double summaryX = marginX + (tableWidth - summaryWidth) / 2;

                gfx.DrawRectangle(lightGrayBrush, summaryX, currentY, summaryWidth, 35);
                gfx.DrawRectangle(new XPen(XColor.FromArgb(222, 226, 230), 1), summaryX, currentY, summaryWidth, 35);

                var summaryFormat = new XStringFormat { Alignment = XStringAlignment.Center, LineAlignment = XLineAlignment.Center };
                var summaryText = ArabicHelper.ShapeArabic($"TOTAL MARKS: {totalMarks}/{totalMax} | GPA: {gpa:F2}");

                gfx.DrawString(summaryText, headerFont, darkBlueBrush, new XRect(summaryX, currentY, summaryWidth, 35), summaryFormat);

                currentY += 50;
                var footerText = $"Issued on: {DateTime.Now:dd/MM/yyyy} | {curriculumName}";
                gfx.DrawString(footerText, regularFont, mediumGrayBrush, new XPoint(marginX, currentY));

                using var ms = new MemoryStream();
                template.Save(ms, false);
                var pdfBytes = ms.ToArray();

                // Validate the generated PDF
                if (pdfBytes == null || pdfBytes.Length == 0)
                {
                    throw new InvalidOperationException("Generated PDF is empty");
                }

                // Check PDF header
                if (pdfBytes.Length < 5 || System.Text.Encoding.ASCII.GetString(pdfBytes, 0, 5) != "%PDF-")
                {
                    throw new InvalidOperationException("Generated data is not a valid PDF");
                }

                return pdfBytes;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] ERROR in PDF generation: {ex.Message}");
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Stack trace: {ex.StackTrace}");
                throw;
            }
        }

        private List<Degree> CalculateCumulativeDegrees(List<Degree> allDegrees, DegreeType degreeType)
        {
            var cumulativeDegrees = new List<Degree>();

            var degreesBySubject = allDegrees
                .Where(d => d.SubjectId != null)
                .GroupBy(d => d.SubjectId);

            foreach (var subjectGroup in degreesBySubject)
            {
                var subjectDegrees = subjectGroup.ToList();
                var firstDegree = subjectDegrees.First();

                if (degreeType == DegreeType.Final1)
                {
                    var midTerm1 = subjectDegrees.FirstOrDefault(d => d.DegreeType == DegreeType.MidTerm1);
                    var final1 = subjectDegrees.FirstOrDefault(d => d.DegreeType == DegreeType.Final1);

                    if (midTerm1 != null || final1 != null)
                    {
                        var cumulativeScore = (midTerm1?.Score ?? 0) + (final1?.Score ?? 0);
                        var cumulativeMax = (midTerm1?.MaxScore ?? 0) + (final1?.MaxScore ?? 0);

                        cumulativeDegrees.Add(new Degree
                        {
                            Subject = firstDegree.Subject,
                            SubjectName = ArabicHelper.ShapeArabic(firstDegree.Subject?.SubjectName ?? firstDegree.SubjectName),
                            Score = cumulativeScore,
                            MaxScore = cumulativeMax,
                            DegreeType = DegreeType.Final1,
                            SubjectId = firstDegree.SubjectId
                        });
                    }
                }
                else if (degreeType == DegreeType.Final2)
                {
                    var midTerm1 = subjectDegrees.FirstOrDefault(d => d.DegreeType == DegreeType.MidTerm1);
                    var final1 = subjectDegrees.FirstOrDefault(d => d.DegreeType == DegreeType.Final1);
                    var midTerm2 = subjectDegrees.FirstOrDefault(d => d.DegreeType == DegreeType.MidTerm2);
                    var final2 = subjectDegrees.FirstOrDefault(d => d.DegreeType == DegreeType.Final2);

                    if (midTerm1 != null || final1 != null || midTerm2 != null || final2 != null)
                    {
                        var cumulativeScore = (midTerm1?.Score ?? 0)
                                            + (final1?.Score ?? 0)
                                            + (midTerm2?.Score ?? 0)
                                            + (final2?.Score ?? 0);

                        var cumulativeMax = (midTerm1?.MaxScore ?? 0)
                                          + (final1?.MaxScore ?? 0)
                                          + (midTerm2?.MaxScore ?? 0)
                                          + (final2?.MaxScore ?? 0);

                        cumulativeDegrees.Add(new Degree
                        {
                            Subject = firstDegree.Subject,
                            SubjectName = ArabicHelper.ShapeArabic(firstDegree.Subject?.SubjectName ?? firstDegree.SubjectName),
                            Score = cumulativeScore,
                            MaxScore = cumulativeMax,
                            DegreeType = DegreeType.Final2,
                            SubjectId = firstDegree.SubjectId
                        });
                    }
                }
            }

            return cumulativeDegrees;
        }

        public string GetTemplatePath(DegreeType degreeType, string? curriculumCode = null, string templateRootPath = "Templates")
        {
            var rootPath = Path.Combine(_env.ContentRootPath, templateRootPath);

            if (!string.IsNullOrEmpty(curriculumCode))
            {
                var curriculumSpecificPath = Path.Combine(rootPath, curriculumCode, $"{degreeType}.pdf");
                if (System.IO.File.Exists(curriculumSpecificPath))
                {
                    Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Found curriculum-specific template: {curriculumSpecificPath}");
                    return curriculumSpecificPath;
                }

                curriculumSpecificPath = Path.Combine(rootPath, curriculumCode, "Default.pdf");
                if (System.IO.File.Exists(curriculumSpecificPath))
                {
                    Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Found curriculum default template: {curriculumSpecificPath}");
                    return curriculumSpecificPath;
                }
            }

            var fallbackPath = Path.Combine(rootPath, $"{degreeType}.pdf");
            if (System.IO.File.Exists(fallbackPath))
            {
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Using fallback template: {fallbackPath}");
                return fallbackPath;
            }

            var defaultPath = Path.Combine(rootPath, "Default.pdf");
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Using default template: {defaultPath}");
            return defaultPath;
        }

        public async Task<Certificate?> GetCertificateAsync(string studentId, DegreeType degreeType, string? academicYear = null)
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AlMaalyGateSchoolContext>();

            var query = context.Certificates.Where(c => c.StudentId == studentId && c.DegreeType == degreeType);

            if (!string.IsNullOrEmpty(academicYear))
                query = query.Where(c => c.AcademicYear == academicYear);

            return await query.FirstOrDefaultAsync();
        }

        public async Task<List<Certificate>> GetCertificatesByCurriculumAsync(string curriculumId, string? academicYear = null)
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AlMaalyGateSchoolContext>();

            var query = context.Certificates.Where(c => c.CurriculumId == curriculumId);

            if (!string.IsNullOrEmpty(academicYear))
                query = query.Where(c => c.AcademicYear == academicYear);

            return await query
                .Include(c => c.Student)
                .OrderByDescending(c => c.IssuedDate)
                .ToListAsync();
        }

        public async Task<List<Certificate>> GetCertificatesByGradeAsync(string gradeId, string? academicYear = null)
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AlMaalyGateSchoolContext>();

            var query = context.Certificates.Where(c => c.GradeId == gradeId);

            if (!string.IsNullOrEmpty(academicYear))
                query = query.Where(c => c.AcademicYear == academicYear);

            return await query
                .Include(c => c.Student)
                .OrderByDescending(c => c.IssuedDate)
                .ToListAsync();
        }

        public async Task<List<Certificate>> GetCertificatesByClassAsync(string classId, string? academicYear = null)
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AlMaalyGateSchoolContext>();

            var query = context.Certificates.Where(c => c.ClassId == classId);

            if (!string.IsNullOrEmpty(academicYear))
                query = query.Where(c => c.AcademicYear == academicYear);

            return await query
                .Include(c => c.Student)
                .OrderByDescending(c => c.IssuedDate)
                .ToListAsync();
        }

        public async Task<List<Certificate>> GetCertificatesForDownloadAsync(string classId, DegreeType degreeType, string? academicYear = null)
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AlMaalyGateSchoolContext>();

            var query = context.Certificates
                .Where(c => c.ClassId == classId && c.DegreeType == degreeType);

            if (!string.IsNullOrEmpty(academicYear))
                query = query.Where(c => c.AcademicYear == academicYear);

            // Return certificates WITH PDF data for download
            return await query
                .OrderByDescending(c => c.IssuedDate)
                .ToListAsync();
        }

        public async Task<byte[]> DownloadCertificatesAsZipAsync(List<Certificate> certificates)
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AlMaalyGateSchoolContext>();

            // Get full certificates with PDF data
            var certificateIds = certificates.Select(c => c.Id).ToList();
            var fullCertificates = await context.Certificates
                .Where(c => certificateIds.Contains(c.Id))
                .ToListAsync();

            using var memoryStream = new MemoryStream();
            using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
            {
                foreach (var certificate in fullCertificates)
                {
                    // Ensure PDF data exists
                    if (certificate.PdfData == null || certificate.PdfData.Length == 0)
                        continue;

                    var entryName = $"{certificate.FileName}";
                    var entry = archive.CreateEntry(entryName);

                    using var entryStream = entry.Open();
                    entryStream.Write(certificate.PdfData, 0, certificate.PdfData.Length);
                }
            }

            memoryStream.Position = 0;
            return memoryStream.ToArray();
        }

        public async Task<bool> VerifyCertificateAsync(string certificateId, string verifiedBy)
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AlMaalyGateSchoolContext>();

            var certificate = await context.Certificates.FindAsync(certificateId);
            if (certificate == null)
                return false;

            certificate.IsVerified = true;
            certificate.VerifiedDate = DateTime.Now;
            certificate.VerifiedBy = verifiedBy;

            await context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ArchiveCertificateAsync(string certificateId)
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AlMaalyGateSchoolContext>();

            var certificate = await context.Certificates.FindAsync(certificateId);
            if (certificate == null)
                return false;

            certificate.IsArchived = true;
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<Certificate?> GetCertificateByIdAsync(string certificateId)
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AlMaalyGateSchoolContext>();

            return await context.Certificates
                .Include(c => c.Student)
                .FirstOrDefaultAsync(c => c.Id == certificateId);
        }

        public async Task<List<Certificate>> SearchCertificatesAsync(
            string? studentName = null,
            string? certificateNumber = null,
            string? curriculumId = null,
            string? gradeId = null,
            string? classId = null,
            DegreeType? degreeType = null,
            string? academicYear = null,
            DateTime? fromDate = null,
            DateTime? toDate = null)
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AlMaalyGateSchoolContext>();

            var query = context.Certificates.AsQueryable();

            if (!string.IsNullOrEmpty(studentName))
            {
                query = query.Where(c => c.Student != null &&
                    (c.Student.FullName.Contains(studentName) ||
                     c.Student.Email.Contains(studentName)));
            }

            if (!string.IsNullOrEmpty(certificateNumber))
                query = query.Where(c => c.CertificateNumber != null && c.CertificateNumber.Contains(certificateNumber));

            if (!string.IsNullOrEmpty(curriculumId))
                query = query.Where(c => c.CurriculumId == curriculumId);

            if (!string.IsNullOrEmpty(gradeId))
                query = query.Where(c => c.GradeId == gradeId);

            if (!string.IsNullOrEmpty(classId))
                query = query.Where(c => c.ClassId == classId);

            if (degreeType.HasValue)
                query = query.Where(c => c.DegreeType == degreeType.Value);

            if (!string.IsNullOrEmpty(academicYear))
                query = query.Where(c => c.AcademicYear == academicYear);

            if (fromDate.HasValue)
                query = query.Where(c => c.IssuedDate >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(c => c.IssuedDate <= toDate.Value.AddDays(1));

            return await query
                .Include(c => c.Student)
                .OrderByDescending(c => c.IssuedDate)
                .ToListAsync();
        }

        // Add this method to help debug PDF issues
        public async Task<ServiceResult<string>> DebugCertificatePdfAsync(string certificateId)
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AlMaalyGateSchoolContext>();

            var certificate = await context.Certificates
                .FirstOrDefaultAsync(c => c.Id == certificateId);

            if (certificate == null)
                return ServiceResult<string>.Fail("Certificate not found");

            if (certificate.PdfData == null)
                return ServiceResult<string>.Fail("Certificate has no PDF data");

            if (certificate.PdfData.Length == 0)
                return ServiceResult<string>.Fail("Certificate PDF data is empty");

            // Check PDF header
            var header = certificate.PdfData.Length >= 5
                ? System.Text.Encoding.ASCII.GetString(certificate.PdfData, 0, 5)
                : "N/A (too short)";

            // Save a debug copy locally
            var debugPath = Path.Combine(_env.ContentRootPath, "Debug", $"certificate_{certificateId}.pdf");
            Directory.CreateDirectory(Path.GetDirectoryName(debugPath)!);
            await System.IO.File.WriteAllBytesAsync(debugPath, certificate.PdfData);

            var info = $"PDF Info: {certificate.PdfData.Length} bytes, Header: {header}, Saved to: {debugPath}";
            return ServiceResult<string>.Ok(info);
        }
    }

}