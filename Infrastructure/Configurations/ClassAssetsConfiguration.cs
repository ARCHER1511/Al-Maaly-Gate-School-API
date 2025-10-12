using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    public class ClassAssetsConfiguration : IEntityTypeConfiguration<ClassAssets>
    {
        public void Configure(EntityTypeBuilder<ClassAssets> builder)
        {
            new BaseEntityConfiguration<ClassAssets>().Configure(builder);

            builder.Property(a => a.AssetsPath)
                   .HasMaxLength(500);

            builder.HasOne(a => a.Class)
                   .WithMany(c => c.ClassAssets)
                   .HasForeignKey(a => a.ClassId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.ToTable("ClassAssets", "Academics");
        }
    }
}
