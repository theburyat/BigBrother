using System.Diagnostics.CodeAnalysis;
using BigBrother.Repository.Entities;
using Microsoft.EntityFrameworkCore;

namespace BigBrother.Repository.Context;

[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
public sealed class Context : DbContext
{
    private readonly string _connectionString;

    public DbSet<GroupEntity> Groups { get; set; }
    public DbSet<UserEntity> Users { get; set; }
    public DbSet<SessionEntity> Sessions { get; set; }
    public DbSet<ScoreEntity> Scores { get; set; }
    public DbSet<ActionEntity> Actions { get; set; }

    public Context(string connectionString)
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
