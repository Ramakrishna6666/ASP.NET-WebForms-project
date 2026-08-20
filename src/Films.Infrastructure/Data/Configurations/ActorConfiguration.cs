using Films.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Films.Infrastructure.Data.Configurations;

public class ActorConfiguration : IEntityTypeConfiguration<Actor>
{
    public void Configure(EntityTypeBuilder<Actor> builder)
    {
        builder.ToTable("Actors");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.FirstName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(a => a.LastName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(a => a.Nationality)
            .HasMaxLength(100);

        builder.Property(a => a.CreatedDate)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(a => a.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(a => a.CreatedBy)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(a => a.ModifiedBy)
            .HasMaxLength(100);

        // Relationships
        builder.HasOne(a => a.Sex)
            .WithMany(s => s.Actors)
            .HasForeignKey(a => a.SexId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(a => a.RefAFs)
            .WithOne(r => r.Actor)
            .HasForeignKey(r => r.ActorId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
