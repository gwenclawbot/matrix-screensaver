using MatrixScreensaver;

namespace MatrixScreensaver.Tests;

public class SettingsTests
{
    [Fact]
    public void ToQueryString_IncludesCoreParameters()
    {
        var settings = ScreensaverSettings.CreateDefault();

        var query = settings.ToQueryString();

        Assert.Contains("numColumns=120", query);
        Assert.Contains("fps=60", query);
        Assert.Contains("renderer=regl", query);
    }

    [Fact]
    public void ToQueryString_AppendsCustomQuery()
    {
        var settings = ScreensaverSettings.CreateDefault();
        settings.CustomQuery = "foo=bar";

        var query = settings.ToQueryString();

        Assert.Contains("foo=bar", query);
    }
}
