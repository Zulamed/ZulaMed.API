using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Options;
using ZulaMed.VideoConversion.Infrastructure;

namespace ZulaMed.VideoConversion.Endpoints.Transcode.Queries;

public class GetVideoFromS3Query : IQuery<GetObjectResponse>
{
    public required string Key { get; set; }
}


public class GetVideoFromS3QueryHandler : IQueryHandler<GetVideoFromS3Query, GetObjectResponse>
{
    private readonly IAmazonS3 _s3Client;
    private readonly IOptions<S3BucketOptions> _s3Options;

    public GetVideoFromS3QueryHandler(IAmazonS3 s3Client, IOptions<S3BucketOptions> s3Options)
    {
        _s3Client = s3Client;
        _s3Options = s3Options;
    }

    public async Task<GetObjectResponse> HandleAsync(GetVideoFromS3Query query, CancellationToken cancellationToken)
    {
        var request = new GetObjectRequest
        {
            BucketName = _s3Options.Value.BucketName, 
            Key = query.Key
        };
        return await _s3Client.GetObjectAsync(request, cancellationToken);
    }
}