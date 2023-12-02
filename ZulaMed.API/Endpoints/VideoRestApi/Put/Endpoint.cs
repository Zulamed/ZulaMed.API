using System.Linq.Expressions;
using FastEndpoints;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using OneOf.Types;
using Vogen;
using ZulaMed.API.Data;
using ZulaMed.API.Domain.Video;
using ZulaMed.API.Extensions;

namespace ZulaMed.API.Endpoints.VideoRestApi.Put;

public class UpdateVideoCommandHandler : Mediator.ICommandHandler<UpdateVideoCommand,
    Result<bool, ValueObjectValidationException>>
{
    private readonly ZulaMedDbContext _dbContext;

    public UpdateVideoCommandHandler(ZulaMedDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async ValueTask<Result<bool, ValueObjectValidationException>> Handle(UpdateVideoCommand command,
        CancellationToken cancellationToken)
    {
        try
        {
            Expression<Func<SetPropertyCalls<Video>, SetPropertyCalls<Video>>> set =
                calls => calls;
            if (!string.IsNullOrWhiteSpace(command.VideoDescription))
            {
                set = SetPropertyCallsExtensions.AppendSetProperty(set, s => s
                    .SetProperty(video => video.VideoDescription, (VideoDescription)command.VideoDescription));
            }

            set = SetPropertyCallsExtensions.AppendSetProperty(set, s => s
                .SetProperty(video => video.VideoTitle, (VideoTitle)command.VideoTitle));

            var rows = await _dbContext.Set<Video>()
                .Where(x => (Guid)x.Id == command.Id && command.UserId == (Guid)x.Publisher.Id)
                .ExecuteUpdateAsync(set,
                    cancellationToken);
            return rows > 0;
        }
        catch (ValueObjectValidationException e)
        {
            return new Error<ValueObjectValidationException>(e);
        }
    }
}

public class Endpoint : Endpoint<Request>
{
    private readonly IMediator _mediator;

    public Endpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Put("/video/{id}");
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var userId = Guid.Parse(HttpContext.User.Claims.FirstOrDefault(x => x.Type == "UserId")!.Value);
        var result = await _mediator.Send(new UpdateVideoCommand
        {
            Id = req.Id,
            UserId = userId,
            VideoTitle = req.VideoTitle,
            VideoDescription = req.VideoDescription,
        }, ct);
        if (result.TryPickT0(out var isUpdated, out var error))
        {
            if (!isUpdated)
            {
                await SendNotFoundAsync(ct);
                return;
            }

            await SendOkAsync(ct);
            return;
        }

        AddError(error.Value.Message);
        await SendErrorsAsync(cancellation: ct);
    }
}