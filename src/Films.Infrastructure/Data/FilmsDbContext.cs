using Films.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Films.Infrastructure.Data;

/// <summary>
/// Database context for the Films application
/// </summary>
public class FilmsDbContext : DbContext
{
    public FilmsDbContext(DbContextOptions<FilmsDbContext> options) : base(options)
    {
    }

    public DbSet<Film> Films { get; set; } = null!;
    public DbSet<Actor> Actors { get; set; } = null!;
    public DbSet<Director> Directors { get; set; } = null!;
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Sex> Sexes { get; set; } = null!;
    public DbSet<TypeUser> TypeUsers { get; set; } = null!;
    public DbSet<Right> Rights { get; set; } = null!;
    public DbSet<UserRight> UserRights { get; set; } = null!;
    public DbSet<RefAF> ActorFilms { get; set; } = null!;
    public DbSet<RefDAF> DirectorFilms { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Film configuration
        modelBuilder.Entity<Film>(entity =>
        {
            entity.ToTable("Films");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.Genre).HasMaxLength(100);
        });

        // Actor configuration
        modelBuilder.Entity<Actor>(entity =>
        {
            entity.ToTable("Actors");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
            entity.HasOne(e => e.Sex)
                .WithMany(s => s.Actors)
                .HasForeignKey(e => e.SexId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // Director configuration
        modelBuilder.Entity<Director>(entity =>
        {
            entity.ToTable("Directors");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
            entity.HasOne(e => e.Sex)
                .WithMany(s => s.Directors)
                .HasForeignKey(e => e.SexId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // User configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Users");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Username).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(200);
            entity.Property(e => e.PasswordHash).IsRequired();
            entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
            entity.HasOne(e => e.TypeUser)
                .WithMany(t => t.Users)
                .HasForeignKey(e => e.TypeUserId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // Sex configuration
        modelBuilder.Entity<Sex>(entity =>
        {
            entity.ToTable("Sex");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(50);
        });

        // TypeUser configuration
        modelBuilder.Entity<TypeUser>(entity =>
        {
            entity.ToTable("TypeUsers");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
        });

        // Right configuration
        modelBuilder.Entity<Right>(entity =>
        {
            entity.ToTable("Rights");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
        });

        // UserRight configuration
        modelBuilder.Entity<UserRight>(entity =>
        {
            entity.ToTable("UserRights");
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.User)
                .WithMany(u => u.UserRights)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Right)
                .WithMany(r => r.UserRights)
                .HasForeignKey(e => e.RightId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // RefAF configuration (Actor-Film relationship)
        modelBuilder.Entity<RefAF>(entity =>
        {
            entity.ToTable("RefAF");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Role).HasMaxLength(200);
            entity.HasOne(e => e.Actor)
                .WithMany(a => a.ActorFilms)
                .HasForeignKey(e => e.ActorId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Film)
                .WithMany(f => f.ActorFilms)
                .HasForeignKey(e => e.FilmId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // RefDAF configuration (Director-Film relationship)
        modelBuilder.Entity<RefDAF>(entity =>
        {
            entity.ToTable("RefDAF");
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Director)
                .WithMany(d => d.DirectorFilms)
                .HasForeignKey(e => e.DirectorId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Film)
                .WithMany(f => f.DirectorFilms)
                .HasForeignKey(e => e.FilmId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
