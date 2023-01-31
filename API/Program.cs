using Microsoft.EntityFrameworkCore;
using Persistence;
using API.Extensions;
using API.Middleware;
using Microsoft.AspNetCore.Identity;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;

// create a server
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers(opt => 
{
  var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
  opt.Filters.Add(new AuthorizeFilter(policy));
});
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddIdentityServices(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline (middleware)
app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// cors policy again but need to be before UseAuthorization()
app.UseCors("CorsPolicy");

app.UseAuthentication(); // always need to be before UseAuthorization
app.UseAuthorization();

app.MapControllers();

// create database, migrate and seed
using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;

try
{
  var context = services.GetRequiredService<DataContext> ();
  var userManager = services.GetRequiredService<UserManager<AppUser>>();
  await context.Database.MigrateAsync();
  await Seed.SeedData(context, userManager);
}
catch (Exception ex)
{
  var logger = services.GetRequiredService<ILogger<Program>>();
  logger.LogError(ex, "An error occured during migration");
}


app.Run();
