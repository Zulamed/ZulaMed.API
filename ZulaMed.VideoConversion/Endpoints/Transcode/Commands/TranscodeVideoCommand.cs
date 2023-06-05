using FFMpegCore;
using FFMpegCore.Enums;
using ZulaMed.VideoConversion.Infrastructure;

namespace ZulaMed.VideoConversion.Endpoints.Transcode.Commands;

public class TranscodeVideoCommand : ICommand<string[]>
{
    public required string VideoPath { get; init; }

    public required Resolution[] Resolutions { get; init; }
}

public class TranscodeVideoCommandHandler : ICommandHandler<TranscodeVideoCommand, string[]>
{
    public async Task<string[]> HandleAsync(TranscodeVideoCommand command, CancellationToken token)
    {
        var tasks = command.Resolutions
            .Select(resolution => Task.Run(() => ProcessArguments(command, resolution), token))
            .ToList();
        await Task.WhenAll(tasks);
        return new[] { "a" };
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