using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Core;
using Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Photos
{
    public class Delete
    {
        public class Command : IRequest<Result<Unit>> // no need to return anything
        {
           public string Id { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result<Unit>>
        {
          private readonly DataContext _context;
          private readonly IPhotoAccessor _photoAccessor;
          private readonly IUserAccessor _userAccessor;
          public Handler(DataContext context, IPhotoAccessor photoAccessor, IUserAccessor userAccessor) // we need three things here: we need to get our use's object, get access to the PhotoAccessor and update our database
          {
            _userAccessor = userAccessor;
            _photoAccessor = photoAccessor;
            _context = context;
          }

          public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken) // we need to get our user object, including the photos collection
              {
                var user = await _context.Users.Include(p => p.Photos).FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername()); // get the user from database including photos collection

                if (user == null) return null;

                var photo = user.Photos.FirstOrDefault(x => x.Id == request.Id); // get the photo we want to delete. we don't use async here, as we've already retieved our user from the database above and in memory we should have access to their photos

                if (photo == null) return null;

                if (photo.IsMain) return Result<Unit>.Failure("You cannot delete your main photo"); // check if it is the main photo

                var result = await _photoAccessor.DeletePhoto(photo.Id);

                if (result == null) return Result<Unit>.Failure("Problem deleting photo from Cloudinary");

                user.Photos.Remove(photo);

                var success = await _context.SaveChangesAsync() > 0;

                if (success) return Result<Unit>.Success(Unit.Value);

                return Result<Unit>.Failure("Problem deleting photo from API ");
              }
        }
  }
}