using TaApostado.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TaApostado.Persistence.Configurations
{
    public class ChallengeDBConfiguration : IEntityTypeConfiguration<Challenge>
    {
        public void Configure(EntityTypeBuilder<Challenge> builder)
        {
            builder.HasKey(c => c.Id);

            builder.HasOne(c => c.Challenged).WithOne().HasForeignKey<Challenge>(c => c.IdChallenged);

            builder.Property(c => c.Name).HasMaxLength(30).IsRequired(true);

            builder.Property(c => c.Description).HasMaxLength(80).IsRequired(true);

            builder.Property(c => c.Theme).HasMaxLength(12).IsRequired(true);

            builder.Property(c => c.TimeExecution).IsRequired(true);

            builder.Property(c => c.TimeOpen).IsRequired(true);
        }
    }
}
