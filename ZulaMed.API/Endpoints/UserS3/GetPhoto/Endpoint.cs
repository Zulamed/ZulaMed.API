using Amazon.S3;
using Amazon.S3.Model;
using FastEndpoints;
using Mediator;
using Microsoft.Extensions.Options;

namespace ZulaMed.API.Endpoints.UserS3.GetPhoto;


public class GetUserPhotoHandler : IQueryHandler<GetUserPhotoQuery, Response>
{
    private readonly IOptions<S3BucketOptions> _s3Options;

    public GetUserPhotoHandler(IOptions<S3BucketOptions> s3Options)
    {
        _s3Options = s3Options;
    }
    
    public ValueTask<Response> Handle(GetUserPhotoQuery query, CancellationToken cancellationToken)
    {
        return ValueTask.FromResult(new Response
        {
            PhotoUrl = _s3Options.Value.BaseUrl + $"users/images/{query.UserId}",
        });
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
        Put("/user/{id}/photo");
        AllowAnonymous();
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var response = await _mediator.Send(new GetUserPhotoQuery
        {
            UserId = req.Id
        }, cancellationToken: ct);
        
        await SendAsync(new Response()
        {
            PhotoUrl = response.PhotoUrl
        }, cancellation: ct);
    }
}