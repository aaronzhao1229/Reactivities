

namespace Domain
{
  public class Activity
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public string Category{ get; set; }
        public string City { get; set; }
        public string Venue { get; set; }
        public bool IsCancelled { get; set; }

        public ICollection<ActivityAttendee> Attendees { get; set; } = new List<ActivityAttendee>(); // make sure that we don't get a no reference when we try and add something to this collection

    }
}