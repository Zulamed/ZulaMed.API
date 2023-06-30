using System.Net.Sockets;
using System.Text;
using CliWrap;
using CliWrap.Buffered;
using FFMpegCore;
using FFMpegCore.Arguments;
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
            var resolutions = _resolutions.Value.VideoConfigurations;
            var scaledResolutions =
                resolutions.SkipWhile(x =>
                        x.Height > command.VideoResolution.Height && x.Width > command.VideoResolution.Width)
                    .ToArray();
            return await ProcessArguments(command, scaledResolutions, token);
        }
        catch (Exception e)
        {
            return new Error<Exception>(e);
        }
    }

    private static Command GetFFMpegCommand(string inputFile, Resolution[] resolutions, string directoryName)
    {
        var maps = new StringBuilder();
        var range = Enumerable.Range(0, resolutions.Length).ToArray();
        
        const string map = "-map 0:v -map 0:a?";
        foreach (var _ in range)
        {
            maps.Append(map);
            maps.Append(' ');
        }
        maps.RemoveLastCharacter();
        
        var varStreamMaps = new StringBuilder();
        foreach (var (resolution, index) in resolutions.Zip(range))
        {
            varStreamMaps.Append($"""v:{index},a:{index},name:{resolution.Height}p""");
            varStreamMaps.Append(' ');
        }
        varStreamMaps.RemoveLastCharacter();
        
        var outputArguments = new StringBuilder();
        foreach (var (resolution, index) in resolutions.Zip(range))
        {
            outputArguments.Append(
                $"""-filter:v:{index} scale=-2:{resolution.Height} -b:v:{index} {resolution.BitrateMbps}M -g 48""");
            outputArguments.Append(' ');
        }
        outputArguments.RemoveLastCharacter();
        
        var ffMpegArguments = FFMpegArguments
            .FromFileInput(inputFile, addArguments: options => options
                .WithVideoCodec(VideoCodec.LibX264)
                .WithSpeedPreset(Speed.Fast)
                .WithConstantRateFactor(21)
                .WithAudioCodec(AudioCodec.Aac)
                .WithCustomArgument("-sc_threshold 0")
                .WithCustomArgument(maps.ToString())
                .WithCustomArgument(outputArguments.ToString())
                .WithCustomArgument($"-var_stream_map \"{varStreamMaps}\"")
                .WithCustomArgument("-f hls")
                .WithCustomArgument("-hls_time 5")
                .WithCustomArgument("-hls_list_size 0")
                .WithCustomArgument($"-hls_segment_filename {directoryName}/%v/data%03d.ts")
                .WithCustomArgument("-hls_playlist_type vod")
                .WithCustomArgument("-hls_flags independent_segments")
                .WithCustomArgument("-master_pl_name master.m3u8")
                .WithCustomArgument($"-y {directoryName}/%v/stream.m3u8")
            );
        var argument = ffMpegArguments.Text;
        argument = argument.Replace($"-i \"{inputFile}\"", "");
        argument = argument.Remove(argument.Length - 1, 1);
        return Cli.Wrap("ffmpeg")
            .WithArguments(args =>
            {
                args.Add("-hide_banner")
                    .Add("-i")
                    .Add(inputFile)
                    .Add(argument, false);
            });
    }


    private async Task<OneOf<Success, Error<Exception>>> ProcessArguments(TranscodeVideoCommand command, Resolution[] resolutions,
        CancellationToken token)
    {
        var directoryName = Path.GetDirectoryName($"{command.VideoPath}");
        var ffMpegCommand = GetFFMpegCommand(command.VideoPath, resolutions, directoryName!);
        var result = await ffMpegCommand.ExecuteBufferedAsync(token);
        if (result.ExitCode != 0)
            return new Error<Exception>(new Exception(result.StandardError));
        var fileName = Path.GetFileName(command.VideoPath);
        await _publisher.Publish(new VideoTranscodedEvent
        {
            VideoNameWithExtension = fileName,
            VideoDirectoryPath = $"{directoryName}"
        }, token);
        return new Success();
    }
}