using System.Text;
using API.Services;
using Domain;
using Infrastructure.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Persistence;
using Microsoft.AspNetCore.Authorization;

namespace API.Extensions
{
  public static class IdentityServiceExtensions
    {
        public static IServiceCollection AddIdentityServices(this IServiceCollection services, IConfiguration config) 
        {
            services.AddIdentityCore<AppUser>(opt => {
              opt.Password.RequireNonAlphanumeric = false;
              opt.User.RequireUniqueEmail = true; // only works after a user is created

              // there is no method in opt.User to check if username exists and the logic to check this is in AcountController
            })
            .AddEntityFrameworkStores<DataContext>();

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"]));

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(opt => {
              opt.TokenValidationParameters = new TokenValidationParameters
              {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = key,
                ValidateIssuer = false,
                ValidateAudience = false  // we cannot validate against these things, but we would need to add them to the token that we are issuing
              };
            });

            services.AddAuthorization(opt => 
            {
                opt.AddPolicy("IsActivityHost", policy => {
                  policy.Requirements.Add(new IsHostRequirement());
                });
            });
            services.AddTransient<IAuthorizationHandler, IsHostRequirementHandler>(); // with this in place, we can use is attributes on our endpoints to add policies. Then go to activities controller do more work


            services.AddScoped<TokenService>(); // this token service is going to be scoped to the HTTP request itself. When the HTTP request comes in, we go to our account controller and request a token, then out token service is a new instance of our token service will be created and we need to add that in the <> after AddScopted. When the HTTP request is finished, then we'll dispose of the token service.

            return services;
        }
         
    }
}