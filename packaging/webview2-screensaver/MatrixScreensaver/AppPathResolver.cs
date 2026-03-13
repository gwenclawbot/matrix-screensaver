namespace MatrixScreensaver;

using Microsoft.Win32;

internal static class AppPathResolver
{
    public static Uri ResolveIndexHtmlUri(ScreensaverSettings settings)
    {
        var appDirectory = ResolveAppDirectory();
        var indexPath = Path.Combine(appDirectory, "index.html");

        if (!File.Exists(indexPath))
        {
            throw new FileNotFoundException("Could not find app/index.html. Reinstall the screensaver package.", indexPath);
        }

        var builder = new UriBuilder(new Uri(indexPath))
        {
            Query = settings.ToQueryString()
        };

        return builder.Uri;
    }

    public static string ResolveAppDirectory()
    {
        var configured = Environment.GetEnvironmentVariable("MATRIX_SCREENSAVER_APP_PATH");
        if (!string.IsNullOrWhiteSpace(configured) && Directory.Exists(configured))
        {
            return configured;
        }

        var userAppPath = Registry.GetValue(
            @"HKEY_CURRENT_USER\Software\MatrixScreensaver",
            "AppPath",
            null) as string;
        if (!string.IsNullOrWhiteSpace(userAppPath) && Directory.Exists(userAppPath))
        {
            return userAppPath;
        }

        var machineAppPath = Registry.GetValue(
            @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\ScreenSavers",
            "MatrixScreensaverPath",
            null) as string;
        if (!string.IsNullOrWhiteSpace(machineAppPath) && Directory.Exists(machineAppPath))
        {
            return machineAppPath;
        }

        var sidecarPathFile = Path.Combine(AppContext.BaseDirectory, "app-path.txt");
        if (File.Exists(sidecarPathFile))
        {
            var path = File.ReadAllText(sidecarPathFile).Trim();
            if (Directory.Exists(path))
            {
                return path;
            }
        }

        var local = Path.Combine(AppContext.BaseDirectory, "app");
        if (Directory.Exists(local))
        {
            return local;
        }

        return local;
    }
}
