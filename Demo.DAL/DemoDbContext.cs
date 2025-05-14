using Demo.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace Demo.DAL;

public class DemoDbContext: DbContext
{
    public DemoDbContext(DbContextOptions<DemoDbContext> options) : base(options) { }

    public DbSet<Product> Products { get; set; }
}