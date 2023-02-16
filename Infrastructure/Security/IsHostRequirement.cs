using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Infrastructure.Security
{
    public class IsHostRequirement : IAuthorizationRequirement
    {
        
    }

    public class IsHostRequirementHandler : AuthorizationHandler<IsHostRequirement>
      {
         // what we need to do is get access to the roots ID of the activity that we are trying to access because we need to get the acitivty ID so that we can check the attendees of the activity from our activity attendees joined table and see if this particular user is the host of that activity
        private readonly DataContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public IsHostRequirementHandler(DataContext dbContext, IHttpContextAccessor httpContextAccessor)
        {
          _httpContextAccessor = httpContextAccessor;
          _dbContext = dbContext;

        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, IsHostRequirement requirement)
          {
            var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null) return Task.CompletedTask;

            var activityId = Guid.Parse(_httpContextAccessor.HttpContext?.Request.RouteValues.SingleOrDefault(x => x.Key == "id").Value?.ToString());

            // var attendee = _dbContext.ActivityAttendees.FindAsync(userId,activityId).Result;  // this line of code, when we are getting our attendee objects from entity framework, this(FindAsync) is tracking the entity that we are getting and this stays in memory, even though our handler will have been disposed of because it's transient, it doesn't mean that the entity that we've obtained from entity framework is also going to be disposed. This is staying in memory and it's causing a problem when we're editing an activity because we're only sending up the activity object and in our edit class, we're not getting the related entity and we've got an activityattendees object inside memory for that particular activity. It's that combination of things that is making our activityattendees disppear from the list. So what we want from this is we need the attendee object. but we don't want an entity framework to track this in memory.
            var attendee = _dbContext.ActivityAttendees.AsNoTracking().SingleOrDefaultAsync(x => x.AppUserId == userId && x.ActivityId == activityId).Result;

            if (attendee == null) return Task.CompletedTask;

            if (attendee.IsHost) context.Succeed(requirement);

            return Task.CompletedTask; // if we return at this point and  the context succeed flag is set, then the user would be authorized to go ahead and edit the activity 
          }
      }
}