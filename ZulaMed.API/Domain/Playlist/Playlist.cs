using ZulaMed.API.Domain.Shared;

namespace ZulaMed.API.Domain.Playlist;

public class Playlist
{
    public Id Id { get; init; } = null!;
    public required PlaylistName PlaylistName { get; set; }
    public required PlaylistDescription PlaylistDescription { get; set; }
    public required User.User Owner { get; set; }
    public List<Video.Video> Videos { get; set; } = new();
}