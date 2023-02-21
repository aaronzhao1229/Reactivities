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

  // activityattendee configuration

    public DbSet<ActivityAttendee> ActivityAttendees { get; set; }  // for the occasion where we just want to get an attendee without needing to get the activity object as well

    public DbSet<Photo> Photos { get; set; }  // just in case we need to query the photo collection directly from our data context.

    protected override void OnModelCreating(ModelBuilder builder)
    {
      base.OnModelCreating(builder);

      builder.Entity<ActivityAttendee>(x => x.HasKey(aa => new {aa.AppUserId, aa.ActivityId})); // tell it about its primary key (make up a combination of app user id and activity id)

      builder.Entity<ActivityAttendee>().HasOne(u => u.AppUser).WithMany(a => a.Activities).HasForeignKey(aa=> aa.AppUserId);

      builder.Entity<ActivityAttendee>().HasOne(u => u.Activity).WithMany(a => a.Attendees).HasForeignKey(aa=> aa.ActivityId);
    }
  }
}