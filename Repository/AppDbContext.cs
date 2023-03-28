using Entities.Database;
using Microsoft.EntityFrameworkCore;

namespace Repository;

public sealed class AppDbContext: DbContext
{
    public DbSet<UserEntity> UserEntities { get; set; }

    public DbSet<ExamEntity> ExamEntities { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options): base(options)
    {
        Database.EnsureCreated();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ExamEntity>()
            .HasOne<UserEntity>()
            .WithMany()
            .HasForeignKey(k => k.UserId);
    }
}
