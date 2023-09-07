namespace ZulaMed.API.Domain.Playlist;

public class Playlist
{
    public required PlaylistId Id { get; init; }
    public required PlaylistName PlaylistName { get; set; }
    public required PlaylistDescription PlaylistDescription { get; set; }
    public required User.User Owner { get; set; }
    public List<Video.Video> Videos { get; set; } = new();
}