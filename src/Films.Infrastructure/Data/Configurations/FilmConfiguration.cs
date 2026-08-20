using Films.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Films.Infrastructure.Data.Configurations;

public class FilmConfiguration : IEntityTypeConfiguration<Film>
{
    public void Configure(EntityTypeBuilder<Film> builder)
    {
        builder.ToTable("Films");

        builder.HasKey(f => f.Id);

        builder.Property(f => f.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(f => f.Description)
            .HasMaxLength(1000);

        builder.Property(f => f.Genre)
            .HasMaxLength(100);

        builder.Property(f => f.Country)
            .HasMaxLength(100);

        builder.Property(f => f.CreatedDate)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(f => f.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(f => f.CreatedBy)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(f => f.ModifiedBy)
            .HasMaxLength(100);

        // Relationships
        builder.HasMany(f => f.RefAFs)
            .WithOne(r => r.Film)
            .HasForeignKey(r => r.FilmId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(f => f.RefDAFs)
            .WithOne(r => r.Film)
            .HasForeignKey(r => r.FilmId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
