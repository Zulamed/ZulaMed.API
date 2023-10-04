using FastEndpoints;
using Mediator;
using OneOf;
using OneOf.Types;

namespace ZulaMed.API.Endpoints.VideoRestApi.Upload;

public class
    UploadVideoCommandHandler : IQueryHandler<GetUploadUrlQuery, OneOf<Success<Uri>, Error<Exception>>>
{
    private readonly IMuxClientApi _muxClient;

    public UploadVideoCommandHandler(IMuxClientApi muxClient)
    {
        _muxClient = muxClient;
    }


    public async ValueTask<OneOf<Success<Uri>, Error<Exception>>> Handle(GetUploadUrlQuery command,
        CancellationToken cancellationToken)
    {
        using var response = await _muxClient.GetUploadUrl(new MuxRequestBody()
        {
            CorsOrigin = "*",
            NewAssetSettings = new NewAssetSettings
            {
                PlaybackPolicy = new[] { "public" },
                MaxResolutionTier = "2160p"
            }
        });
        if (!response.IsSuccessStatusCode)
        {
            return new Error<Exception>(new Exception("Failed to get upload link from CloudFlare"));
        }

        var uploadUrl = response.Content?.Data.Url;

        if (uploadUrl is null)
        {
            return new Error<Exception>(new InvalidDataException("Upload link was null"));
        }

        return new Success<Uri>(new Uri(uploadUrl));
    }
}

public class UploadEndpoint : EndpointWithoutRequest
{
    private readonly IMediator _mediator;

    public UploadEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }


    public override void Configure()
    {
        Post("/video/upload");
        AllowAnonymous(); 
    }

    public override async Task HandleAsync(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetUploadUrlQuery(), cancellationToken);

        await result.Match(success => SendOkAsync(new
            {
                Url = success.Value
            }, cancellationToken),
            error =>
            {
                AddError(error.Value.Message);
                return SendErrorsAsync(cancellation: cancellationToken);
            });
    }
}