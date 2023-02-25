using Entities.Database;
using Microsoft.EntityFrameworkCore;

namespace Repository;

public sealed class AppDbContext: DbContext
{
    public DbSet<UserEntity> UserEntities { get; set; }

    public DbSet<ExamEntity> ExamEntities { get; set; }

    public AppDbContext()
    {
        Database.EnsureCreated();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=bigbrother.db");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ExamEntity>()
            .HasOne<UserEntity>()
            .WithMany()
            .HasForeignKey(k => k.UserId);
    }
}