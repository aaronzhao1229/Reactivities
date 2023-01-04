using Domain;
using MediatR;
using Persistence;

namespace Application.Acitivites
{
  public class Create
    {
        public class Command : IRequest  // command doesn't return a value, while Query returns a value
        {
          public Activity Activity { get; set; }
        }

    public class Handler : IRequestHandler<Command>
    {
    private readonly DataContext _context;
      public Handler(DataContext context)
      {
          _context = context;
      }

      public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
      {
        _context.Activities.Add(request.Activity); // adding the activity to the Activities in _context in memory, but not the database, so no need to add Async.
        await _context.SaveChangesAsync();
        return Unit.Value; // (basically return nothing)
      }
    }
  }
}