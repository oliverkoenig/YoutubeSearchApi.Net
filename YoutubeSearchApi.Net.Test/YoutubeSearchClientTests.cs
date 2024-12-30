namespace YoutubeSearchApi.Net.Test;

using YoutubeSearchApi.Net.Services;

public class YoutubeSearchClientTests
{
    [Fact]
    public void Test1()
    {
        var json = GetTestData("YoutubeSearchResponse.json");
        var client = new YoutubeSearchClient(new HttpClient());
        client.ParseData(json);
    }

    private static string GetTestData(string fileName)
    {
        return File.ReadAllText(Path.Combine(TestUtils.GetTestDataPath()!, fileName));
    }
}