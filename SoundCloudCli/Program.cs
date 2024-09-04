using OpenQA.Selenium.Chrome;

namespace SoundCloudCli;

public static class Program
{
    private static async Task Main(string[] args)
    {
        var client = new SoundCloudClient(new ChromeDriver());

        // TODO: Test should be added
    }
}