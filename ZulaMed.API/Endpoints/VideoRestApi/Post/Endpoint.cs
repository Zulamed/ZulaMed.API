using FastEndpoints;
using Mediator;
using OneOf.Types;
using Vogen;
using ZulaMed.API.Data;
using ZulaMed.API.Domain.Video;
using VoException = Vogen.ValueObjectValidationException;

namespace ZulaMed.API.Endpoints.VideoRestApi.Post;

public class CreateVideoCommandHandler 
    : Mediator.ICommandHandler<CreateVideoCommand, Result<Video, VoException>>
{
    private readonly ZulaMedDbContext _dbContext;

    public CreateVideoCommandHandler(ZulaMedDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async ValueTask<Result<Video, ValueObjectValidationException>> Handle(CreateVideoCommand command,
        CancellationToken cancellationToken)
    {
        var dbSet = _dbContext.Set<Video>();
        try
        {
            var entity = await dbSet.AddAsync(new Video()
            {
                Id = (VideoId)Guid.NewGuid(),
                VideoDescription = (VideoDescription)command.VideoDescription,
                VideoPublishedDate = (VideoPublishedDate)DateTime.UtcNow,
                VideoThumbnail = (VideoThumbnail)command.VideoThumbnail,
                VideoTitle = (VideoTitle)command.VideoTitle,
                VideoUrl = (VideoUrl)command.VideoUrl 
            }, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return entity.Entity;
        }
        catch (VoException e)
        {
            return new Error<VoException>(e);
        }
    }
}

public class Endpoint : Endpoint<Request, VideoDTO>
{
    private readonly IMediator _mediator;

    public Endpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Post("/video");
        AllowAnonymous();
    }

    public override async Task HandleAsync(Request request, CancellationToken ct)
    {
        var result = await _mediator.Send(request.MapToCommand(), ct);
        if (result.TryPickT0(out var value, out var error))
        {
            await SendOkAsync(value.MapToResponse(), ct);
            return;
        }
        AddError(error.Value.Message);
        await SendErrorsAsync(cancellation: ct);
    }
}