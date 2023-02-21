using Application.Core;
using Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Photos
{
  public class SetMain
    {
        public class Command : IRequest<Result<Unit>>
        {
           public string Id { get; set; } 
        }

        public class Handler : IRequestHandler<Command, Result<Unit>>
        {
          private readonly DataContext _context;
          private readonly IUserAccessor _userAccessor;

          // going to access the current user's username from the token and access to our context so we can update the database here
          public Handler(DataContext context, IUserAccessor userAccessor)
          {
              _userAccessor = userAccessor;
              _context = context;
          }

          
          public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
              {
                var user = await _context.Users.Include(p => p.Photos).FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername()); // get the user from database including photos collection

                if (user == null) return null;

                var photo = user.Photos.FirstOrDefault(x => x.Id == request.Id); // get the photo we want to delete. we don't use async here, as we've already retieved our user from the database above and in memory we should have access to their photos

                if (photo == null) return null;

                var currentMain = user.Photos.FirstOrDefault(x => x.IsMain);

                if (currentMain != null) currentMain.IsMain = false;

                photo.IsMain = true;

                // update the database
                var success = await _context.SaveChangesAsync() > 0;

                if (success) return Result<Unit>.Success(Unit.Value);

                return Result<Unit>.Failure("Problem setting main photo");
              }
        }
  }
}