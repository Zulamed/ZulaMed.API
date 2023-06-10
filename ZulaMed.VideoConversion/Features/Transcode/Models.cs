using System.Text.RegularExpressions;

namespace ZulaMed.VideoConversion.Features.Transcode;


public struct Resolution
{
    public required int Width { get; init; }
    public required int Height { get; init; }
}

public class VideoTranscodeRequest
{
    public required string VideoS3Path { get; init; }
}