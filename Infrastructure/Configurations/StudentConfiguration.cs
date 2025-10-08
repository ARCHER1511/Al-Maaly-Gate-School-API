using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    public class StudentConfiguration : IEntityTypeConfiguration<Student>
    {
        public void Configure(EntityTypeBuilder<Student> builder)
        {
            new UserBaseConfiguration<Student>().Configure(builder);

            builder.Property(s => s.ClassYear)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.Property(s => s.Age)
                   .IsRequired();

            builder.HasOne(s => s.Class)
                   .WithMany(c => c.Students)
                   .HasForeignKey(s => s.ClassId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
