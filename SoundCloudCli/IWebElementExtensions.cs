using System.Diagnostics;
using System.Globalization;
using OpenQA.Selenium;

namespace SoundCloudCli;

public static class IWebElementExtensions
{
    public static bool HasClassAttribute(this IWebElement element, string attributeName)
    {
        var classAttr = element.GetAttribute("class");
        if (classAttr == null) return false;
        var classes = classAttr.Split(" ");
        return classes.Contains(attributeName);

    }

    public static UserInfo GetUserInfo(this IWebElement element)
    {
        if (element.TagName.Equals("li", StringComparison.OrdinalIgnoreCase)
            && element.HasClassAttribute("searchList__item"))
        {
            return GetUserInfoFromSearchListItem(element);
        }

        if (element.TagName.Equals("li", StringComparison.OrdinalIgnoreCase)
            && element.HasClassAttribute("badgeList__item"))
        {
            return GetUserInfoFromBadgeListItem(element);
        }

        throw new InvalidOperationException("Cannot extract user info from the given element.");
    }

    private static UserInfo GetUserInfoFromSearchListItem(IWebElement element)
    {
        var mediaContentItem = element.FindElement(By.ClassName("sc-media-content")) ?? throw new UnreachableException();

        var userItemTitle = mediaContentItem.FindElement(By.XPath(".//h2[contains(@class, 'userItem__title')]//a"));
        
        var url = userItemTitle.GetAttribute("href");
        var username = userItemTitle.Text;
        var followerCount = mediaContentItem.GetFollowerCount();

        return new UserInfo()
        {
            Username = username,
            Url = url,
            FollowerCount = followerCount
        };
    }

    private static UserInfo GetUserInfoFromBadgeListItem(IWebElement element)
    {
        var userItemTitle = element.FindElement(By.XPath(".//div[contains(@class, 'userBadgeListItem__title')]//a"));
        var url = userItemTitle.GetAttribute("href");
        var username = userItemTitle.Text;
        var followerCount = element.GetFollowerCount();
        
        return new UserInfo()
        {
            Username = username,
            Url = url,
            FollowerCount = followerCount
        };
    }
    
    private static int GetFollowerCount(this IWebElement element)
    {
        var followerCount = 0;
        
        var userStatElement = element.FindElements(By.XPath(".//li[@class='sc-ministats-item']"));
        
        if (userStatElement.Any())
        {        
            _ = int.TryParse(userStatElement[0].GetAttribute("title").Split(" ")[0], 
                NumberStyles.AllowThousands,
                CultureInfo.InvariantCulture,
                out followerCount);
        }
        
        return followerCount;
    }
}
