using Amazon.S3;
using Amazon.S3.Model;
using Mediator;
using Microsoft.Extensions.Options;

namespace ZulaMed.VideoConversion.Features.Transcode.Events;

public class MasterFileGeneratedEvent : INotification
{
    public required string MasterFilePath { get; init; }
    
    public required string VideoKey { get; init; }
}


public class MasterFileGeneratedEventHandler : INotificationHandler<MasterFileGeneratedEvent>
{
    private readonly IAmazonS3 _s3;
    private readonly IOptions<S3BucketOptions> _options;

    public MasterFileGeneratedEventHandler(IAmazonS3 s3, IOptions<S3BucketOptions> options)
    {
        _s3 = s3;
        _options = options;
    }
    
    
    public async ValueTask Handle(MasterFileGeneratedEvent notification, CancellationToken cancellationToken)
    {
        var request = new PutObjectRequest()
        {
            BucketName = _options.Value.BucketNameConverted,
            Key = $"{notification.VideoKey}/master.m3u8",
            FilePath = notification.MasterFilePath
        };
        await _s3.PutObjectAsync(request, cancellationToken);
    }
}