using TaApostado.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TaApostado.Persistence.Configurations
{
    public class BetDBConfiguration : IEntityTypeConfiguration<Bet>
    {
        public void Configure(EntityTypeBuilder<Bet> builder)
        {
            builder.HasKey(b => b.Id);

            builder.HasOne(b => b.Challenge).WithMany(c => c.Bets).HasForeignKey(b => b.IdChallenge);

            builder.HasOne(b => b.Bettor).WithOne().HasForeignKey<Bet>(b => b.IdBettor);       
        }
    }
}
