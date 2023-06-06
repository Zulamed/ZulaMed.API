using Amazon.S3;
using Amazon.S3.Model;
using FFMpegCore;
using FFMpegCore.Enums;
using Microsoft.Extensions.Options;
using OneOf.Types;
using ZulaMed.VideoConversion.Infrastructure;
using OneOf;

namespace ZulaMed.VideoConversion.Endpoints.Transcode.Commands;

public class TranscodeVideoCommand : ICommand<OneOf<Success<string>, Error>>
{
    public required string VideoPath { get; init; }

    public required Resolution Resolution { get; init; }
}

public class TranscodeVideoCommandHandler : ICommandHandler<TranscodeVideoCommand, OneOf<Success<string>, Error>>
{
    public async Task<OneOf<Success<string>, Error>> HandleAsync(TranscodeVideoCommand command, CancellationToken token)
    {
        // var tasks = command.Resolution
        //     .Select(resolution => Task.Run(() => ProcessArguments(command, resolution), token))
        // await Task.WhenAll(tasks);
        try
        {
            await ProcessArguments(command, command.Resolution);
        }
        catch (Exception)
        {
            return new Error();
        }
        return new Success<string>($"output-{command.Resolution.Width}x{command.Resolution.Height}.mp4");
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
public class TranscodeVideoCommandHandlerDecorator : ICommandHandler<TranscodeVideoCommand, OneOf<Success<string>, Error>>
{
    private readonly ICommandHandler<TranscodeVideoCommand, OneOf<Success<string>, Error>> _decorated;
    private readonly IAmazonS3 _s3;
    private readonly IOptions<S3BucketOptions> _s3Options;

    public TranscodeVideoCommandHandlerDecorator(ICommandHandler<TranscodeVideoCommand, OneOf<Success<string>, Error>> decorated,
        IAmazonS3 s3, IOptions<S3BucketOptions> s3Options)
    {
        _decorated = decorated;
        _s3 = s3;
        _s3Options = s3Options;
    }
    
    
    public async Task<OneOf<Success<string>, Error>> HandleAsync(TranscodeVideoCommand command, CancellationToken token)
    {
        var result = await _decorated.HandleAsync(command, token);
        if (result.TryPickT1(out var error, out var success))
        {
            return error;
        }
        var request = new PutObjectRequest
        {
            BucketName = _s3Options.Value.BucketNameConverted,
            Key = success.Value,
            FilePath = success.Value
        };
        var response = await _s3.PutObjectAsync(request, token);
        return new Success<string>(success.Value);
    }
}
