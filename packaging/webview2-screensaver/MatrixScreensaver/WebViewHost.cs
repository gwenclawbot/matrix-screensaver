using System.Runtime.InteropServices;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;

namespace MatrixScreensaver;

internal sealed class WebViewHost : UserControl
{
    private readonly WebView2 _webView = new();
    private static readonly string LogFile = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "matrix-screensaver", "debug.log");

    private static void Log(string msg)
    {
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(LogFile)!);
            File.AppendAllText(LogFile, $"[{DateTime.Now:HH:mm:ss.fff}] {msg}\r\n");
        }
        catch { /* log failure is non-fatal */ }
    }

    public WebViewHost()
    {
        Controls.Add(_webView);
        _webView.Dock = DockStyle.Fill;
        _webView.DefaultBackgroundColor = Color.Black;
        Log("WebViewHost constructed");
    }

    private const string VirtualHostName = "matrix.screensaver.local";
    private const string ImagesHostName = "matrix-images.screensaver.local";
    public static string BuildImageUrl(string filename) => $"https://{ImagesHostName}/{filename}";

    public async Task InitializeAndNavigateAsync(Uri appUri, bool isPreview)
    {
        Log($"InitializeAndNavigateAsync called. appUri={appUri}");

        if (!IsWebView2RuntimeInstalled())
        {
            Log("WebView2 runtime NOT found");
            ShowMissingRuntimeMessage();
            return;
        }

        Log("WebView2 runtime found");

        try
        {
            // Use a dedicated user data folder to avoid conflicts with other WebView2 instances.
            var userDataFolder = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "matrix-screensaver", "webview2-cache");
            Log($"UserDataFolder={userDataFolder}");

            var options = new CoreWebView2EnvironmentOptions("--autoplay-policy=no-user-gesture-required");
            Log("Creating CoreWebView2Environment...");
            var env = await CoreWebView2Environment.CreateAsync(null, userDataFolder, options);
            Log("Environment created, calling EnsureCoreWebView2Async...");
            await _webView.EnsureCoreWebView2Async(env);
            Log("CoreWebView2 initialized");

            _webView.CoreWebView2.Settings.AreDevToolsEnabled = true;
            _webView.CoreWebView2.Settings.AreDefaultContextMenusEnabled = true;
            _webView.CoreWebView2.Settings.IsStatusBarEnabled = false;
            _webView.CoreWebView2.Settings.IsZoomControlEnabled = false;

            _webView.CoreWebView2.WebMessageReceived += (_, e) =>
            {
                var msg = e.TryGetWebMessageAsString();
                Log($"WebMessage: {msg}");
                Invoke(() => ShowError("JS: " + msg));
            };
            _webView.CoreWebView2.NavigationCompleted += (_, e) =>
            {
                Log($"NavigationCompleted: IsSuccess={e.IsSuccess} Status={e.WebErrorStatus} Source={_webView.Source}");
                if (!e.IsSuccess)
                    Invoke(() => ShowError($"Navigation failed: {e.WebErrorStatus}\n{_webView.Source}"));
            };

            await _webView.CoreWebView2.AddScriptToExecuteOnDocumentCreatedAsync("""
                window.onerror = (msg, src, line, col, err) => {
                    window.chrome.webview.postMessage('onerror: ' + msg + ' @ ' + src + ':' + line);
                    return false;
                };
                window.onunhandledrejection = (e) => {
                    window.chrome.webview.postMessage('unhandledrejection: ' + (e.reason?.message ?? String(e.reason)));
                };
                """);

            if (!isPreview)
            {
                await _webView.CoreWebView2.AddScriptToExecuteOnDocumentCreatedAsync(
                    "document.addEventListener('DOMContentLoaded', function(){ document.documentElement.style.cursor = 'none'; document.body.style.cursor = 'none'; });");
            }

            // Map a virtual hostname so ES module imports work.
            // file:// origins block cross-file module loading in Chromium;
            // serving via a virtual https host resolves this.
            var appDirectory = Path.GetDirectoryName(appUri.LocalPath)!;
            Log($"SetVirtualHostNameToFolderMapping: {VirtualHostName} -> {appDirectory}");
            _webView.CoreWebView2.SetVirtualHostNameToFolderMapping(
                VirtualHostName,
                appDirectory,
                CoreWebView2HostResourceAccessKind.Allow);

            // Map a second virtual host for user-selected local images so that
            // file:// URIs (blocked from https origins) can be served safely.
            var imagesDir = SettingsManager.LocalImagesDirectory;
            Directory.CreateDirectory(imagesDir);
            Log($"SetVirtualHostNameToFolderMapping: {ImagesHostName} -> {imagesDir}");
            _webView.CoreWebView2.SetVirtualHostNameToFolderMapping(
                ImagesHostName,
                imagesDir,
                CoreWebView2HostResourceAccessKind.Allow);

            var query = appUri.Query;
            var virtualUri = new Uri($"https://{VirtualHostName}/index.html{query}");
            Log($"Navigating to: {virtualUri}");
            _webView.Source = virtualUri;
        }
        catch (Exception ex)
        {
            Log($"EXCEPTION: {ex}");
            Invoke(() => ShowError(ex.GetType().Name + ": " + ex.Message));
        }
    }

    private static bool IsWebView2RuntimeInstalled()
    {
        try
        {
            var version = CoreWebView2Environment.GetAvailableBrowserVersionString();
            return !string.IsNullOrWhiteSpace(version);
        }
        catch (COMException)
        {
            return false;
        }
        catch (WebView2RuntimeNotFoundException)
        {
            return false;
        }
    }

    private void ShowMissingRuntimeMessage()
    {
        ShowError("Microsoft Edge WebView2 Runtime is required.\n\nInstall Evergreen Runtime:\nhttps://go.microsoft.com/fwlink/p/?LinkId=2124703");
    }

    private void ShowError(string message)
    {
        var label = new Label
        {
            Dock = DockStyle.Fill,
            Text = message,
            ForeColor = Color.White,
            BackColor = Color.Black,
            TextAlign = ContentAlignment.MiddleCenter,
            Font = new Font("Segoe UI", 10, FontStyle.Regular)
        };

        Controls.Clear();
        Controls.Add(label);
    }
}
