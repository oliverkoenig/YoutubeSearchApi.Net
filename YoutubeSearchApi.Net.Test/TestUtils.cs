namespace YoutubeSearchApi.Net.Test;

using System.Runtime.CompilerServices;

internal class TestUtils
{
    public static string? GetThisFilePath([CallerFilePath] string? path = null)
    {
        return path;
    }

    public static string? GetTestDataPath([CallerFilePath] string? path = null)
    {
        var basePath = path![..path!.IndexOf("\\", path.IndexOf("YoutubeSearchApi.Net.Test"))];
        var testDataPath = Path.Combine(basePath, "TestData");
        return testDataPath;
    }
}
