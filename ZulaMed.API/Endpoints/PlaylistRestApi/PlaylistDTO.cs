using ZulaMed.API.Domain.User;
using ZulaMed.API.Domain.Video;

namespace ZulaMed.API.Endpoints.PlaylistRestApi;

public class PlaylistDTO
{
    public required Guid Id { get; init; }
    public required string PlaylistName { get; init; }
    public required User Owner { get; set; }
    public required List<Video> Videos { get; set; }
}