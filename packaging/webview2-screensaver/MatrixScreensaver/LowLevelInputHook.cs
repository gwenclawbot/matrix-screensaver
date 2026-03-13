using System.Runtime.InteropServices;

namespace MatrixScreensaver;

/// <summary>
/// Installs WH_MOUSE_LL and WH_KEYBOARD_LL hooks so the screensaver can detect
/// input even when WebView2 (a native Win32 child window) has keyboard/mouse focus.
/// </summary>
internal sealed class LowLevelInputHook : IDisposable
{
    private const int WH_KEYBOARD_LL = 13;
    private const int WH_MOUSE_LL = 14;
    private const int WM_KEYDOWN = 0x0100;
    private const int WM_SYSKEYDOWN = 0x0104;
    private const int WM_LBUTTONDOWN = 0x0201;
    private const int WM_RBUTTONDOWN = 0x0204;
    private const int WM_MBUTTONDOWN = 0x0207;
    private const int WM_MOUSEMOVE = 0x0200;

    private readonly Action<Point> _onMouseActivity;
    private readonly Action _onMouseClick;
    private readonly Action _onKeyActivity;

    private readonly NativeMethods.LowLevelHookProc _mouseProc;
    private readonly NativeMethods.LowLevelHookProc _keyboardProc;
    private IntPtr _mouseHook = IntPtr.Zero;
    private IntPtr _keyboardHook = IntPtr.Zero;
    private bool _disposed;

    public LowLevelInputHook(Action<Point> onMouseActivity, Action onMouseClick, Action onKeyActivity)
    {
        _onMouseActivity = onMouseActivity;
        _onMouseClick = onMouseClick;
        _onKeyActivity = onKeyActivity;

        // Keep delegate references alive for the lifetime of this object.
        _mouseProc = MouseHookCallback;
        _keyboardProc = KeyboardHookCallback;

        using var module = System.Diagnostics.Process.GetCurrentProcess().MainModule!;
        var hModule = NativeMethods.GetModuleHandle(module.ModuleName);
        _mouseHook = NativeMethods.SetWindowsHookEx(WH_MOUSE_LL, _mouseProc, hModule, 0);
        _keyboardHook = NativeMethods.SetWindowsHookEx(WH_KEYBOARD_LL, _keyboardProc, hModule, 0);
    }

    private IntPtr MouseHookCallback(int nCode, IntPtr wParam, IntPtr lParam)
    {
        if (nCode >= 0)
        {
            var msg = wParam.ToInt32();
            if (msg == WM_MOUSEMOVE)
            {
                var info = Marshal.PtrToStructure<MSLLHOOKSTRUCT>(lParam);
                _onMouseActivity(new Point(info.pt.x, info.pt.y));
            }
            else if (msg is WM_LBUTTONDOWN or WM_RBUTTONDOWN or WM_MBUTTONDOWN)
            {
                _onMouseClick();
            }
        }
        return NativeMethods.CallNextHookEx(_mouseHook, nCode, wParam, lParam);
    }

    private IntPtr KeyboardHookCallback(int nCode, IntPtr wParam, IntPtr lParam)
    {
        if (nCode >= 0 && (wParam.ToInt32() == WM_KEYDOWN || wParam.ToInt32() == WM_SYSKEYDOWN))
        {
            _onKeyActivity();
        }
        return NativeMethods.CallNextHookEx(_keyboardHook, nCode, wParam, lParam);
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        if (_mouseHook != IntPtr.Zero) { NativeMethods.UnhookWindowsHookEx(_mouseHook); _mouseHook = IntPtr.Zero; }
        if (_keyboardHook != IntPtr.Zero) { NativeMethods.UnhookWindowsHookEx(_keyboardHook); _keyboardHook = IntPtr.Zero; }
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct POINT { public int x, y; }

    [StructLayout(LayoutKind.Sequential)]
    private struct MSLLHOOKSTRUCT
    {
        public POINT pt;
        public uint mouseData, flags, time;
        public IntPtr dwExtraInfo;
    }

    private static class NativeMethods
    {
        public delegate IntPtr LowLevelHookProc(int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr SetWindowsHookEx(int idHook, LowLevelHookProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr GetModuleHandle(string? lpModuleName);
    }
}
