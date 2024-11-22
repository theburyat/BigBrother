using BigBrother.Repository.Entities;
using Microsoft.EntityFrameworkCore;

namespace BigBrother.Repository.Context;

public sealed class Context : DbContext
{
    private readonly string _connectionString;

    internal DbSet<GroupEntity> Groups => null!;
    internal DbSet<UserEntity> Users => null!;
    internal DbSet<SessionEntity> Sessions => null!;
    internal DbSet<ScoreEntity> Scores => null!;
    internal DbSet<ActionEntity> Actions => null!;

    internal Context(string connectionString)
    {
        _connectionString = connectionString;

        Database.EnsureCreated();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(_connectionString);
        base.OnConfiguring(optionsBuilder);
    }
}
