using ZulaMed.API.Domain.Comments;
using ZulaMed.API.Domain.Like;
using ZulaMed.API.Domain.Dislike;

namespace ZulaMed.API.Domain.Video;

public class Video
{
    public required VideoId Id { get; init; }
    public required VideoTitle VideoTitle { get; init; }
    public required VideoUrl VideoUrl { get; init; }
    public required VideoThumbnail VideoThumbnail { get; init; }
    public required VideoDescription VideoDescription { get; init; }
    public required VideoPublishedDate VideoPublishedDate { get; init; }
    // public required VideoPublisherId VideoPublisherId { get; init; }
    
    public required User.User Publisher { get; init; }
    
    public VideoLike VideoLike { get; init; } = VideoLike.Zero; 
    
    public VideoDislike VideoDislike { get; init; } = VideoDislike.Zero;
  
    public VideoView VideoView { get; init; } = VideoView.Zero;
  
    public List<Comment> Comments { get; init; } = new();
    
    public List<Like<Video>> Likes { get; init; } = new();
    public List<Dislike<Video>> Dislikes { get; init; } = new();
    public List<Playlist.Playlist> Playlists { get; init; } = new();
}