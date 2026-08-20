using Films.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Films.Infrastructure.Data.Configurations;

public class DirectedByConfiguration : IEntityTypeConfiguration<DirectedBy>
{
    public void Configure(EntityTypeBuilder<DirectedBy> builder)
    {
        builder.ToTable("DirectedBy");

        builder.HasKey(d => d.Id);

        builder.Property(d => d.FirstName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(d => d.LastName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(d => d.Nationality)
            .HasMaxLength(100);

        builder.Property(d => d.CreatedDate)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(d => d.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(d => d.CreatedBy)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(d => d.ModifiedBy)
            .HasMaxLength(100);

        // Relationships
        builder.HasOne(d => d.Sex)
            .WithMany(s => s.Directors)
            .HasForeignKey(d => d.SexId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(d => d.RefDAFs)
            .WithOne(r => r.Director)
            .HasForeignKey(r => r.DirectorId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
