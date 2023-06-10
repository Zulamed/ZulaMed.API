using System.Text.Json;
using Mediator;
using ZulaMed.VideoConversion.Features.Transcode.Commands;
using ZulaMed.VideoConversion.Features.Transcode.Queries;

namespace ZulaMed.VideoConversion.Features.Transcode;

public class Worker : BackgroundService
{
    private readonly IMediator _mediator;
    private readonly ILogger<Worker> _logger;

    public Worker(IMediator mediator, ILogger<Worker> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }


    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var response = await _mediator.Send(new ConsumeMessageFromSqs(), stoppingToken);
            if (response.Messages.Count == 0)
                continue;
            foreach (var message in response.Messages)
            {
                var messageBody = JsonSerializer.Deserialize<VideoTranscodeRequest>(message.Body);
                if (messageBody is null)
                    continue;
                var video = await _mediator.Send(new GetVideoFromS3Query
                {
                    Key = messageBody.VideoS3Path
                }, stoppingToken);
                await video.WriteResponseStreamToFileAsync("test.mp4", true, stoppingToken);
                var resolution = await _mediator.Send(new GetVideoResolutionFromVideoQuery
                {
                    PathToFile = "test.mp4"
                }, stoppingToken);
                if (!resolution.TryPickT0(out var videoResolution, out var exception))
                {
                    _logger.Log(LogLevel.Warning, "Couldn't get video resolution. Key: {MessageId}, Exception: {Message}",
                        message.MessageId, exception.Value.Message);
                    continue;
                }
                var videoTranscodedResult = await _mediator.Send(new TranscodeVideoCommand
                {
                    VideoPath = "test.mp4",
                    Resolution = new Resolution() { Width = 1280, Height = 720 }
                }, stoppingToken);
                if (videoTranscodedResult.IsT1)
                {
                    _logger.Log(LogLevel.Warning, "Couldn't transcode video. Key: {MessageId}",
                        message.MessageId);
                    continue;
                }
                _logger.Log(LogLevel.Information, "Video transcoded successfully. Key: {MessageId}",
                    message.MessageId);
                await _mediator.Send(new DeleteMessageFromSqsCommand()
                {
                   ReceiptHandle = message.ReceiptHandle
                }, stoppingToken);
            }

            await Task.Delay(3000, stoppingToken);
        }
    }
}