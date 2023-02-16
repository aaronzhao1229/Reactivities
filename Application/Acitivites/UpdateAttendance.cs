using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Core;
using Application.Interfaces;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;


// three different purposes: for a user who is not the host, we hit this handler is going to revove them from the activity; if they are not going to the event, we hit this handler is going to join them to the activity; If they are the host, then it's going to cancel the activity.

namespace Application.Acitivites
{
    public class UpdateAttendance
    {
        public class Command : IRequest<Result<Unit>>
        {
            public Guid Id { get; set; }
        }

    public class Handler : IRequestHandler<Command, Result<Unit>>
    {
      private readonly DataContext _context;
      private readonly IUserAccessor _userAccessor;
      public Handler(DataContext context, IUserAccessor userAccessor)
      {
          _userAccessor = userAccessor;
         _context = context;
      }

      public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
      {
        var activity = await _context.Activities.Include(a => a.Attendees).ThenInclude(u => u.AppUser).SingleOrDefaultAsync(x => x.Id == request.Id);

        // SingleOrDefault will return an exception if they are more than one entity matches. But FirstOrDefault will return null.

        if (activity == null) return null;

        var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());

        if (user == null) return null;

        var hostUsername = activity.Attendees.FirstOrDefault(x => x.IsHost)?.AppUser?.UserName;

        var attendance = activity.Attendees.FirstOrDefault(x => x.AppUser.UserName == user.UserName); // to get the attendance status for this particular users, then we can add logic to decide what we're going to do based on the attendance and the hostUsername

        if (attendance != null && hostUsername == user.UserName) // if this is the host
            activity.IsCancelled = !activity.IsCancelled;

        if (attendance != null && hostUsername != user.UserName) // if this is the normal attendee
            activity.Attendees.Remove(attendance);

        if (attendance == null)
        {
            attendance = new ActivityAttendee 
            {
              AppUser = user,
              Activity = activity, 
              IsHost = false
            };

            activity.Attendees.Add(attendance);
        }

        var result = await _context.SaveChangesAsync() > 0;

        return result ? Result<Unit>.Success(Unit.Value) : Result<Unit>.Failure("Problem updating attendance ");
      }
    }
  }
}