using System.Text.Json;
using Amazon.S3;
using Amazon.S3.Model;
using FastEndpoints;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Mux.Csharp.Sdk.Api;
using Mux.Csharp.Sdk.Model;
using OneOf.Types;
using Vogen;
using ZulaMed.API.Data;
using ZulaMed.API.Domain.User;
using ZulaMed.API.Domain.Video;
using VoException = Vogen.ValueObjectValidationException;

namespace ZulaMed.API.Endpoints.VideoRestApi.Post;

public class CreateVideoCommandHandler
    : Mediator.ICommandHandler<CreateVideoCommand, Result<Response, VoException>>
{
    private readonly ZulaMedDbContext _dbContext;
    private readonly DirectUploadsApi _muxUploadClient;

    public CreateVideoCommandHandler(ZulaMedDbContext dbContext, DirectUploadsApi muxUploadClient)
    {
        _dbContext = dbContext;
        _muxUploadClient = muxUploadClient;
    }

    public async ValueTask<Result<Response, ValueObjectValidationException>> Handle(CreateVideoCommand command,
        CancellationToken cancellationToken)
    {
        var dbSet = _dbContext.Set<Video>();
        var guid = Guid.NewGuid();
        try
        {
            var user = await _dbContext.Set<User>().FirstOrDefaultAsync(x => (Guid)x.Id == command.VideoPublisherId,
                cancellationToken: cancellationToken);
            if (user is null)
            {
                return new Error<VoException>(new VoException("User not found"));
            }

            await dbSet.AddAsync(new Video
            {
                Id = (VideoId)guid,
                Publisher = user,
                VideoStatus = VideoStatus.WaitingForUpload,
                VideoPublishedDate = (VideoPublishedDate)DateTime.UtcNow
            }, cancellationToken);

            await _dbContext.SaveChangesAsync(cancellationToken);

            var newAssetSettings = new CreateAssetRequest
            {
                AdditionalProperties =
                {
                    ["max_resolution_tier"] = "2160p"
                },
                Passthrough = JsonSerializer.Serialize(new
                {
                    videoId = guid
                }),
                PlaybackPolicy = new List<PlaybackPolicy>
                {
                    PlaybackPolicy.Public
                }
            };
            var response = await _muxUploadClient.CreateDirectUploadAsync(
                new CreateUploadRequest(newAssetSettings: newAssetSettings)
                {
                    CorsOrigin = "*"
                }, cancellationToken);


            var uploadUrl = response.Data.Url;

            if (uploadUrl is null)
            {
                return new Error<VoException>(new VoException("Upload link was null"));
            }


            return new Response
            {
                UploadUrl = uploadUrl,
                Id = guid
            };
        }
        catch (VoException e)
        {
            return new Error<VoException>(e);
        }
    }
}

public class Endpoint : Endpoint<Request, Response>
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

        await result.Match(
            r => SendOkAsync(r, ct),
            e =>
            {
                AddError(e.Value.Message);
                return SendErrorsAsync(cancellation: ct);
            });
    }
}