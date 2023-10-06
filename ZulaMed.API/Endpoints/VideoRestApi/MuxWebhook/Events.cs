using Mediator;
using Microsoft.EntityFrameworkCore;
using OneOf;
using OneOf.Types;
using ZulaMed.API.Data;
using ZulaMed.API.Domain.Video;

namespace ZulaMed.API.Endpoints.VideoRestApi.MuxWebhook;

public class AssetCreatedEvent : IRequest<OneOf<Success, Error>>
{
    public required Guid VideoId { get; set; }
}

public class AssetCreatedEventHandler : IRequestHandler<AssetCreatedEvent, OneOf<Success, Error>>
{
    private readonly ZulaMedDbContext _dbContext;

    public AssetCreatedEventHandler(ZulaMedDbContext dbContext)
    {
        _dbContext = dbContext;
    }


    public async ValueTask<OneOf<Success, Error>> Handle(AssetCreatedEvent request, CancellationToken cancellationToken)
    {
        try
        {
            var rows = await _dbContext.Set<Video>()
                .Where(x => (Guid)x.Id == request.VideoId)
                .ExecuteUpdateAsync(calls =>
                        calls.SetProperty(x => x.VideoStatus, VideoStatus.GettingProcessed),
                    cancellationToken: cancellationToken);
            return new Success();
        }
        catch (Exception)
        {
            return new Error();
        }
    }
}

public class AssetReadyEvent : IRequest<OneOf<Success, Error>>
{
    public required Guid VideoId { get; set; }
}

public class AssetReadyEventHandler : IRequestHandler<AssetReadyEvent, OneOf<Success, Error>>
{
    private readonly ZulaMedDbContext _dbContext;

    public AssetReadyEventHandler(ZulaMedDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async ValueTask<OneOf<Success, Error>> Handle(AssetReadyEvent request, CancellationToken cancellationToken)
    {
        try
        {
            await _dbContext.Set<Video>()
                .Where(x => (Guid)x.Id == request.VideoId)
                .ExecuteUpdateAsync(calls =>
                        calls.SetProperty(x => x.VideoStatus, VideoStatus.Ready),
                    cancellationToken: cancellationToken);
            return new Success();
        }
        catch (Exception)
        {
            return new Error();
        }
    }
}

public class UploadCanceledEvent : IRequest<OneOf<Success, Error>>
{
    public required Guid VideoId { get; set; }
}

public class UploadCanceledEventHandler : IRequestHandler<UploadCanceledEvent, OneOf<Success, Error>>
{
    private readonly ZulaMedDbContext _dbContext;

    public UploadCanceledEventHandler(ZulaMedDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async ValueTask<OneOf<Success, Error>> Handle(UploadCanceledEvent request,
        CancellationToken cancellationToken)
    {
        try
        {
            await _dbContext.Set<Video>()
                .Where(x => (Guid)x.Id == request.VideoId)
                .ExecuteUpdateAsync(calls =>
                        calls.SetProperty(x => x.VideoStatus, VideoStatus.Cancelled),
                    cancellationToken: cancellationToken);
            return new Success();
        }
        catch (Exception)
        {
            return new Error();
        }
    }
}