using Amazon.S3;
using Amazon.S3.Model;
using FFMpegCore;
using FFMpegCore.Enums;
using Mediator;
using Microsoft.Extensions.Options;
using OneOf;
using OneOf.Types;

namespace ZulaMed.VideoConversion.Features.Transcode.Commands;

public class TranscodeVideoCommand : ICommand<OneOf<Success<string>, Error>>
{
    public required string VideoPath { get; init; }

    public required Resolution Resolution { get; init; }
}

public class VideoTranscodedEvent : INotification
{
    public required string VideoPath { get; init; }

    public required string VideoKey { get; init; }
}

public class TranscodeVideoCommandHandler : ICommandHandler<TranscodeVideoCommand, OneOf<Success<string>, Error>>
{
    private readonly IPublisher _publisher;

    public TranscodeVideoCommandHandler(IPublisher publisher)
    {
        _publisher = publisher;
    }

    public async ValueTask<OneOf<Success<string>, Error>> Handle(TranscodeVideoCommand command, CancellationToken token)
    {
        try
        {
            await ProcessArguments(command, command.Resolution);
            var key = $"output-{command.Resolution.Width}x{command.Resolution.Height}.mp4";
            await _publisher.Publish(new VideoTranscodedEvent()
            {
                VideoKey = key,
                VideoPath = key
            }, token);
            return new Success<string>(key);
        }
        catch (Exception)
        {
            return new Error();
        }
    }


    private static async Task ProcessArguments(TranscodeVideoCommand command, Resolution resolution)
    {
        await FFMpegArguments
            .FromFileInput($"{command.VideoPath}")
            .OutputToFile($"output-{resolution.Width}x{resolution.Height}.mp4",
                addArguments: options => options
                    .WithVideoCodec(VideoCodec.LibX264)
                    .WithVideoFilters(filters => { filters.Scale(resolution.Width, -1); })
                    .WithConstantRateFactor(21)
                    .WithAudioCodec(AudioCodec.Aac)
                    .WithFastStart()
            ).ProcessAsynchronously();
    }
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
        var request = new PutObjectRequest
        {
            BucketName = _s3Options.Value.BucketNameConverted,
            Key = notification.VideoKey,
            FilePath = notification.VideoPath
        };
        await _s3.PutObjectAsync(request, cancellationToken);
    }
}