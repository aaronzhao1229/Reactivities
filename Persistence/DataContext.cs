using Domain;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


namespace Persistence
{
  public class DataContext : IdentityDbContext<AppUser>
  {
    public DataContext(DbContextOptions options) : base(options)
    {
    }

 // represent the tables we are going to create
    public DbSet<Activity> Activities{ get; set; }
  }
}