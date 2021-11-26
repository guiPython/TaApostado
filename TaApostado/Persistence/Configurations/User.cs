using TaApostado.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TaApostado.Persistence.Configurations
{
    public class UserDBConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(u => u.Id);

            builder.HasMany(c => c.Challenges).WithOne(c => c.Challenged).HasForeignKey(c => c.IdChallenged);

            builder.HasMany(c => c.Bets).WithOne(b => b.Bettor).HasForeignKey(c => c.IdBettor);

            builder.Property(c => c.Name).HasMaxLength(30).IsRequired(true);

            builder.Property(c => c.LastName).HasMaxLength(30).IsRequired(true);

            builder.HasIndex(c => c.CPF).IsUnique(true);

            builder.Property(c => c.CPF).IsRequired(true);

            builder.HasIndex(c => c.Email).IsUnique(true);

            builder.Property(c => c.Email).HasMaxLength(45).IsRequired(true);
        }
    }
}
