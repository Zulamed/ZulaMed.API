using FFMpegCore;
using Mediator;
using OneOf.Types;

namespace ZulaMed.VideoConversion.Features.Transcode.Queries;



public class GetVideoResolutionFromVideoQuery : IQuery<Result<Resolution, InvalidOperationException>>
{
    public required string PathToFile { get; set; }
}

public class GetVideoResolutionFromVideoQueryHandler : IQueryHandler<GetVideoResolutionFromVideoQuery,
    Result<Resolution, InvalidOperationException>>
{
    public async ValueTask<Result<Resolution, InvalidOperationException>> Handle(GetVideoResolutionFromVideoQuery query,
        CancellationToken cancellationToken)
    {
        var mediaAnalysis = await FFProbe.AnalyseAsync(query.PathToFile, cancellationToken: cancellationToken);
        if (mediaAnalysis.PrimaryVideoStream == null)
        {
            return new 
                Error<InvalidOperationException>(new InvalidOperationException("No video stream found"));
        }
        return new Resolution
        {
            Width = mediaAnalysis.PrimaryVideoStream.Width,
            Height = mediaAnalysis.PrimaryVideoStream.Height
        };
    }
}