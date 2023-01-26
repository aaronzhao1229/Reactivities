using Microsoft.AspNetCore.Mvc;
using MediatR;
using Application.Core;

namespace API.Controllers
{
  [ApiController]  // makes model validation errors automatically trigger an HTTP 400 response
  [Route("api/[controller]")]
    public class BaseApiController : ControllerBase
    {
        private IMediator _mediator;

        // just in case, if _mediator exist, use it; if not exisit, use HttpContext
        protected IMediator Mediator => _mediator ??= HttpContext.RequestServices.GetService<IMediator>();

        protected ActionResult HandleResult<T>(Result<T> result) 
        {
          if (result == null) return NotFound();
          if (result.IsSuccess && result.Value != null) return Ok(result.Value);
          if (result.IsSuccess && result.Value == null)
          return NotFound();
          return BadRequest(result.Error);
        }
    }
}