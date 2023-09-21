namespace ZulaMed.API.Domain.Playlist;

public class Playlist
{
    public required PlaylistId Id { get; init; }
    public required PlaylistName PlaylistName { get; init; }
    public required PlaylistDescription PlaylistDescription { get; init; }
    public required User.User Owner { get; init; }
    public List<Video.Video> Videos { get; init; } = new();
}