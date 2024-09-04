using System.ComponentModel;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace SoundCloudCli;

public class SoundCloudClientCore(IWebDriver webDriver)
{
    /// <summary>
    /// Searches for users by username and retrieves a list of <see cref="UserInfo"/> objects for each matching result.
    /// </summary>
    /// <param name="url">The URL to query, which should include the username search endpoint.</param>
    /// <param name="maxCount">The maximum number of results to retrieve.</param>
    /// <returns>A list of <see cref="UserInfo"/> objects representing users that match the search criteria.</returns>
    public async Task<List<UserInfo>> SearchByUsername(string url, int maxCount)
    {
        await webDriver.Navigate().GoToUrlAsync(url);
        var wait = new WebDriverWait(webDriver, TimeSpan.FromMilliseconds(5000));
        var element = wait.Until(driver => driver.FindElement(By.XPath(".//div[contains (@class, 'lazyLoadingList')]")));

        var children = ScrollAndYield(element, maxCount, (IJavaScriptExecutor)webDriver,
            LazyLoadingListType.SearchList);
        
        return children.Select(e => e.GetUserInfo())
            .ToList();
    }

    /// <summary>
    /// Retrieves <see cref="UserInfo"/> objects for each follower of a user, specified by their URL.
    /// </summary>
    /// <param name="url">The URL of the user's followers page.</param>
    /// <param name="maxCount">The maximum number of followers to retrieve.</param>
    /// <returns>A list of <see cref="UserInfo"/> objects representing the followers of the specified user.</returns>
    public async Task<List<UserInfo>> GetFollowerInfos(string url, int maxCount)
    {
        await webDriver.Navigate().GoToUrlAsync(url);
        var wait = new WebDriverWait(webDriver, TimeSpan.FromMilliseconds(5000));
        var element = wait.Until(driver => driver.FindElement(By.XPath(".//div[contains (@class, 'lazyLoadingList')]")));

        var children = ScrollAndYield(element, maxCount, (IJavaScriptExecutor)webDriver,
            LazyLoadingListType.BadgeList);

        return children.Select(e => e.GetUserInfo())
            .ToList();
    }

    /// <summary>
    /// Scrolls through a lazy-loading list to load and return up to the specified number of items.
    /// </summary>
    /// <param name="element">The web element containing the lazy-loading list.</param>
    /// <param name="maxCount">The maximum number of items to return.</param>
    /// <param name="driver">The JavaScript executor used to perform scrolling actions.</param>
    /// <param name="listType">The type of list items to be loaded, specified by the LazyLoadingListType enum.</param>
    /// <returns>A list of web elements containing the loaded items, up to the specified maximum count.</returns>
    private List<IWebElement> ScrollAndYield(IWebElement element, int maxCount, IJavaScriptExecutor driver, LazyLoadingListType listType)
    {
        if (!element.HasClassAttribute("lazyLoadingList"))
        {
            return [];
        }

        var xPath = listType switch
        {
            LazyLoadingListType.None => throw new InvalidEnumArgumentException("listType cannot be None."),
            LazyLoadingListType.SearchList => ".//li[contains(@class, 'searchList__item')]",
            LazyLoadingListType.BadgeList => ".//li[contains(@class, 'badgeList__item')]",
            _ => String.Empty
        };

        var children = element.FindElements(By.XPath(xPath));
        
        if (children.Count == 0)
        {
            return [];
        }
        
        if (children.Count >= maxCount)
        {
            return children.Take(maxCount).ToList();
        }

        var prevChildrenCount = children.Count;
        var consecutiveEmptyScrolls = 0;
        
        while (true)
        {
            driver.ExecuteScript("window.scrollTo(0, document.body.scrollHeight);");
            Thread.Sleep(50);
            
            children = element.FindElements(By.XPath(xPath));
            if (children.Count == prevChildrenCount)
            {
                consecutiveEmptyScrolls++;
            }
            else
            {
                consecutiveEmptyScrolls = 0;
                prevChildrenCount = children.Count;
            }

            if (consecutiveEmptyScrolls >= Const.MaxWaitTimeMs / Const.ScrollIntervalMs)
            {
                break;
            }

            if (children.Count >= maxCount)
            {
                break;
            }
        }
        
        return children.Take(maxCount).ToList();
    }
}
