using Demo.DAL.Models;
using Microsoft.EntityFrameworkCore;
namespace Demo.DAL;

public class DemoDbContext (DbContextOptions<DemoDbContext> options): DbContext (options)
{

    public DbSet<Employee> Employee { get; set; }
    public DbSet<ProjectTask> ProjectTask { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=your_database.db");
        }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
            // настройка enum
            modelBuilder.Entity<Employee>()
                .Property(e => e.Department)
                .HasConversion<string>();

            modelBuilder.Entity<ProjectTask>()
                .Property(t => t.Status)
                .HasConversion<string>();
    }


}