using ZulaMed.API.Domain.User;
using ZulaMed.API.Domain.Video;

namespace ZulaMed.API.Endpoints.ViewHistory;

public class ViewHistoryDTO
{
    public required Guid ViewHistoryId { get; init; }
    public required UserDTO ViewedBy { get; set; }
    public required VideoDTO ViewedVideo { get; set; }
    public required DateTime ViewedAt { get; init; } 
}