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
        public const int SM_CXICON = 11;
        public const int SM_CYICON = 12;
        public const int SM_CXSMICON = 49;
        public const int SM_CYSMICON = 50;
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
        public const uint SWP_FRAMECHANGED = 0x0020;
        public const uint SWP_SHOWWINDOW = 0x0040;

        public const int GWL_EXSTYLE = -20;
        public const int WS_EX_TOPMOST = 0x00000008;
        public const int WS_EX_TRANSPARENT = 0x00000020;
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

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr FindWindowEx(IntPtr parentHandle, IntPtr childAfter, string? className, string? windowTitle);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

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

        [DllImport("user32.dll", SetLastError = true)]
        public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll")]
        public static extern bool SetLayeredWindowAttributes(IntPtr hwnd, uint crKey, byte bAlpha, uint dwFlags);

        public const uint LWA_ALPHA = 0x00000002;
        public const int WS_EX_LAYERED = 0x00080000;
        public const uint SWP_NOZORDER = 0x0004;

        public static void SetWindowGhost(IntPtr hwnd, bool ghost)
        {
            if (hwnd == IntPtr.Zero) return;
            try
            {
                int exStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
                if (ghost)
                {
                    SetWindowLong(hwnd, GWL_EXSTYLE, exStyle | WS_EX_LAYERED | WS_EX_TRANSPARENT);
                    SetLayeredWindowAttributes(hwnd, 0, 1, LWA_ALPHA); // Alpha = 1: invisible, WS_EX_TRANSPARENT: click-through so user can use their monitor!
                }
                else
                {
                    SetLayeredWindowAttributes(hwnd, 0, 255, LWA_ALPHA);
                    SetWindowLong(hwnd, GWL_EXSTYLE, exStyle & ~(WS_EX_LAYERED | WS_EX_TRANSPARENT));
                }
            }
            catch { }
        }

        [DllImport("dwmapi.dll")]
        public static extern int DwmGetWindowAttribute(IntPtr hwnd, int dwAttribute, out int pvAttribute, int cbAttribute);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr LoadImage(IntPtr hinst, string lpszName, uint uType, int cxDesired, int cyDesired, uint fuLoad);

        [DllImport("user32.dll")]
        public static extern int GetSystemMetrics(int nIndex);

        [DllImport("shell32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern int SetCurrentProcessExplicitAppUserModelID(string AppID);

        public const int WH_KEYBOARD_LL = 13;
        public const int WM_KEYDOWN_MSG = 0x0100;
        public const int WM_SYSKEYDOWN_MSG = 0x0104;

        public delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        [StructLayout(LayoutKind.Sequential)]
        public struct KBDLLHOOKSTRUCT
        {
            public uint vkCode;
            public uint scanCode;
            public uint flags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll")]
        public static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr GetModuleHandle(string? lpModuleName);

        [DllImport("user32.dll")]
        public static extern short GetAsyncKeyState(int vKey);

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

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr OpenProcess(uint processAccess, bool bInheritHandle, int processId);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool QueryFullProcessImageName(IntPtr hProcess, int flags, StringBuilder lpExeName, ref int lpdwSize);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool CloseHandle(IntPtr hObject);

        public const uint PROCESS_QUERY_LIMITED_INFORMATION = 0x1000;

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

                // 2. Query full exe path via OpenProcess with PROCESS_QUERY_LIMITED_INFORMATION
                string? exePath = null;
                IntPtr hProcess = OpenProcess(PROCESS_QUERY_LIMITED_INFORMATION, false, (int)processId);
                if (hProcess != IntPtr.Zero)
                {
                    try
                    {
                        var sb = new StringBuilder(1024);
                        int size = sb.Capacity;
                        if (QueryFullProcessImageName(hProcess, 0, sb, ref size))
                        {
                            exePath = sb.ToString();
                        }
                    }
                    finally
                    {
                        CloseHandle(hProcess);
                    }
                }

                if (string.IsNullOrEmpty(exePath))
                {
                    try
                    {
                        using var proc = Process.GetProcessById((int)processId);
                        exePath = proc.MainModule?.FileName;
                    }
                    catch { }
                }

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

                // 1. Filter out cloaked/invisible UWP virtual windows
                int cloaked = 0;
                DwmGetWindowAttribute(hWnd, DWMWA_CLOAKED, out cloaked, sizeof(int));
                if (cloaked != 0) return true;

                // 2. Filter out zero/tiny windows
                if (GetWindowRect(hWnd, out var rc))
                {
                    if (!IsIconic(hWnd) && (rc.Width <= 30 || rc.Height <= 30))
                        return true;
                }
                else
                {
                    return true;
                }

                // 3. Resolve Process ID and Process Name (handling UWP ApplicationFrameHost)
                GetWindowThreadProcessId(hWnd, out uint pid);
                string procName = "App";
                try
                {
                    using var proc = Process.GetProcessById((int)pid);
                    procName = proc.ProcessName;
                }
                catch { }

                if (procName.Equals("ApplicationFrameHost", StringComparison.OrdinalIgnoreCase))
                {
                    IntPtr childCore = FindWindowEx(hWnd, IntPtr.Zero, "Windows.UI.Core.CoreWindow", null);
                    if (childCore != IntPtr.Zero)
                    {
                        GetWindowThreadProcessId(childCore, out uint childPid);
                        try
                        {
                            using var cProc = Process.GetProcessById((int)childPid);
                            procName = cProc.ProcessName;
                            pid = childPid;
                        }
                        catch { }
                    }
                }

                // 4. Check tool windows (allow WhatsApp call windows even if tool style is set)
                bool isWhatsApp = procName.Contains("WhatsApp", StringComparison.OrdinalIgnoreCase);
                int exStyle = GetWindowLong(hWnd, GWL_EXSTYLE);
                bool isToolWindow = (exStyle & WS_EX_TOOLWINDOW) != 0;

                if (isToolWindow && (!isWhatsApp || rc.Width < 100 || rc.Height < 100))
                    return true;

                // 5. Get window title
                int length = GetWindowTextLength(hWnd);
                string title = string.Empty;
                if (length > 0)
                {
                    StringBuilder sb = new StringBuilder(length + 1);
                    GetWindowText(hWnd, sb, sb.Capacity);
                    title = sb.ToString().Trim();
                }

                // If WhatsApp window has no title but is visible and large enough, name it!
                if (string.IsNullOrWhiteSpace(title))
                {
                    if (isWhatsApp && rc.Width >= 120 && rc.Height >= 120)
                    {
                        var clsSb = new StringBuilder(256);
                        GetClassName(hWnd, clsSb, clsSb.Capacity);
                        string className = clsSb.ToString();
                        title = className.Contains("Call", StringComparison.OrdinalIgnoreCase) || className.Contains("Video", StringComparison.OrdinalIgnoreCase)
                            ? "WhatsApp - Videollamada"
                            : "WhatsApp (Llamada / Vídeo)";
                    }
                    else
                    {
                        return true;
                    }
                }

                if (title == "Program Manager" || title == "Windows Input Experience" || title.StartsWith("AlwaysRotate"))
                    return true;

                // 6. Explicitly identify and format WhatsApp Call & Video Call windows
                if (isWhatsApp || title.Contains("WhatsApp", StringComparison.OrdinalIgnoreCase))
                {
                    procName = "WhatsApp";
                    if (title.Contains("Video call", StringComparison.OrdinalIgnoreCase) ||
                        title.Contains("Videollamada", StringComparison.OrdinalIgnoreCase) ||
                        title.Contains("Llamada", StringComparison.OrdinalIgnoreCase) ||
                        title.Contains("Call", StringComparison.OrdinalIgnoreCase))
                    {
                        if (!title.StartsWith("WhatsApp", StringComparison.OrdinalIgnoreCase))
                        {
                            title = $"WhatsApp - {title}";
                        }
                    }
                }

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

            // Sort so WhatsApp Video Call and WhatsApp appear at the top!
            list.Sort((a, b) =>
            {
                bool aIsCall = a.Title.Contains("WhatsApp", StringComparison.OrdinalIgnoreCase) &&
                              (a.Title.Contains("Call", StringComparison.OrdinalIgnoreCase) ||
                               a.Title.Contains("Llamada", StringComparison.OrdinalIgnoreCase) ||
                               a.Title.Contains("Video", StringComparison.OrdinalIgnoreCase));
                bool bIsCall = b.Title.Contains("WhatsApp", StringComparison.OrdinalIgnoreCase) &&
                              (b.Title.Contains("Call", StringComparison.OrdinalIgnoreCase) ||
                               b.Title.Contains("Llamada", StringComparison.OrdinalIgnoreCase) ||
                               b.Title.Contains("Video", StringComparison.OrdinalIgnoreCase));

                if (aIsCall && !bIsCall) return -1;
                if (!aIsCall && bIsCall) return 1;

                bool aIsWa = a.ProcessName.Contains("WhatsApp", StringComparison.OrdinalIgnoreCase) || a.Title.Contains("WhatsApp", StringComparison.OrdinalIgnoreCase);
                bool bIsWa = b.ProcessName.Contains("WhatsApp", StringComparison.OrdinalIgnoreCase) || b.Title.Contains("WhatsApp", StringComparison.OrdinalIgnoreCase);

                if (aIsWa && !bIsWa) return -1;
                if (!aIsWa && bIsWa) return 1;

                return string.Compare(a.DisplayTitle, b.DisplayTitle, StringComparison.OrdinalIgnoreCase);
            });

            return list;
        }
    }
}
