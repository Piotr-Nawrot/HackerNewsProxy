namespace Contracts.V1.Stories;

public sealed class GetStoryResponse
{
    public string? Title { get; set; }
    public Uri? Uri { get; set; }
    public string? PostedBy { get; set; }
    public DateTimeOffset? Time { get; set; }
    public int Score { get; set; }
    public int CommentCount { get; set; }
}