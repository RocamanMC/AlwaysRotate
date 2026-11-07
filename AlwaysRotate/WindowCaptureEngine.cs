using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace AlwaysRotate
{
    public class WindowCaptureEngine : IDisposable
    {
        private int _targetWidth = 0;
        private int _targetHeight = 0;
        private BitmapSource? _currentBitmap;

        public int TargetWidth => _targetWidth;
        public int TargetHeight => _targetHeight;
        public BitmapSource? CurrentBitmap => _currentBitmap;

        public bool Capture(IntPtr hwnd)
        {
            if (hwnd == IntPtr.Zero || !Win32.IsWindowVisible(hwnd))
                return false;

            if (Win32.IsIconic(hwnd))
            {
                Win32.ShowWindow(hwnd, Win32.SW_SHOWNOACTIVATE);
            }

            if (!Win32.GetWindowRect(hwnd, out var rect))
                return false;

            int width = Math.Max(1, rect.Width);
            int height = Math.Max(1, rect.Height);

            _targetWidth = width;
            _targetHeight = height;

            IntPtr hdcScreen = Win32.GetDC(IntPtr.Zero);
            IntPtr hdcMem = Win32.CreateCompatibleDC(hdcScreen);

            // Use CreateCompatibleBitmap
            IntPtr hBmp = CreateCompatibleBitmap(hdcScreen, width, height);
            if (hBmp == IntPtr.Zero)
            {
                Win32.DeleteDC(hdcMem);
                Win32.ReleaseDC(IntPtr.Zero, hdcScreen);
                return false;
            }

            IntPtr hOldBmp = Win32.SelectObject(hdcMem, hBmp);

            // Capture
            bool success = Win32.PrintWindow(hwnd, hdcMem, Win32.PW_RENDERFULLCONTENT);
            if (!success)
            {
                IntPtr hdcWnd = Win32.GetDC(hwnd);
                if (hdcWnd != IntPtr.Zero)
                {
                    Win32.BitBlt(hdcMem, 0, 0, width, height, hdcWnd, 0, 0, Win32.SRCCOPY);
                    Win32.ReleaseDC(hwnd, hdcWnd);
                    success = true;
                }
            }

            if (success)
            {
                try
                {
                    var bmpSource = Imaging.CreateBitmapSourceFromHBitmap(
                        hBmp,
                        IntPtr.Zero,
                        Int32Rect.Empty,
                        BitmapSizeOptions.FromEmptyOptions()
                    );
                    bmpSource.Freeze();
                    _currentBitmap = bmpSource;
                }
                catch (Exception ex)
                {
                    System.IO.File.AppendAllText("E:\\anti\\Utilidades\\bin\\AlwaysRotate\\run.log", $"Imaging error: {ex.Message}\n");
                    success = false;
                }
            }

            // Cleanup GDI objects
            Win32.SelectObject(hdcMem, hOldBmp);
            Win32.DeleteObject(hBmp);
            Win32.DeleteDC(hdcMem);
            Win32.ReleaseDC(IntPtr.Zero, hdcScreen);

            return success;
        }

        [DllImport("gdi32.dll")]
        private static extern IntPtr CreateCompatibleBitmap(IntPtr hdc, int nWidth, int nHeight);

        public void Dispose()
        {
            _currentBitmap = null;
            GC.SuppressFinalize(this);
        }
    }
}
