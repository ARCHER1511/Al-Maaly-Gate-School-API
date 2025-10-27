using Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Configurations
{
    public class TrueAndFalsesConfiguration : IEntityTypeConfiguration<TrueAndFalses>
    {
        public void Configure(EntityTypeBuilder<TrueAndFalses> builder)
        {
            builder.ToTable("TrueAndFalses", "Academics");

            builder.HasKey(tf => tf.Id);

            builder.Property(tf => tf.IsTrue)
                   .IsRequired();

            builder.HasOne(tf => tf.Question)
                   .WithOne(q => q.TrueAndFalses)
                   .HasForeignKey<TrueAndFalses>(tf => tf.QuestionId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
