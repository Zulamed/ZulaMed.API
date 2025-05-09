using System.Drawing;
using System.Net;
using Amazon.S3;
using Amazon.S3.Model;
using FFMpegCore;
using Mediator;
using Microsoft.Extensions.Options;
using Polly;

namespace ZulaMed.VideoConversion.Features.Transcode.Events;

public class VideoTranscodedEvent : INotification
{
    public required string VideoDirectoryPath { get; init; }

    public required string VideoNameWithExtension { get; init; }
}

public class VideoTranscodedThumbnailHandler
    : INotificationHandler<VideoTranscodedEvent>
{
    private readonly IAmazonS3 _s3Client;
    private readonly IOptions<S3BucketOptions> _options;

    public VideoTranscodedThumbnailHandler(IAmazonS3 s3Client, IOptions<S3BucketOptions> options)
    {
        _s3Client = s3Client;
        _options = options;
    }

    public async ValueTask Handle(VideoTranscodedEvent notification, CancellationToken cancellationToken)
    {
        var inputPath = $"{notification.VideoDirectoryPath}/{notification.VideoNameWithExtension}";
        var outputPath = Path.Combine(notification.VideoDirectoryPath, "thumbnail.png");
        var success = await FFMpeg.SnapshotAsync(inputPath, outputPath, new Size(1920, 1080), TimeSpan.FromSeconds(1));
        if (success)
        {
            await _s3Client.PutObjectAsync(new PutObjectRequest
            {
                BucketName = _options.Value.BucketNameConverted,
                Key = outputPath,
                FilePath = outputPath
            }, cancellationToken);
        }
    }
}

public class VideoTranscodedUploadToS3Handler : INotificationHandler<VideoTranscodedEvent>
{
    private readonly IAmazonS3 _s3;
    private readonly IOptions<S3BucketOptions> _s3Options;


    public VideoTranscodedUploadToS3Handler(IAmazonS3 s3, IOptions<S3BucketOptions> s3Options)
    {
        _s3 = s3;
        _s3Options = s3Options;
    }

    public async ValueTask Handle(VideoTranscodedEvent notification, CancellationToken cancellationToken)
    {
        var directory = Directory.GetFiles(notification.VideoDirectoryPath,
            "*", SearchOption.AllDirectories);
        var tasks = (
                from file in directory
                select RetryUpload(notification, file, cancellationToken))
            .ToList();
        await Task.WhenAll(tasks);
    }

    private async Task RetryUpload(VideoTranscodedEvent notification, string file, CancellationToken cancellationToken)
    {
        if (notification.VideoNameWithExtension == Path.GetFileName(file))
            return;
        await Policy.HandleResult<HttpStatusCode>(r => HttpStatusCode.OK != r)
            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)))
            .ExecuteAsync(async () => await UploadToS3(notification, file, cancellationToken));
    }


    private async Task<HttpStatusCode> UploadToS3(VideoTranscodedEvent notification, string fileName,
        CancellationToken token)
    {
        var request = new PutObjectRequest
        {
            BucketName = _s3Options.Value.BucketNameConverted,
            Key = fileName,
            FilePath = fileName
        };
        var response = await _s3.PutObjectAsync(request, token);
        return response.HttpStatusCode;
    }
}