namespace TwitterCloneAPI.Models;

public class TweetHashtag
{
    public int TweetId { get; set; }
    public Tweet Tweet { get; set; } = null!;

    public int HashtagId { get; set; }
    public Hashtag Hashtag { get; set; } = null!;
}