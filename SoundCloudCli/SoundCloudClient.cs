using OpenQA.Selenium;

namespace SoundCloudCli;

public class SoundCloudClient(IWebDriver driver)
{
    private readonly SoundCloudClientCore _core = new(driver);
    private const string BaseAddress = "https://www.soundcloud.com/";

    /// <summary>
    /// Searches for users by username and retrieves a list of <see cref="UserInfo"/> objects for each matching user.
    /// </summary>
    /// <param name="username">The username to search for.</param>
    /// <param name="limit">The maximum number of results to return.</param>
    /// <returns>A list of <see cref="UserInfo"/> objects representing users that match the given username.</returns>
    public async Task<List<UserInfo>> SearchByUsername(string username, int limit)
    {
        var requestUri = $"search/people?q={username}";
        return await _core.SearchByUsername(BaseAddress + requestUri, limit);
    }

    /// <summary>
    /// Retrieves <see cref="UserInfo"/> objects for all followers of a specified user.
    /// The user is identified by their URL.
    /// </summary>
    /// <param name="user">The user whose followers are to be queried.</param>
    /// <returns>A list of <see cref="UserInfo"/> objects representing the followers of the specified user.</returns>
    public async Task<List<UserInfo>> GetAllFollowerInfos(UserInfo user)
    {
        var urlPostfix = user.Url;
        var url = $"{urlPostfix}/followers";
        return await _core.GetFollowerInfos(url, user.FollowerCount);
    }
    
    /// <summary>
    /// Retrieves the most recent <see cref="UserInfo"/> objects for the specified number of followers of a user.
    /// The user is identified by their URL.
    /// </summary>
    /// <param name="user">The user whose followers are to be queried.</param>
    /// <param name="n">The number of recent followers to retrieve.</param>
    /// <returns>A list of <see cref="UserInfo"/> objects representing the most recent followers of the specified user.</returns>
    public async Task<List<UserInfo>> GetRecentNFollowerInfos(UserInfo user, int n)
    {
        var urlPostfix = user.Url;
        var url = $"{urlPostfix}/followers";
        return await _core.GetFollowerInfos(url, n);
    }
}