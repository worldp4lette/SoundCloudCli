namespace SoundCloudCli;

public class TrackInfo
{
    public long TrackId { get; set; }
    public required string Title { get; set; }
    public required string Duration { get; set; }
    public long Plays { get; set; }
    public int Likes { get; set; }
}