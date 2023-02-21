using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Core;
using Application.Interfaces;
using Domain;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Photos
{
    public class Add
    {
        public class Command : IRequest<Result<Photo>>
        {
           public IFormFile File { get; set; } // this File name is important
        }

    public class Handler : IRequestHandler<Command, Result<Photo>>
    {
      private readonly DataContext _context;
     private readonly IPhotoAccessor _photoAccessor;
    private readonly IUserAccessor _userAccessor;
      public Handler(DataContext context, IPhotoAccessor photoAccessor, IUserAccessor userAccessor)
      {
        _userAccessor = userAccessor;
        _photoAccessor = photoAccessor;
        _context = context;
      }

      public async Task<Result<Photo>> Handle(Command request, CancellationToken cancellationToken)
      {
        // get user from database, as only the currently loggedIn user can add photos to that collection
        // eager loading with Include because we need to get access to the user's photo collection when we're doing this.
        var user = await _context.Users.Include(p => p.Photos).FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());

        if (user == null) return null;

        var photoUploadResult = await _photoAccessor.AddPhoto(request.File); // If this fails, ten the logic inside our photoAccessor is going to throw an exception, so we don't need to check exception

        var photo = new Photo
        {
            Url = photoUploadResult.Url,
            Id = photoUploadResult.PublicId
        };

        // check if the use has any photos already set to Main. If not, then this is the first photo that user is uploading. We will take the oportunity to set is be their main photo if they do not have any photos.
        if (!user.Photos.Any(x => x.IsMain)) photo.IsMain = true;

        user.Photos.Add(photo);

        var result = await _context.SaveChangesAsync() > 0;

        if (result) return Result<Photo>.Success(photo);

        return Result<Photo>.Failure("Problem adding photo");

      }
    }
  }
}