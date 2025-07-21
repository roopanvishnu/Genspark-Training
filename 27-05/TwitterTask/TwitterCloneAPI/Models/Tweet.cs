namespace TwitterCloneAPI.Models;
public class Tweet
{
    public int Id { get; set; }
    public string Content { get; set; } = null!;

    public ICollection<TweetHashtag> TweetHashtags { get; set; } = new List<TweetHashtag>();
}