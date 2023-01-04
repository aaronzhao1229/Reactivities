using Domain;
using Microsoft.EntityFrameworkCore;

namespace Persistence
{
  public class DataContext : DbContext
  {
    public DataContext(DbContextOptions options) : base(options)
    {
    }

 // represent the tables we are going to create
    public DbSet<Activity> Activities{ get; set; }
  }
}