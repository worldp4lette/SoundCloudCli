namespace SoundCloudCli;

public class UserInfo : IComparable<UserInfo>
{
    public long UserId { get; init; }
    public required string Url { get; init; }
    public required string Username { get; init; }
    public int FollowerCount { get; init; }
    
    public int CompareTo(UserInfo? other)
    {
        return string.Compare(Username, other?.Username, StringComparison.Ordinal);
    }
}

public class UserInfoExtended : UserInfo
{
    public List<UserInfo> Followings { get; set; } = [];
    public List<UserInfo> Followers { get; set; } = [];
    public List<TrackInfo> Tracks { get; set; } = [];
}