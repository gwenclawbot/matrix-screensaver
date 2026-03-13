namespace MatrixScreensaver;

internal sealed class ScreensaverForm : Form
{
    private const int ExitThresholdPixels = 10;
    private static readonly TimeSpan StartupGracePeriod = TimeSpan.FromMilliseconds(200);

    private readonly WebViewHost _host = new();
    private readonly ScreensaverSettings _settings;
    private readonly bool _allowConfigShortcut;
    private Point _initialMousePosition;
    private DateTime _shownAtUtc;

    public event EventHandler? ExitRequested;

    public ScreensaverForm(Screen targetScreen, ScreensaverSettings settings, bool allowConfigShortcut, bool topMost = true)
    {
        _settings = settings;
        _allowConfigShortcut = allowConfigShortcut;

        // Test mode gets a normal window with a title bar so it can be closed with the X button.
        if (allowConfigShortcut)
        {
            FormBorderStyle = FormBorderStyle.Sizable;
            StartPosition = FormStartPosition.WindowsDefaultBounds;
            TopMost = false;
            ShowInTaskbar = true;
            Text = "Matrix Screensaver [Test Mode]";
        }
        else
        {
            FormBorderStyle = FormBorderStyle.None;
            StartPosition = FormStartPosition.Manual;
            Bounds = targetScreen.Bounds;
            TopMost = topMost;
            ShowInTaskbar = false;
        }

        KeyPreview = true;
        BackColor = Color.Black;

        Controls.Add(_host);
        _host.Dock = DockStyle.Fill;

        Load += OnLoad;
        Shown += OnShown;
        MouseMove += OnMouseMove;
        MouseDown += (_, _) => RequestExitIfAllowed();
        KeyDown += OnKeyDown;
    }

    private async void OnLoad(object? sender, EventArgs e)
    {
        try
        {
            var appUri = AppPathResolver.ResolveIndexHtmlUri(_settings);
            await _host.InitializeAndNavigateAsync(appUri, isPreview: false);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Matrix Screensaver", MessageBoxButtons.OK, MessageBoxIcon.Error);
            ExitRequested?.Invoke(this, EventArgs.Empty);
        }
    }

    private void OnShown(object? sender, EventArgs e)
    {
        _shownAtUtc = DateTime.UtcNow;
        _initialMousePosition = Cursor.Position;
    }

    private void OnMouseMove(object? sender, MouseEventArgs e)
    {
        RequestExitIfAllowed();
    }

    private void OnKeyDown(object? sender, KeyEventArgs e)
    {
        if (_allowConfigShortcut && e.Control && e.KeyCode == Keys.O)
        {
            using var config = new ConfigForm(new SettingsManager(), _settings);
            config.ShowDialog(this);
            return;
        }

        // Keyboard presses always exit (no mouse threshold check needed).
        RequestExit();
    }

    private void RequestExitIfAllowed()
    {
        if (DateTime.UtcNow - _shownAtUtc < StartupGracePeriod)
        {
            return;
        }

        var current = Cursor.Position;
        var movedTooMuch = Math.Abs(current.X - _initialMousePosition.X) > ExitThresholdPixels ||
                           Math.Abs(current.Y - _initialMousePosition.Y) > ExitThresholdPixels;

        if (!movedTooMuch && MouseButtons == MouseButtons.None)
        {
            return;
        }

        RequestExit();
    }

    private void RequestExit()
    {
        if (DateTime.UtcNow - _shownAtUtc < StartupGracePeriod)
        {
            return;
        }

        ExitRequested?.Invoke(this, EventArgs.Empty);
    }
}
