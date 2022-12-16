using Persistence;
using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
namespace API.Controllers
{
  public class ActivitiesController : BaseApiController
    {
        
        private readonly DataContext _context;
   
        public ActivitiesController(DataContext context)
        {
            _context = context;
          
        }
        [HttpGet] //api/activities
        public async Task<ActionResult<List<Activity>>> GetActivities() {
          return await _context.Activities.ToListAsync();
        }

        [HttpGet("{id}")] //api/activities/{id}
        public async Task<ActionResult<Activity>> GetActivity(Guid id) {
          return await _context.Activities.FindAsync(id);
        }
    }
}