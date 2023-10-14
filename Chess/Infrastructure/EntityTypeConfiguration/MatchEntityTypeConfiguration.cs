using System.Text.Json;
using Chess.Domain.Configuration;
using Chess.Infrastructure.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chess.Infrastructure.EntityTypeConfiguration;

public class MatchEntityTypeConfiguration : IEntityTypeConfiguration<Match>
{
    public void Configure(EntityTypeBuilder<Match> builder)
    {
       builder.HasKey(m => m.AggregateId);

       builder.Property(m => m.BlackPlayerId)
              .IsRequired();

       builder.Property(m => m.WhitePlayerId)
              .IsRequired();

       builder.Property(m => m.StartTime)
              .IsRequired();

       builder.Property(m => m.Options)
              .IsRequired()
              .HasConversion(m => JsonSerializer.Serialize(m, JsonSerializerOptions.Default),
                             m => JsonSerializer.Deserialize<MatchOptions>(m, JsonSerializerOptions.Default));

       builder.HasMany(m => m.Events)
              .WithOne(m => m.Match);

    }
}

