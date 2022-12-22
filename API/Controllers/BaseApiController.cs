using Microsoft.AspNetCore.Mvc;
using MediatR;
namespace API.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
    public class BaseApiController : ControllerBase
    {
        private IMediator _mediator;

        // just in case, if _mediator exist, use it; if not exisit, use HttpContext
        protected IMediator Mediator => _mediator ??= HttpContext.RequestServices.GetService<IMediator>();
    }
}