using MatrixScreensaver;

namespace MatrixScreensaver.Tests;

public class CommandLineOptionsTests
{
    [Theory]
    [InlineData("/s", ScreensaverMode.Run)]
    [InlineData("-s", ScreensaverMode.Run)]
    [InlineData("/c", ScreensaverMode.Configure)]
    [InlineData("-c", ScreensaverMode.Configure)]
    [InlineData("/t", ScreensaverMode.Test)]
    public void Parse_RecognizesMode(string arg, ScreensaverMode expected)
    {
        var options = CommandLineOptions.Parse(new[] { arg });

        Assert.Equal(expected, options.Mode);
    }

    [Fact]
    public void Parse_PreviewWithSeparateHandle_Works()
    {
        var options = CommandLineOptions.Parse(new[] { "/p", "12345" });

        Assert.Equal(ScreensaverMode.Preview, options.Mode);
        Assert.NotEqual(IntPtr.Zero, options.PreviewParentHandle);
    }

    [Fact]
    public void Parse_PreviewWithInlineHandle_Works()
    {
        var options = CommandLineOptions.Parse(new[] { "/p:12345" });

        Assert.Equal(ScreensaverMode.Preview, options.Mode);
        Assert.NotEqual(IntPtr.Zero, options.PreviewParentHandle);
    }
}
