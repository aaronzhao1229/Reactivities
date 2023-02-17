using API.DTOs;
using API.Services;
using Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace API.Controllers   // we don't want this one to be an authenticated
{
  // [AllowAnonymous]  // ensure all of endpoints in this class no longer need authentication. Even an [Authorize] attribute was added to any endpoint below, [AllowAnonymous] at this level will take the priority
  [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
    private readonly UserManager<AppUser> _userManager;

    private readonly TokenService _tokenService;
        public AccountController(UserManager<AppUser> userManager, TokenService tokenService)
        {
            _tokenService = tokenService;
            _userManager = userManager;
        }

        [AllowAnonymous] 
        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        { 
          // we need to include eager load the phots when we want to return a user DTO with an image. We don't have to do this for Resgister because there is no image for register, but we do need it for login.

          // var user = await _userManager.FindByEmailAsync(loginDto.Email);
          // there is no way to include or equally load if we're using one of the user managers find by methods. So we use the following
          var user = await _userManager.Users.Include(p => p.Photos).FirstOrDefaultAsync(x => x.Email == loginDto.Email); // we should have the photo available to return after a user logs in. We need to do the same for the getcurrentuser method.


          if (user == null) return Unauthorized();

          var result = await _userManager.CheckPasswordAsync(user, loginDto.Password);

          if (result)
          {
            return CreateUserObject(user);
          }

          return Unauthorized();
        }

        [AllowAnonymous] 
        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto) 
        {
            if (await _userManager.Users.AnyAsync(x => x.UserName == registerDto.UserName))
            {
              ModelState.AddModelError("userName", "UserName taken");
              return ValidationProblem();
            }

            if (await _userManager.Users.AnyAsync(x => x.Email == registerDto.Email))
            {
              ModelState.AddModelError("email", "Email taken");
              return ValidationProblem();
            }

            var user = new AppUser
            {
              DisplayName = registerDto.DisplayName,
              Email = registerDto.Email,
              UserName = registerDto.UserName
            };

            // use our user manager to create this user
            var result = await _userManager.CreateAsync(user, registerDto.Password);

            if (result.Succeeded)
      {
        return CreateUserObject(user);
      }

      return BadRequest(result.Errors);
        }

    

    [HttpGet]
        public async Task<ActionResult<UserDto>> GetCurrentUser() 
        {
          var user = await _userManager.Users.Include(p => p.Photos).FirstOrDefaultAsync(x => x.Email == User.FindFirstValue(ClaimTypes.Email));
          // var user = await _userManager.FindByEmailAsync(User.FindFirstValue(ClaimTypes.Email)); // get the email claim from the token that we present to the API server

          // create a new user DTO based on the information contained inside that user above
          return CreateUserObject(user);
        }
    private UserDto CreateUserObject(AppUser user)
    {
      return new UserDto
      {
        DisplayName = user.DisplayName,
        Image = user?.Photos?.FirstOrDefault(x => x.IsMain)?.Url, // adding these question marks should keep us safe from getting an error when we're trying to return the image.
        Token = _tokenService.CreateToken(user),
        Username = user.UserName
      };
    }
    }
}