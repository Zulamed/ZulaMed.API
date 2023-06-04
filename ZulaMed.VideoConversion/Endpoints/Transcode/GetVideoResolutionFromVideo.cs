using FFMpegCore;
using ZulaMed.VideoConversion.Infrastructure;

namespace ZulaMed.VideoConversion.Endpoints.Transcode;


public class GetVideoResolutionFromVideoQuery : IQuery<string>
{
   public required string PathToFile { get; set; } 
}


public class GetVideoResolutionFromVideoQueryHandler : IQueryHandler<GetVideoResolutionFromVideoQuery, string>
{
    public async Task<string> HandleAsync(GetVideoResolutionFromVideoQuery query, CancellationToken cancellationToken)
    {
        var mediaAnalysis = await FFProbe.AnalyseAsync(query.PathToFile, cancellationToken: cancellationToken);
        return $"{mediaAnalysis.PrimaryVideoStream.Width}x{mediaAnalysis.PrimaryVideoStream.Height}";
    }
}