namespace YoutubeSearchApi.Net.Test;

using YoutubeSearchApi.Net.Services;

public class YoutubeSearchClientTests
{
    [Fact]
    public void TestParseSearchResponseJson()
    {
        var json = GetTestData("YoutubeSearchResponse.json");
        var client = new YoutubeSearchClient(new HttpClient());
        var videos = client.ParseData(json);
        var descriptions = videos.Select(v => v.Description).ToList();
    }

    [Fact]
    public void TestParseSearchResponseHtml()
    {
        var html = GetTestData("YoutubeSearchResponse.html");
        var client = new YoutubeSearchClient(new HttpClient());
        var videos = client.ParseData(html);
    }

    private static string GetTestData(string fileName)
    {
        return File.ReadAllText(Path.Combine(TestUtils.GetTestDataPath()!, fileName));
    }
}