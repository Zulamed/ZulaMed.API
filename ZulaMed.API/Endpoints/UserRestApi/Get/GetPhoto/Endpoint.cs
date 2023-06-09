using Amazon.S3;
using FastEndpoints;
using Mediator;
using Microsoft.Extensions.Options;

namespace ZulaMed.API.Endpoints.UserRestApi.Get.GetPhoto;


public class GetUserPhotoHandler : IQueryHandler<GetUserPhotoQuery, Response>
{
    private readonly IAmazonS3 _s3Client;
    private readonly IOptions<S3BucketOptions> _s3Options;

    public GetUserPhotoHandler(IAmazonS3 s3Client, IOptions<S3BucketOptions> s3Options)
    {
        _s3Client = s3Client;
        _s3Options = s3Options;
    }
    
    public async ValueTask<Response> Handle(GetUserPhotoQuery query, CancellationToken cancellationToken)
    {
        //var photoUrl = await _s3Client.
        
        throw new NotImplementedException();
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
        Put("/user/{id}/photo");
        AllowAnonymous();
    }
}