using FFMpegCore;
using FFMpegCore.Enums;
using Mediator;
using Microsoft.Extensions.Options;
using OneOf;
using OneOf.Types;
using ZulaMed.VideoConversion.Features.Transcode.Events;

namespace ZulaMed.VideoConversion.Features.Transcode.Commands;

public class TranscodeVideoCommand : ICommand<OneOf<Success, Error<Exception>>>
{
    public required string VideoPath { get; init; }

    public required Resolution VideoResolution { get; init; }
}

public class
    TranscodeVideoCommandHandler : ICommandHandler<TranscodeVideoCommand, OneOf<Success, Error<Exception>>>
{
    private readonly IPublisher _publisher;
    private readonly IOptions<ResolutionOptions> _resolutions;

    public TranscodeVideoCommandHandler(IPublisher publisher, IOptions<ResolutionOptions> resolutions)
    {
        _publisher = publisher;
        _resolutions = resolutions;
    }

    public async ValueTask<OneOf<Success, Error<Exception>>> Handle(TranscodeVideoCommand command,
        CancellationToken token)
    {
        try
        {
            var resolutions = _resolutions.Value.Resolutions;
            var scaledResolutions =
                resolutions.SkipWhile(x =>
                        x.Height > command.VideoResolution.Height && x.Width > command.VideoResolution.Width)
                    .ToArray();
            var tasks = scaledResolutions.Select(scaledResolution =>
                    ProcessArguments(command, scaledResolution, token))
                .ToList();
            await Task.WhenAll(tasks);
            await GenerateMasterFile(command, scaledResolutions);
            return new Success();
        }
        catch (Exception e)
        {
            return new Error<Exception>(e);
        }
    }


    // because this handlers are registered as singleton, i don't put stuff into fields and just pass the objects as arguments into private methods
    // but putting them into fields would be DRYer. I don't do it bcs i'll have initialization problems(basically need to initialize them each time in the handle method)
    private async Task GenerateMasterFile(TranscodeVideoCommand command, Resolution[] supportedResolutions)
    {
        var directoryName = Path.GetDirectoryName($"{command.VideoPath}");
        var filesArray = supportedResolutions
            .Select(x => $"{directoryName}/{x.Height}p/transcoded-video.m3u8")
            .ToArray();
        var arguments = FFMpegArguments
            .FromConcatInput(filesArray)
            .OutputToFile($"{directoryName}/master.m3u8",
                addArguments: options => { options.WithCustomArgument("-c copy"); });
        await arguments.ProcessAsynchronously();
        
        await _publisher.Publish(new MasterFileGeneratedEvent()
        {
            VideoKey = command.VideoPath,
            MasterFilePath = $"{command.VideoPath}/master.m3u8"
        });
    }


    private async Task ProcessArguments(TranscodeVideoCommand command, Resolution resolution, CancellationToken token)
    {
        var directoryName = Path.GetDirectoryName($"{command.VideoPath}");
        Directory.CreateDirectory($"{directoryName}/{resolution.Height}p");
        var arguments = FFMpegArguments
            .FromFileInput($"{command.VideoPath}")
            .OutputToFile($"{directoryName}/{resolution.Height}p/transcoded-video.m3u8",
                addArguments: options => options
                    .WithVideoCodec(VideoCodec.LibX264)
                    .WithVideoFilters(filters => { filters.Scale(resolution.Width, -1); })
                    .WithConstantRateFactor(21)
                    .WithAudioCodec(AudioCodec.Aac)
                    .WithFastStart()
                    .WithCustomArgument("-f hls")
                    .WithCustomArgument("-hls_time 5")
                    .WithCustomArgument("-hls_list_size 0")
                    .WithCustomArgument($"-hls_segment_filename {directoryName}/{resolution.Height}p/%v_%03d.ts")
                    .WithCustomArgument("-hls_playlist_type vod")
            );
        await arguments.ProcessAsynchronously();
        var key = Path.GetFileNameWithoutExtension(command.VideoPath);
        await _publisher.Publish(new VideoTranscodedEvent
        {
            VideoKey = key,
            VideoDirectoryPath = $"{directoryName}/{resolution.Height}p/"
        }, token);
    }
}