using Chess.Infrastructure.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chess.Infrastructure.EntityTypeConfiguration;

public class MatchEntityTypeConfiguration : IEntityTypeConfiguration<Match>
{
    public void Configure(EntityTypeBuilder<Match> builder)
    {
        builder.HasKey(m => m.BlackPlayerId);

        builder.Property(m => m.BlackPlayerId)
               .IsRequired();

        builder.Property(m => m.WhitePlayerId)
               .IsRequired();
    }
}

