namespace TwitterCloneAPI.Models;
public class Hashtag
{
    public int Id { get; set; }
    public string Tag { get; set; } = null!;

    public ICollection<TweetHashtag> TweetHashtags { get; set; } = new List<TweetHashtag>();
}