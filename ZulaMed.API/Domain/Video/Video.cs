using ZulaMed.API.Domain.Comments;
using ZulaMed.API.Domain.Like;
using ZulaMed.API.Domain.Dislike;

namespace ZulaMed.API.Domain.Video;

public enum VideoStatus
{
   WaitingForUpload,
   GettingProcessed,
   Ready,
   Cancelled,
}


public class Video
{
    public required VideoId Id { get; init; }
    public VideoTitle VideoTitle { get; init; }
    public VideoUrl VideoUrl { get; init; }
    public VideoThumbnail VideoThumbnail { get; init; }
    public VideoDescription VideoDescription { get; init; }
    public VideoPublishedDate VideoPublishedDate { get; init; }
    
    public required VideoStatus VideoStatus { get; init; } 
    
    public required User.User Publisher { get; init; }
    
    public VideoLike VideoLike { get; init; } = VideoLike.Zero; 
    
    public VideoDislike VideoDislike { get; init; } = VideoDislike.Zero;
  
    public VideoView VideoView { get; init; } = VideoView.Zero;
  
    public List<Comment> Comments { get; init; } = new();
    
    public List<Like<Video>> Likes { get; init; } = new();
    public List<Dislike<Video>> Dislikes { get; init; } = new();
    public List<Playlist.Playlist> Playlists { get; init; } = new();
    public List<ViewHistory.ViewHistory> ViewHistories { get; init; } = new();
}