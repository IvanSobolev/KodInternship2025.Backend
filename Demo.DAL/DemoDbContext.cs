using Demo.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace Demo.DAL;

public class DemoDbContext(DbContextOptions<DemoDbContext> options) : DbContext(options)
{
    public DbSet<Worker> Workers { get; set; }
    public DbSet<ProjectTask> Tasks { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Worker>(entity =>
        {
            entity.HasIndex(w => w.TelegramUserId).IsUnique();

            entity.Property(w => w.Department)
                .HasConversion<string>()
                .HasMaxLength(50);

            entity.HasMany(w => w.AssignedTasks)
                .WithOne(t => t.AssignedWorker)
                .HasForeignKey(t => t.AssignedWorkerId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<ProjectTask>(entity =>
        {
            entity.Property(t => t.Status)
                .HasConversion<string>()
                .HasMaxLength(50);

            entity.Property(t => t.Department)
                .HasConversion<string>()
                .HasMaxLength(50);
        });
    }
    
    public override int SaveChanges()
    {
        AddTimestamps();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        AddTimestamps();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void AddTimestamps()
    {
        var entities = ChangeTracker.Entries()
            .Where(x => x.Entity is ProjectTask && (x.State == EntityState.Added || x.State == EntityState.Modified));

        foreach (var entityEntry in entities)
        {
            var task = (ProjectTask)entityEntry.Entity;
            task.UpdatedAt = DateTime.UtcNow;

            if (entityEntry.State == EntityState.Added)
            {
                task.CreatedAt = DateTime.UtcNow;
            }
        }
    }
}