using Amazon.S3;
using Amazon.S3.Model;
using Mediator;
using Microsoft.Extensions.Options;

namespace ZulaMed.VideoConversion.Features.Transcode.Events;

public class VideoTranscodedEvent : INotification
{
    public required string VideoDirectoryPath { get; init; }

    public required string VideoKey { get; init; }
}

// upload to S3
public class VideoTranscodedEventHandler : INotificationHandler<VideoTranscodedEvent>
{
    private readonly IAmazonS3 _s3;
    private readonly IOptions<S3BucketOptions> _s3Options;

    

    public VideoTranscodedEventHandler(IAmazonS3 s3, IOptions<S3BucketOptions> s3Options)
    {
        _s3 = s3;
        _s3Options = s3Options;
    }

    public async ValueTask Handle(VideoTranscodedEvent notification, CancellationToken cancellationToken)
    {
        var directory = Directory.GetFiles(notification.VideoDirectoryPath);
        var tasks = (
            from file in directory let fileName = Path.GetFileNameWithoutExtension(file) 
            let extension = Path.GetExtension(file) 
            select UploadToS3(notification, fileName, extension, cancellationToken))
            .ToList();
        await Task.WhenAll(tasks);
    }


    private async Task UploadToS3(VideoTranscodedEvent notification, string fileName, string extension, CancellationToken token)
    {
        // need to add retry with Polly
        var path = $"{notification.VideoDirectoryPath}{fileName}{extension}";
        var request = new PutObjectRequest
        {
            BucketName = _s3Options.Value.BucketNameConverted,
            Key = path,
            FilePath = $"{notification.VideoDirectoryPath}/{fileName}{extension}"
        };
        await _s3.PutObjectAsync(request, token);
    }
}