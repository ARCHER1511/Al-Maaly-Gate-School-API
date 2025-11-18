using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

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
        }

        public async Task<byte[]> GenerateCertificatePdfAsync(string studentId)
        {
            // Load student with degrees and subjects
            var student = await _unitOfWork.Students.FirstOrDefaultAsync(
                s => s.Id == studentId,
                include: q => q
                    .Include(s => s.Degrees!)
                    .ThenInclude(d => d.Subject)
                    .Include(s => s.Class)
            );

            if (student == null)
                throw new KeyNotFoundException("Student not found");

            var degrees = student.Degrees!.ToList();

            double totalGpa = _gpaCalculator.CalculateGpa(degrees);
            double totalMarks = degrees.Sum(d => d.Score);
            double totalMax = degrees.Sum(d => d.MaxScore);
            double gpaPercent = totalMax > 0 ? (totalMarks / totalMax) * 100.0 : 0.0;

            // Save certificate record
            var cert = new Certificate
            {
                StudentId = student.Id,
                GPA = totalGpa,
                IssuedDate = DateTime.UtcNow,
                TemplateName = "Default"
            };

            await _unitOfWork.Certificates.AddAsync(cert);
            await _unitOfWork.SaveChangesAsync();

            // ========================
            // PDF GENERATION
            // ========================
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(20);
                    page.DefaultTextStyle(x => x.FontSize(12));

                    // Header
                    page.Header().Column(col =>
                    {
                        col.Item().Text("المعالي بوابة مدارس").Bold().FontSize(18).AlignCenter();
                        col.Item().Text("Al Maaly Gate Int'l Schools").Bold().FontSize(14).AlignCenter();
                        col.Item().Text("K.S.A - Ministry of Education").AlignCenter();
                        col.Item().Text($"Academic Year: {student.ClassYear}").AlignCenter();
                    });

                    // Content: Only ONE page.Content()
                    page.Content().PaddingTop(10).Column(col =>
                    {
                        col.Spacing(8);

                        // Student Info
                        col.Item().Text($"Name: {student.FullName}").Bold();
                        string grade = student.Class?.ClassYear ?? "N/A";
                        col.Item().Text($"Grade: {grade}");
                        col.Item().Text($"Nationality: {student.Nationality}");
                        col.Item().Text($"Iqama No: {student.IqamaNumber}  Passport No: {student.PassportNumber}");

                        // Subjects Table
                        col.Item().PaddingTop(10).Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(2); // Course
                                columns.RelativeColumn();  // Max
                                columns.RelativeColumn();  // Marks
                                columns.RelativeColumn();  // Course GPA
                                columns.RelativeColumn();  // Credit Hours
                                columns.RelativeColumn();  // Letter Grade
                            });

                            table.Header(header =>
                            {
                                header.Cell().Text("Course").SemiBold();
                                header.Cell().Text("Max");
                                header.Cell().Text("Marks Obtained");
                                header.Cell().Text("Course GPA");
                                header.Cell().Text("Credit Hours");
                                header.Cell().Text("Letter Grade");
                            });

                            foreach (var d in degrees)
                            {
                                double subjectGpa = _gpaCalculator.GetSubjectGpa(d.Score, d.MaxScore);
                                string letter = _gpaCalculator.GetLetterGrade(subjectGpa);
                                double creditHours = d.Subject?.CreditHours ?? 3.0;

                                table.Cell().Text(d.Subject?.SubjectName ?? d.SubjectName);
                                table.Cell().Text(d.MaxScore.ToString());
                                table.Cell().Text(d.Score.ToString());
                                table.Cell().Text(subjectGpa.ToString("F2"));
                                table.Cell().Text(creditHours.ToString("F2"));
                                table.Cell().Text(letter);
                            }
                        });

                        // Totals & Comments
                        col.Item().PaddingTop(10).Column(totals =>
                        {
                            totals.Item().Text($"Total Marks: {totalMarks} / {totalMax}");
                            totals.Item().Text($"Total GPA: {totalGpa:F2} / 4.0  GPA%: {gpaPercent:F2}%");
                            totals.Item().Text("General Comment: Excellent").Italic();
                        });
                    });

                    // Footer
                    page.Footer().AlignCenter().Text("➤ This certificate is for the parent's use ONLY & not for the official use.").FontSize(10);
                });
            });

            using var ms = new MemoryStream();
            document.GeneratePdf(ms);
            return ms.ToArray();
        }
    }
}
