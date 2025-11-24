using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using PdfSharp.Fonts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Application.Helpers;

namespace Application.Services
{
    public class CertificateService : ICertificateService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGpaCalculator _gpaCalculator;

        public CertificateService(IUnitOfWork unitOfWork, IGpaCalculator gpaCalculator)
        {
            _unitOfWork = unitOfWork;
            _gpaCalculator = gpaCalculator;

            if (GlobalFontSettings.FontResolver == null)
                GlobalFontSettings.FontResolver = new FontResolver();
        }

        public async Task<byte[]> GenerateCertificateForStudentAsync(string studentId, DegreeType degreeType, string templatePath)
        {
            var (pdfBytes, _) = await GenerateCertificateInternalAsync(studentId, degreeType, templatePath);
            return pdfBytes;
        }

        public async Task<Certificate> GenerateAndSaveCertificateAsync(string studentId, DegreeType degreeType, string templatePath)
        {
            var (pdfBytes, gpa) = await GenerateCertificateInternalAsync(studentId, degreeType, templatePath);

            // Check if certificate already exists - FIXED: No include parameter
            var existingCertificate = await _unitOfWork.Certificates
                .FirstOrDefaultAsync(c => c.StudentId == studentId && c.DegreeType == degreeType);

            if (existingCertificate != null)
            {
                // Update existing certificate
                existingCertificate.PdfData = pdfBytes;
                existingCertificate.GPA = gpa;
                existingCertificate.IssuedDate = DateTime.UtcNow;
                existingCertificate.FileSize = pdfBytes.Length;
                existingCertificate.FileName = $"{studentId}_{degreeType}_certificate.pdf";

                _unitOfWork.Certificates.Update(existingCertificate);
            }
            else
            {
                // Create new certificate
                var certificate = new Certificate
                {
                    StudentId = studentId,
                    DegreeType = degreeType,
                    GPA = gpa,
                    PdfData = pdfBytes,
                    FileName = $"{studentId}_{degreeType}_certificate.pdf",
                    ContentType = "application/pdf",
                    FileSize = pdfBytes.Length,
                    TemplateName = degreeType.ToString(),
                    IssuedDate = DateTime.UtcNow
                };

                await _unitOfWork.Certificates.AddAsync(certificate);
            }

            await _unitOfWork.SaveChangesAsync();
            
            var finalCertificate = existingCertificate ?? await _unitOfWork.Certificates
                .FirstOrDefaultAsync(c => c.StudentId == studentId && c.DegreeType == degreeType);

            if (finalCertificate is null)
                throw new InvalidOperationException("Certificate should exist but was not found in the database.");

            return finalCertificate;
        }

        public async Task<Certificate?> GetCertificateAsync(string studentId, DegreeType degreeType)
        {
            // FIXED: No include parameter
            return await _unitOfWork.Certificates
                .FirstOrDefaultAsync(c => c.StudentId == studentId && c.DegreeType == degreeType);
        }

        private async Task<(byte[] pdfBytes, double gpa)> GenerateCertificateInternalAsync(string studentId, DegreeType degreeType, string templatePath)
        {
            // FIXED: Use the correct FirstOrDefaultAsync with include parameter
            var student = await _unitOfWork.Students.FirstOrDefaultAsync(
                s => s.Id == studentId,
                include: q => q
                    .Include(s => s.Degrees!)
                    .ThenInclude(d => d.Subject)
                    .Include(s => s.Class)!
            );

            if (student == null)
                throw new KeyNotFoundException("Student not found");

            // Shape Arabic text for student fields
            student.FullName = ArabicHelper.ShapeArabic(student.FullName);
            student.Nationality = ArabicHelper.ShapeArabic(student.Nationality);
            student.PassportNumber = ArabicHelper.ShapeArabic(student.PassportNumber ?? "");
            student.IqamaNumber = ArabicHelper.ShapeArabic(student.IqamaNumber ?? "");
            if (student.Class != null)
                student.Class.ClassYear = ArabicHelper.ShapeArabic(student.Class.ClassYear);

            var allDegrees = student.Degrees!.ToList();
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

            // Shape all subject names
            foreach (var d in degreesToDisplay)
            {
                if (d.Subject != null)
                    d.Subject.SubjectName = ArabicHelper.ShapeArabic(d.Subject.SubjectName);
                d.SubjectName = ArabicHelper.ShapeArabic(d.SubjectName ?? "");
            }

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

            // Auto Arabic font selector + shaping
            XFont GetAppropriateFont(string text, double size, XFontStyleEx style)
            {
                if (FontResolver.ContainsArabic(text))
                {
                    text = ArabicHelper.ShapeArabic(text);
                    return new XFont("ArabicFont", size, style);
                }
                else
                {
                    return new XFont("MyFont", size, style);
                }
            }

            var darkBlueBrush = new XSolidBrush(XColor.FromArgb(44, 62, 80));
            var mediumGrayBrush = new XSolidBrush(XColor.FromArgb(108, 117, 125));
            var lightGrayBrush = new XSolidBrush(XColor.FromArgb(248, 249, 250));

            var headerFont = new XFont("MyFont", 11, XFontStyleEx.Bold);
            var regularFont = new XFont("MyFont", 10, XFontStyleEx.Regular);

            double pageWidth = page.Width;
            double pageHeight = page.Height;
            double tableWidth = 480;
            double marginX = (pageWidth - tableWidth) / 2 + 100;
            double startY = 180;
            double rowHeight = 22;
            double currentY = startY;

            // Student info
            gfx.DrawString($"Name: {ArabicHelper.ShapeArabic(student.FullName)}", GetAppropriateFont(student.FullName, 11, XFontStyleEx.Bold), darkBlueBrush, new XPoint(marginX, currentY));
            currentY += 18;

            gfx.DrawString("Grade:", headerFont, darkBlueBrush, new XPoint(marginX, currentY));
            gfx.DrawString(ArabicHelper.ShapeArabic(student.Class?.ClassYear ?? "N/A"),
               GetAppropriateFont(student.Class?.ClassYear ?? "N/A", 11, XFontStyleEx.Bold),
               darkBlueBrush, new XPoint(marginX + 50, currentY));
            currentY += 18;

            double rightSideX = marginX + 250;
            double rightSideY = startY;

            gfx.DrawString($"Nationality: {ArabicHelper.ShapeArabic(student.Nationality)}", GetAppropriateFont(student.Nationality, 11, XFontStyleEx.Bold), darkBlueBrush, new XPoint(rightSideX, rightSideY));
            rightSideY += 18;

            gfx.DrawString($"Iqama: {ArabicHelper.ShapeArabic(student.IqamaNumber)}", GetAppropriateFont(student.IqamaNumber, 11, XFontStyleEx.Bold), darkBlueBrush, new XPoint(rightSideX, rightSideY));
            rightSideY += 18;

            gfx.DrawString($"Passport: {ArabicHelper.ShapeArabic(student.PassportNumber)}", GetAppropriateFont(student.PassportNumber, 11, XFontStyleEx.Bold), darkBlueBrush, new XPoint(rightSideX, rightSideY));

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

            using var ms = new MemoryStream();
            template.Save(ms, false);

            return (ms.ToArray(), gpa);
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
    }
}