using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using quizz.Entities;
using quizz.Utils;

namespace quizz.Data;

public class ApplicationDbContext : DbContext
{
    public DbSet<Topic>? Topics { get; set; }
    public DbSet<Quiz>? Quizzes { get; set; }
    public DbSet<Question>? Questions { get; set; }
    public DbSet<McqOption>? McqOptions { get; set; }
    public DbSet<TestCase>? TestCases { get; set; }
    
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

    public override int SaveChanges()
    {
        AddNameHash();
        SetDates();
        
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        AddNameHash();
        SetDates();

        return base.SaveChangesAsync(cancellationToken);
    }

    private void SetDates()
    {
        foreach(var entry in ChangeTracker.Entries<EntityBase>())
        {
            if(entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = DateTime.UtcNow;
                entry.Entity.UpdatedAt = DateTime.UtcNow;
            }

            if(entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = DateTime.UtcNow;
            }
        }
    }

    private void AddNameHash()
    {
        foreach(var entry in ChangeTracker.Entries<Topic>())
        {
            if(entry.Entity is Topic topic)
                topic.NameHash = topic?.Name?.ToLower().Sha256();
        }
    }
}