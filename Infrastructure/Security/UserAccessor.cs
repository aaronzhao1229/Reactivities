using System.Security.Claims;
using Application.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Security
{
  public class UserAccessor : IUserAccessor
  {

    // we need this as we are not inside the context of our API project and we need to access the HTTP context and we can do it via the IHttpContextAccessor interface, because our HTTP context contains our user objects, and from our user objects we can get access to the claims inside the token.
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserAccessor(IHttpContextAccessor httpContextAccessor)
    {
      _httpContextAccessor = httpContextAccessor;
      
    }
    public string GetUsername()
    {
      return _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name);
    }
  }
}