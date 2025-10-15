using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    public class FileRecordConfiguration : IEntityTypeConfiguration<FileRecord>
    {
        public void Configure(EntityTypeBuilder<FileRecord> builder) 
        {
            builder.HasKey(f => f.Id);

            builder.Property(f => f.FileName).IsRequired().HasMaxLength(255);
            builder.Property(f => f.RelativePath).IsRequired().HasMaxLength(500);
            builder.Property(f => f.ControllerName).HasMaxLength(100);
            builder.Property(f => f.FileType).HasMaxLength(10);

            builder.ToTable("FileRecords","Files");
        }
    }
}
