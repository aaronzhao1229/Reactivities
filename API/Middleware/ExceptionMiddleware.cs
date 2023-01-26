using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Application.Core;

namespace API.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        
        private readonly IHostEnvironment _env;
        private readonly ILogger<ExceptionMiddleware> _logger;
        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env) {
          _logger = logger;
          _next = next;
          _env = env;
        }

       // the name has to be InvokeAsync
        public async Task InvokeAsync(HttpContext context) {
      try
      {
        await _next(context);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, ex.Message);
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError; // will be 500

        var response = _env.IsDevelopment() ? new AppException(context.Response.StatusCode, ex.Message, ex.StackTrace?.ToString()) : new AppException(context.Response.StatusCode, "Internal Server Error"); // give different info under development or production

        var options = new JsonSerializerOptions{PropertyNamingPolicy = JsonNamingPolicy.CamelCase};  // something that our API controllers enabled by default becasue that's how we format JSON when we return it. But outside API controller we need to specify this by ourselves.

        var json = JsonSerializer.Serialize(response, options);

        await context.Response.WriteAsync(json);
        
      }
    }
    }

    
}