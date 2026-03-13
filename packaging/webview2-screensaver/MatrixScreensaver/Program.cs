using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace MatrixScreensaver;

internal static class Program
{
    [STAThread]
    private static void Main(string[] args)
    {
        ApplicationConfiguration.Initialize();

        var options = CommandLineOptions.Parse(args);
        var settingsManager = new SettingsManager();
        var settings = settingsManager.Load();

        switch (options.Mode)
        {
            case ScreensaverMode.Configure:
                using (var config = new ConfigForm(settingsManager, settings))
                {
                    config.ShowDialog();
                }
                break;

            case ScreensaverMode.Preview:
                if (options.PreviewParentHandle == IntPtr.Zero)
                {
                    MessageBox.Show("Preview mode requires a valid parent window handle.", "Matrix Screensaver", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                Application.Run(new PreviewApplicationContext(options.PreviewParentHandle, settings));
                break;

            case ScreensaverMode.Test:
                Application.Run(new TestApplicationContext(settings));
                break;

            case ScreensaverMode.Run:
            default:
                Application.Run(new ScreensaverApplicationContext(settings));
                break;
        }
    }
}

public enum ScreensaverMode
{
    Run,
    Configure,
    Preview,
    Test
}

public sealed class CommandLineOptions
{
    public ScreensaverMode Mode { get; private init; } = ScreensaverMode.Run;
    public IntPtr PreviewParentHandle { get; private init; } = IntPtr.Zero;

    public static CommandLineOptions Parse(string[] args)
    {
        if (args.Length == 0)
        {
            return new CommandLineOptions();
        }

        var normalized = args.Select(a => a.Trim()).Where(a => !string.IsNullOrWhiteSpace(a)).ToArray();
        if (normalized.Length == 0)
        {
            return new CommandLineOptions();
        }

        var first = normalized[0].ToLowerInvariant();

        if (first.StartsWith("/p") || first.StartsWith("-p"))
        {
            var handle = TryParsePreviewHandle(normalized);
            return new CommandLineOptions
            {
                Mode = ScreensaverMode.Preview,
                PreviewParentHandle = handle
            };
        }

        if (first is "/c" or "-c" || first.StartsWith("/c:") || first.StartsWith("-c:"))
        {
            return new CommandLineOptions { Mode = ScreensaverMode.Configure };
        }

        if (first is "/s" or "-s")
        {
            return new CommandLineOptions { Mode = ScreensaverMode.Run };
        }

        if (first is "/t" or "-t" or "/test" or "-test")
        {
            return new CommandLineOptions { Mode = ScreensaverMode.Test };
        }

        return new CommandLineOptions();
    }

    private static IntPtr TryParsePreviewHandle(IReadOnlyList<string> args)
    {
        string? hwndCandidate = null;

        if (args[0].Length > 2)
        {
            hwndCandidate = args[0][2..].Trim().TrimStart(':');
        }
        else if (args.Count > 1)
        {
            hwndCandidate = args[1].Trim();
        }

        if (string.IsNullOrWhiteSpace(hwndCandidate))
        {
            return IntPtr.Zero;
        }

        if (long.TryParse(hwndCandidate, out var hwndLong))
        {
            return new IntPtr(hwndLong);
        }

        return IntPtr.Zero;
    }
}

internal sealed class ScreensaverApplicationContext : ApplicationContext
{
    private readonly List<ScreensaverForm> _forms = new();
    private readonly LowLevelInputHook _hook;
    private readonly DateTime _startedAtUtc = DateTime.UtcNow;
    private static readonly TimeSpan StartupGrace = TimeSpan.FromMilliseconds(200);
    private Point _initialCursorPos;
    private const int ExitThresholdPixels = 10;

    public ScreensaverApplicationContext(ScreensaverSettings settings)
    {
        Cursor.Hide();

        foreach (var screen in Screen.AllScreens)
        {
            var form = new ScreensaverForm(screen, settings, allowConfigShortcut: false);
            form.FormClosed += OnFormClosed;
            _forms.Add(form);
        }

        foreach (var form in _forms)
        {
            form.Show();
        }

        _initialCursorPos = Cursor.Position;

        // Low-level hooks intercept input even when WebView2 has focus.
        _hook = new LowLevelInputHook(
            onMouseActivity: pos =>
            {
                if (DateTime.UtcNow - _startedAtUtc < StartupGrace) return;
                if (Math.Abs(pos.X - _initialCursorPos.X) > ExitThresholdPixels ||
                    Math.Abs(pos.Y - _initialCursorPos.Y) > ExitThresholdPixels)
                    ExitThread();
            },
            onMouseClick: () =>
            {
                if (DateTime.UtcNow - _startedAtUtc < StartupGrace) return;
                ExitThread();
            },
            onKeyActivity: () =>
            {
                if (DateTime.UtcNow - _startedAtUtc < StartupGrace) return;
                ExitThread();
            });
    }

    private void OnFormClosed(object? sender, FormClosedEventArgs e)
    {
        if (_forms.All(f => f.IsDisposed || !f.Visible))
            ExitThread();
    }

    protected override void ExitThreadCore()
    {
        _hook.Dispose();
        foreach (var form in _forms.Where(f => !f.IsDisposed).ToArray())
            form.Close();
        Cursor.Show();
        base.ExitThreadCore();
    }
}

internal sealed class PreviewApplicationContext : ApplicationContext
{
    private readonly PreviewForm _preview;

    public PreviewApplicationContext(IntPtr parentHandle, ScreensaverSettings settings)
    {
        _preview = new PreviewForm(parentHandle, settings);
        _preview.FormClosed += (_, _) => ExitThread();
        _preview.Show();
    }
}

internal sealed class TestApplicationContext : ApplicationContext
{
    private readonly ScreensaverForm _form;

    public TestApplicationContext(ScreensaverSettings settings)
    {
        _form = new ScreensaverForm(Screen.PrimaryScreen ?? Screen.AllScreens[0], settings, allowConfigShortcut: true, topMost: false);
        _form.FormClosed += (_, _) => ExitThread();
        _form.Show();
        // Test mode closes via the title-bar X button; no input hook needed.
    }
}
