using System.Runtime.InteropServices;

namespace MatrixScreensaver;

internal sealed class PreviewForm : Form
{
    private readonly IntPtr _parentHandle;
    private readonly WebViewHost _host = new();
    private readonly ScreensaverSettings _settings;
    private readonly System.Windows.Forms.Timer _parentWatchTimer = new() { Interval = 500 };

    public PreviewForm(IntPtr parentHandle, ScreensaverSettings settings)
    {
        _parentHandle = parentHandle;
        _settings = settings;

        FormBorderStyle = FormBorderStyle.None;
        ShowInTaskbar = false;
        StartPosition = FormStartPosition.Manual;
        BackColor = Color.Black;

        Controls.Add(_host);
        _host.Dock = DockStyle.Fill;

        Load += OnLoad;
        Load += (_, _) => _parentWatchTimer.Start();
        FormClosed += (_, _) => _parentWatchTimer.Dispose();
        _parentWatchTimer.Tick += (_, _) =>
        {
            if (!NativeMethods.IsWindow(_parentHandle))
                Close();
        };
    }

    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);

        NativeMethods.SetParent(Handle, _parentHandle);
        NativeMethods.GetClientRect(_parentHandle, out var rect);
        NativeMethods.MoveWindow(Handle, 0, 0, rect.Right - rect.Left, rect.Bottom - rect.Top, true);
    }

    private async void OnLoad(object? sender, EventArgs e)
    {
        try
        {
            var appUri = AppPathResolver.ResolveIndexHtmlUri(_settings);
            await _host.InitializeAndNavigateAsync(appUri, isPreview: true);
        }
        catch
        {
            Close();
        }
    }

    private static class NativeMethods
    {
        [DllImport("user32.dll")]
        public static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool MoveWindow(IntPtr hWnd, int x, int y, int nWidth, int nHeight, bool repaint);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetClientRect(IntPtr hWnd, out Rect lpRect);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsWindow(IntPtr hWnd);
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct Rect
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }
}
