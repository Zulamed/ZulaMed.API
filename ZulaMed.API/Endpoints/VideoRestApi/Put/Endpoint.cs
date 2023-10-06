using FastEndpoints;
using Mediator;
using Microsoft.EntityFrameworkCore;
using OneOf.Types;
using Vogen;
using ZulaMed.API.Data;
using ZulaMed.API.Domain.Video;

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
            var rows = await _dbContext.Set<Video>().Where(x => x.Id == command.Id)
                .ExecuteUpdateAsync(calls => calls
                        .SetProperty(x => x.VideoTitle, (VideoTitle)command.VideoTitle)
                        .SetProperty(x => x.VideoThumbnail, (VideoThumbnail)command.VideoThumbnail)
                        .SetProperty(x => x.VideoDescription, (VideoDescription)command.VideoDescription)
                        .SetProperty(x => x.VideoUrl, (VideoUrl)command.VideoUrl),
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
        AllowAnonymous();
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var result = await _mediator.Send(req.MapToCommand(), ct);
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