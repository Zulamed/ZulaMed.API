using FastEndpoints;
using Mediator;

namespace ZulaMed.API.Endpoints.Users.Get;


public class SayHelloCommand : Mediator.ICommand<string>
{
   public string Message { get; set; } 
}

public class SayHelloCommandHandler : Mediator.ICommandHandler<SayHelloCommand, string>
{
    public ValueTask<string> Handle(SayHelloCommand command, CancellationToken cancellationToken)
    {
        return ValueTask.FromResult(command.Message);
    }
} 

public class Get : Endpoint<Request, Response>
{
    private readonly IMediator _mediator;

    public Get(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    public override void Configure()
    {
        Get("/salam");
        AllowAnonymous();
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
    }
}