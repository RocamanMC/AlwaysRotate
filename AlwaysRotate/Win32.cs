using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace AlwaysRotate
{
    public static class Win32
    {
        public const int PW_RENDERFULLCONTENT = 2;
        public const int PW_CLIENTONLY = 1;

        public const uint SW_SHOW = 5;
        public const uint SW_SHOWNOACTIVATE = 4;
        public const uint SW_RESTORE = 9;

        public const int WM_LBUTTONDOWN = 0x0201;
        public const int WM_LBUTTONUP = 0x0202;
        public const int WM_LBUTTONDBLCLK = 0x0203;
        public const int WM_RBUTTONDOWN = 0x0204;
        public const int WM_RBUTTONUP = 0x0205;
        public const int WM_MOUSEMOVE = 0x0200;
        public const int WM_MOUSEWHEEL = 0x020A;
        public const int WM_SETCURSOR = 0x0020;
        public const int WM_ACTIVATE = 0x0006;
        public const int WM_SETICON = 0x0080;
        public const int WA_CLICKACTIVE = 2;
        public const int MK_LBUTTON = 0x0001;
        public const int MK_RBUTTON = 0x0002;

        public const uint MOUSEEVENTF_LEFTDOWN = 0x0002;
        public const uint MOUSEEVENTF_LEFTUP = 0x0004;
        public const uint MOUSEEVENTF_RIGHTDOWN = 0x0008;
        public const uint MOUSEEVENTF_RIGHTUP = 0x0010;
        public const uint MOUSEEVENTF_WHEEL = 0x0800;

        public const int MOD_ALT = 0x0001;
        public const int MOD_CONTROL = 0x0002;
        public const int MOD_SHIFT = 0x0004;
        public const int MOD_WIN = 0x0008;
        public const int WM_HOTKEY = 0x0312;

        public static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        public static readonly IntPtr HWND_NOTOPMOST = new IntPtr(-2);
        public const uint SWP_NOSIZE = 0x0001;
        public const uint SWP_NOMOVE = 0x0002;
        public const uint SWP_NOACTIVATE = 0x0010;
        public const uint SWP_SHOWWINDOW = 0x0040;

        public const int GWL_EXSTYLE = -20;
        public const int WS_EX_TOPMOST = 0x00000008;
        public const int WS_EX_TOOLWINDOW = 0x00000080;

        public const int DWMWA_CLOAKED = 14;

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;

            public int Width => Right - Left;
            public int Height => Bottom - Top;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;
        }

        public delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern bool IsIconic(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr hWnd, uint nCmdShow);

        [DllImport("user32.dll")]
        public static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        [DllImport("user32.dll")]
        public static extern bool PrintWindow(IntPtr hWnd, IntPtr hdcBlt, uint nFlags);

        [DllImport("user32.dll")]
        public static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateCompatibleDC(IntPtr hDC);

        [DllImport("gdi32.dll")]
        public static extern bool DeleteDC(IntPtr hDC);

        [DllImport("gdi32.dll")]
        public static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject);

        [DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);

        [DllImport("gdi32.dll")]
        public static extern bool BitBlt(IntPtr hdcDest, int nXDest, int nYDest, int nWidth, int nHeight, IntPtr hdcSrc, int nXSrc, int nYSrc, uint dwRop);

        public const uint SRCCOPY = 0x00CC0020;

        [DllImport("user32.dll")]
        public static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern bool ScreenToClient(IntPtr hWnd, ref POINT lpPoint);

        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out POINT lpPoint);

        [DllImport("user32.dll")]
        public static extern bool SetCursorPos(int X, int Y);

        [DllImport("user32.dll")]
        public static extern void mouse_event(uint dwFlags, int dx, int dy, uint dwData, UIntPtr dwExtraInfo);

        [DllImport("user32.dll")]
        public static extern IntPtr WindowFromPoint(POINT Point);

        [DllImport("user32.dll")]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        [DllImport("user32.dll")]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        [DllImport("user32.dll")]
        public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("dwmapi.dll")]
        public static extern int DwmGetWindowAttribute(IntPtr hwnd, int dwAttribute, out int pvAttribute, int cbAttribute);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr LoadImage(IntPtr hinst, string lpszName, uint uType, int cxDesired, int cyDesired, uint fuLoad);

        public static void EnforceTopmost(IntPtr hwnd, bool enable)
        {
            if (hwnd == IntPtr.Zero) return;
            IntPtr flag = enable ? HWND_TOPMOST : HWND_NOTOPMOST;
            SetWindowPos(hwnd, flag, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_NOACTIVATE | SWP_SHOWWINDOW);
        }

        public static bool IsTopmost(IntPtr hwnd)
        {
            if (hwnd == IntPtr.Zero) return false;
            int exStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
            return (exStyle & WS_EX_TOPMOST) != 0;
        }

        public static IntPtr MakeLParam(int loWord, int hiWord)
        {
            return (IntPtr)((hiWord << 16) | (loWord & 0xFFFF));
        }

        public class WindowItem
        {
            public IntPtr Handle { get; set; }
            public string Title { get; set; } = string.Empty;
            public string ProcessName { get; set; } = string.Empty;
            public uint ProcessId { get; set; }
            public ImageSource? RealIcon { get; set; }

            public string DisplayTitle => string.IsNullOrWhiteSpace(Title) ? $"[{ProcessName}]" : Title;
            public string DisplayBadge => ProcessName.ToUpperInvariant();

            public override string ToString() => $"{DisplayTitle} ({ProcessName})";
        }

        public static ImageSource? ExtractRealIcon(IntPtr hWnd, uint processId)
        {
            try
            {
                // 1. Try WM_GETICON
                IntPtr hIcon = SendMessage(hWnd, 0x007F /* WM_GETICON */, (IntPtr)0 /* ICON_SMALL */, IntPtr.Zero);
                if (hIcon == IntPtr.Zero)
                    hIcon = SendMessage(hWnd, 0x007F, (IntPtr)1 /* ICON_BIG */, IntPtr.Zero);

                if (hIcon != IntPtr.Zero)
                {
                    var bmp = Imaging.CreateBitmapSourceFromHIcon(hIcon, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                    bmp.Freeze();
                    return bmp;
                }

                // 2. Extract from Process Executable
                using var proc = Process.GetProcessById((int)processId);
                string? exePath = proc.MainModule?.FileName;
                if (!string.IsNullOrEmpty(exePath) && System.IO.File.Exists(exePath))
                {
                    using var sysIcon = System.Drawing.Icon.ExtractAssociatedIcon(exePath);
                    if (sysIcon != null)
                    {
                        var bmp = Imaging.CreateBitmapSourceFromHIcon(sysIcon.Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                        bmp.Freeze();
                        return bmp;
                    }
                }
            }
            catch { }
            return null;
        }

        public static List<WindowItem> GetOpenWindows(IntPtr excludeHwnd)
        {
            var list = new List<WindowItem>();

            EnumWindows((hWnd, lParam) =>
            {
                if (hWnd == excludeHwnd) return true;
                if (!IsWindowVisible(hWnd)) return true;

                // 1. Filter out cloaked/invisible UWP windows
                int cloaked = 0;
                DwmGetWindowAttribute(hWnd, DWMWA_CLOAKED, out cloaked, sizeof(int));
                if (cloaked != 0) return true;

                // 2. Filter out tool windows
                int exStyle = GetWindowLong(hWnd, GWL_EXSTYLE);
                if ((exStyle & WS_EX_TOOLWINDOW) != 0) return true;

                int length = GetWindowTextLength(hWnd);
                if (length == 0) return true;

                StringBuilder sb = new StringBuilder(length + 1);
                GetWindowText(hWnd, sb, sb.Capacity);
                string title = sb.ToString().Trim();

                if (string.IsNullOrWhiteSpace(title)) return true;

                if (title == "Program Manager" || title == "Windows Input Experience" || title.StartsWith("AlwaysRotate"))
                    return true;

                GetWindowThreadProcessId(hWnd, out uint pid);
                string procName = "App";
                try
                {
                    using var proc = Process.GetProcessById((int)pid);
                    procName = proc.ProcessName;
                }
                catch { }

                // Filter out internal background windows
                if (procName.Equals("ApplicationFrameHost", StringComparison.OrdinalIgnoreCase) && string.IsNullOrWhiteSpace(title))
                    return true;

                var realIcon = ExtractRealIcon(hWnd, pid);

                list.Add(new WindowItem
                {
                    Handle = hWnd,
                    Title = title,
                    ProcessName = procName,
                    ProcessId = pid,
                    RealIcon = realIcon
                });

                return true;
            }, IntPtr.Zero);

            return list;
        }
    }
}
