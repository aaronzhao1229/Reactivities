using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Core;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Profiles
{
    public class Details
    {
        public class Query : IRequest<Result<Profile>>
        {
            public string Username {get; set;}  // Loggin users would be able to get other users profiles and we'll add root parameter onto our endpoint so that we can get the username that we're interested in returning here and then we can add our handler
        }

        public class Hander : IRequestHandler<Query, Result<Profile>>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;
            
            public Hander(DataContext context, IMapper mapper)
            {
                _mapper = mapper;
                _context = context;
            }

            public async Task<Result<Profile>> Handle(Query request, CancellationToken cancellationToken)
              {
                // we need to include the user's photos, and we could do that by eager loading, but we also know that we want to return a profile. so we would use ProjectTo() Profile
                var user = await _context.Users.ProjectTo<Profile>(_mapper.ConfigurationProvider).SingleOrDefaultAsync(x => x.Username == request.Username);

                if (user == null) return null;

                return Result<Profile>.Success(user);
              }
          }
  }
}