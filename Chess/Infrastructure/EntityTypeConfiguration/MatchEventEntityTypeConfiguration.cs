using Chess.Infrastructure.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chess.Infrastructure.EntityTypeConfiguration;

public class MatchEventEntityTypeConfiguration : IEntityTypeConfiguration<MatchEvent>
{
    public void Configure(EntityTypeBuilder<MatchEvent> builder)
    {
       builder.HasKey(m => m.Id);

       builder.Property(m => m.AggregateId)
              .IsRequired();
       builder.Property(m => m.Data)
              .IsRequired();
       builder.Property(m => m.Type)
              .IsRequired();
       builder.Property(m => m.Version)
              .IsRequired();

       builder.HasOne(m => m.Match)
              .WithMany(m => m.Events)
              .HasForeignKey(m => m.AggregateId)
              .HasConstraintName("FK_MatchEvent_AggregateId")
              .OnDelete(DeleteBehavior.Cascade);
    }
}