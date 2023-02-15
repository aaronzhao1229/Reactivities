using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Acitivites;
using Application.Core;
using Application.Interfaces;
using FluentValidation;
using FluentValidation.AspNetCore;
using Infrastructure.Security;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace API.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
        {
          // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            services.AddDbContext<DataContext> (opt => 
            {
              opt.UseSqlite(config.GetConnectionString("DefaultConnection"));
            });


            // allow cors
            services.AddCors(opt => {
              opt.AddPolicy("CorsPolicy", policy => {
                policy.AllowAnyMethod().AllowAnyHeader().WithOrigins("http://localhost:3000");
              });
            });

            // ADD MediatR
            services.AddMediatR(typeof (List.Handler));

            // Add AutoMapper as a service
            services.AddAutoMapper(typeof(MappingProfiles).Assembly);

            // Add Fluent Validation
            services.AddFluentValidationAutoValidation();
            services.AddValidatorsFromAssemblyContaining<Create>();

            // Add IUserAccessor service
            services.AddHttpContextAccessor();
            services.AddScoped<IUserAccessor, UserAccessor>(); // this will make this available to be injected inside our application handlers.
            
            return services;
        } 
    }
}