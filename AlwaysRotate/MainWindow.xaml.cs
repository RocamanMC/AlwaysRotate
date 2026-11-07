using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace AlwaysRotate
{
    public partial class MainWindow : Window
    {
        private const int HOTKEY_ID = 9001;

        private readonly WindowCaptureEngine _engine = new WindowCaptureEngine();
        private readonly DispatcherTimer _captureTimer = new DispatcherTimer();

        private IntPtr _targetHwnd = IntPtr.Zero;
        private string _targetTitle = string.Empty;
        private int _currentAngle = 90;
        private bool _isAlwaysOnTop = true;
        private bool _isInteractive = true;
        private bool _isUpdatingSelection = false;
        private bool _isGhostEnabled = false;
        private IntPtr _ghostedHwnd = IntPtr.Zero;
        private volatile bool _isSimulatingInput = false;

        private HwndSource? _hwndSource;

        public MainWindow()
        {
            InitializeComponent();

            _captureTimer.Interval = TimeSpan.FromMilliseconds(33); // ~30 FPS
            _captureTimer.Tick += CaptureTimer_Tick;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            IntPtr handle = new WindowInteropHelper(this).Handle;
            _hwndSource = HwndSource.FromHwnd(handle);
            _hwndSource?.AddHook(HwndHook);

            // Register global hotkey Ctrl + Alt + R
            Win32.RegisterHotKey(handle, HOTKEY_ID, (uint)(Win32.MOD_CONTROL | Win32.MOD_ALT), 0x52);

            // Set Taskbar and Window Icon explicitly via Win32 WM_SETICON
            try
            {
                string icoPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "app_icon.ico");
                if (!System.IO.File.Exists(icoPath))
                {
                    var resUri = new Uri("pack://application:,,,/app_icon.ico", UriKind.RelativeOrAbsolute);
                    var streamInfo = Application.GetResourceStream(resUri);
                    if (streamInfo != null)
                    {
                        using (var fs = System.IO.File.Create(icoPath))
                        {
                            streamInfo.Stream.CopyTo(fs);
                        }
                    }
                }

                if (System.IO.File.Exists(icoPath))
                {
                    IntPtr hIconBig = Win32.LoadImage(IntPtr.Zero, icoPath, 1, 32, 32, 0x0010);
                    IntPtr hIconSm = Win32.LoadImage(IntPtr.Zero, icoPath, 1, 16, 16, 0x0010);
                    if (hIconBig != IntPtr.Zero) Win32.SendMessage(handle, Win32.WM_SETICON, (IntPtr)1, hIconBig);
                    if (hIconSm != IntPtr.Zero) Win32.SendMessage(handle, Win32.WM_SETICON, (IntPtr)0, hIconSm);

                    this.Icon = BitmapFrame.Create(new Uri(icoPath, UriKind.Absolute));
                }
            }
            catch { }

            ApplyAngle(_currentAngle);
            UpdatePinButtonStyle();
            UpdateInteractButtonStyle();
            UpdateGhostButtonStyle();
            RefreshWindowsList();

            // Auto-select WhatsApp if running
            SelectInitialWindow();

            EnforceAlwaysOnTop();
            _captureTimer.Start();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _captureTimer.Stop();

            // Always restore ghosted original window
            if (_ghostedHwnd != IntPtr.Zero)
            {
                Win32.SetWindowGhost(_ghostedHwnd, false);
            }
            if (_targetHwnd != IntPtr.Zero)
            {
                Win32.SetWindowGhost(_targetHwnd, false);
            }

            IntPtr handle = new WindowInteropHelper(this).Handle;
            Win32.UnregisterHotKey(handle, HOTKEY_ID);

            if (_hwndSource != null)
            {
                _hwndSource.RemoveHook(HwndHook);
                _hwndSource = null;
            }

            _engine.Dispose();
        }

        private void Window_Deactivated(object? sender, EventArgs e)
        {
            if (_isAlwaysOnTop)
            {
                EnforceAlwaysOnTop();
            }
        }

        private void EnforceAlwaysOnTop()
        {
            IntPtr handle = new WindowInteropHelper(this).Handle;
            if (handle != IntPtr.Zero)
            {
                Topmost = _isAlwaysOnTop;
                Win32.EnforceTopmost(handle, _isAlwaysOnTop);
            }
        }

        private IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == Win32.WM_HOTKEY && wParam.ToInt32() == HOTKEY_ID)
            {
                IntPtr fgHwnd = Win32.GetForegroundWindow();
                IntPtr myHwnd = new WindowInteropHelper(this).Handle;

                if (fgHwnd != IntPtr.Zero && fgHwnd != myHwnd)
                {
                    SetTarget(fgHwnd);
                    handled = true;
                }
            }
            return IntPtr.Zero;
        }

        private void CmbWindows_DropDownOpened(object? sender, EventArgs e)
        {
            RefreshWindowsList();
        }

        private void RefreshWindowsList()
        {
            IntPtr myHwnd = new WindowInteropHelper(this).Handle;
            var windows = Win32.GetOpenWindows(myHwnd);

            _isUpdatingSelection = true;
            try
            {
                CmbWindows.ItemsSource = windows;

                if (_targetHwnd != IntPtr.Zero)
                {
                    var current = windows.FirstOrDefault(w => w.Handle == _targetHwnd);
                    if (current != null)
                    {
                        CmbWindows.SelectedItem = current;
                    }
                }
            }
            finally
            {
                _isUpdatingSelection = false;
            }
        }

        private void SelectInitialWindow()
        {
            if (CmbWindows.ItemsSource is IEnumerable<Win32.WindowItem> items)
            {
                var wa = items.FirstOrDefault(w =>
                    w.Title.Contains("WhatsApp", StringComparison.OrdinalIgnoreCase) ||
                    w.ProcessName.Contains("WhatsApp", StringComparison.OrdinalIgnoreCase));

                if (wa != null)
                {
                    SetTarget(wa.Handle);
                    return;
                }

                var first = items.FirstOrDefault();
                if (first != null)
                {
                    SetTarget(first.Handle);
                }
            }
        }

        public void SetTarget(IntPtr hwnd)
        {
            if (hwnd == IntPtr.Zero) return;
            if (_targetHwnd == hwnd && _engine.CurrentBitmap != null) return;

            // If we previously ghosted another window, restore it
            if (_ghostedHwnd != IntPtr.Zero && _ghostedHwnd != hwnd)
            {
                Win32.SetWindowGhost(_ghostedHwnd, false);
                _ghostedHwnd = IntPtr.Zero;
            }

            _targetHwnd = hwnd;
            _targetTitle = "Ventana";

            // If minimized, restore so it physically opens on screen!
            if (Win32.IsIconic(_targetHwnd))
            {
                Win32.ShowWindow(_targetHwnd, Win32.SW_RESTORE);
            }
            else if (!Win32.IsWindowVisible(_targetHwnd))
            {
                Win32.ShowWindow(_targetHwnd, Win32.SW_SHOW);
            }

            if (_isGhostEnabled)
            {
                ApplyGhostState();
            }

            _isUpdatingSelection = true;
            try
            {
                if (CmbWindows.ItemsSource is IEnumerable<Win32.WindowItem> items)
                {
                    var match = items.FirstOrDefault(w => w.Handle == hwnd);
                    if (match != null)
                    {
                        CmbWindows.SelectedItem = match;
                        _targetTitle = match.Title;
                    }
                }
            }
            finally
            {
                _isUpdatingSelection = false;
            }

            TxtStatus.Text = $"Conectado a: {_targetTitle}";
            PlaceholderOverlay.Visibility = Visibility.Collapsed;
        }

        private void CaptureTimer_Tick(object? sender, EventArgs e)
        {
            if (_targetHwnd == IntPtr.Zero)
            {
                PlaceholderOverlay.Visibility = Visibility.Visible;
                return;
            }

            if (_isAlwaysOnTop)
            {
                IntPtr handle = new WindowInteropHelper(this).Handle;
                if (!Win32.IsTopmost(handle))
                {
                    Win32.EnforceTopmost(handle, true);
                }
            }

            if (!Win32.IsWindowVisible(_targetHwnd) || Win32.IsIconic(_targetHwnd))
            {
                Win32.ShowWindow(_targetHwnd, Win32.SW_RESTORE);
            }

            bool captured = _engine.Capture(_targetHwnd, _currentAngle);
            if (captured && _engine.CurrentBitmap != null)
            {
                if (RotatedImageHost.Source != _engine.CurrentBitmap)
                {
                    RotatedImageHost.Source = _engine.CurrentBitmap;
                }
                PlaceholderOverlay.Visibility = Visibility.Collapsed;
            }
            else
            {
                TxtStatus.Text = $"Esperando señal de: {_targetTitle}";
            }
        }

        private void CmbWindows_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_isUpdatingSelection) return;

            if (CmbWindows.SelectedItem is Win32.WindowItem item)
            {
                if (item.Handle != _targetHwnd)
                {
                    SetTarget(item.Handle);
                }
            }
        }

        private void BtnRefreshWindows_Click(object sender, RoutedEventArgs e)
        {
            RefreshWindowsList();
            TxtStatus.Text = "Lista de ventanas actualizada";
        }

        private void BtnPin_Click(object sender, RoutedEventArgs e)
        {
            _isAlwaysOnTop = !_isAlwaysOnTop;
            EnforceAlwaysOnTop();
            UpdatePinButtonStyle();

            TxtStatus.Text = _isAlwaysOnTop
                ? "Siempre Encima: ACTIVADO (Fijo sobre otras apps)"
                : "Siempre Encima: DESACTIVADO (Ventana normal)";
        }

        private void UpdatePinButtonStyle()
        {
            if (_isAlwaysOnTop)
            {
                BtnPin.Background = new SolidColorBrush(Color.FromRgb(45, 32, 6));
                BtnPin.BorderBrush = new SolidColorBrush(Color.FromRgb(180, 83, 9));
                IcoPin.Foreground = new SolidColorBrush(Color.FromRgb(251, 191, 36));
                TxtPin.Foreground = new SolidColorBrush(Color.FromRgb(251, 191, 36));
                TxtPin.Text = "Siempre Encima (ON)";
            }
            else
            {
                BtnPin.Background = new SolidColorBrush(Color.FromRgb(30, 30, 38));
                BtnPin.BorderBrush = new SolidColorBrush(Color.FromRgb(51, 51, 66));
                IcoPin.Foreground = new SolidColorBrush(Color.FromRgb(113, 113, 122));
                TxtPin.Foreground = new SolidColorBrush(Color.FromRgb(113, 113, 122));
                TxtPin.Text = "Siempre Encima (OFF)";
            }
        }

        private void BtnInteract_Click(object sender, RoutedEventArgs e)
        {
            _isInteractive = !_isInteractive;
            UpdateInteractButtonStyle();

            TxtStatus.Text = _isInteractive
                ? "Modo Interactivo: Tus clics y scroll se envían a la app original"
                : "Modo Solo Ver: Haz clic para arrastrar la ventana";
        }

        private void UpdateInteractButtonStyle()
        {
            if (_isInteractive)
            {
                BtnInteract.Background = new SolidColorBrush(Color.FromRgb(6, 40, 30));
                BtnInteract.BorderBrush = new SolidColorBrush(Color.FromRgb(4, 120, 87));
                IcoInteract.Foreground = new SolidColorBrush(Color.FromRgb(52, 211, 153));
                TxtInteract.Foreground = new SolidColorBrush(Color.FromRgb(52, 211, 153));
                TxtInteract.Text = "Interactivo (Clics ON)";
            }
            else
            {
                BtnInteract.Background = new SolidColorBrush(Color.FromRgb(30, 30, 38));
                BtnInteract.BorderBrush = new SolidColorBrush(Color.FromRgb(51, 51, 66));
                IcoInteract.Foreground = new SolidColorBrush(Color.FromRgb(113, 113, 122));
                TxtInteract.Foreground = new SolidColorBrush(Color.FromRgb(113, 113, 122));
                TxtInteract.Text = "Solo Ver (Clics OFF)";
            }
        }

        private void BtnGhost_Click(object sender, RoutedEventArgs e)
        {
            _isGhostEnabled = !_isGhostEnabled;
            ApplyGhostState();
            UpdateGhostButtonStyle();
        }

        private void ApplyGhostState()
        {
            if (_targetHwnd == IntPtr.Zero) return;

            if (_isGhostEnabled)
            {
                _ghostedHwnd = _targetHwnd;
                Win32.SetWindowGhost(_targetHwnd, true);
                TxtStatus.Text = "Ventana original: OCULTA (100% invisible para tus ojos)";
            }
            else
            {
                if (_ghostedHwnd != IntPtr.Zero)
                {
                    Win32.SetWindowGhost(_ghostedHwnd, false);
                    _ghostedHwnd = IntPtr.Zero;
                }
                Win32.SetWindowGhost(_targetHwnd, false);
                TxtStatus.Text = "Ventana original: VISIBLE";
            }
        }

        private void UpdateGhostButtonStyle()
        {
            if (_isGhostEnabled)
            {
                BtnGhost.Background = new SolidColorBrush(Color.FromRgb(58, 20, 80));
                BtnGhost.BorderBrush = new SolidColorBrush(Color.FromRgb(168, 85, 247));
                IcoGhost.Foreground = new SolidColorBrush(Color.FromRgb(216, 180, 254));
                TxtGhost.Foreground = new SolidColorBrush(Color.FromRgb(216, 180, 254));
                TxtGhost.Text = "Oculta (ON)";
            }
            else
            {
                BtnGhost.Background = new SolidColorBrush(Color.FromRgb(30, 30, 38));
                BtnGhost.BorderBrush = new SolidColorBrush(Color.FromRgb(51, 51, 66));
                IcoGhost.Foreground = new SolidColorBrush(Color.FromRgb(113, 113, 122));
                TxtGhost.Foreground = new SolidColorBrush(Color.FromRgb(113, 113, 122));
                TxtGhost.Text = "Ocultar Real";
            }
        }

        private void ApplyAngle(int angle)
        {
            _currentAngle = ((angle % 360) + 360) % 360;
            TxtAngleBadge.Text = $"{_currentAngle}°";
            UpdateRotationButtonStyles();
            TxtStatus.Text = $"Rotación fijada en: {_currentAngle}°";
        }

        private void UpdateRotationButtonStyles()
        {
            void StyleBtn(Button btn, bool isActive)
            {
                if (isActive)
                {
                    btn.Background = new SolidColorBrush(Color.FromRgb(79, 70, 229));
                    btn.BorderBrush = new SolidColorBrush(Color.FromRgb(99, 102, 241));
                    btn.Foreground = Brushes.White;
                }
                else
                {
                    btn.Background = new SolidColorBrush(Color.FromRgb(30, 30, 38));
                    btn.BorderBrush = new SolidColorBrush(Color.FromRgb(51, 51, 66));
                    btn.Foreground = new SolidColorBrush(Color.FromRgb(228, 228, 231));
                }
            }

            StyleBtn(BtnRot0, _currentAngle == 0);
            StyleBtn(BtnRotCW, _currentAngle == 90);
            StyleBtn(BtnRot180, _currentAngle == 180);
            StyleBtn(BtnRotCCW, _currentAngle == 270);
        }

        private void BtnRotateCCW_Click(object sender, RoutedEventArgs e) => ApplyAngle(_currentAngle - 90);
        private void BtnRotateCW_Click(object sender, RoutedEventArgs e) => ApplyAngle(_currentAngle + 90);
        private void BtnRotate0_Click(object sender, RoutedEventArgs e) => ApplyAngle(0);
        private void BtnRotate180_Click(object sender, RoutedEventArgs e) => ApplyAngle(180);

        private void SliderOpacity_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Opacity = e.NewValue;
            if (TxtOpacityVal != null)
            {
                TxtOpacityVal.Text = $"{(int)(e.NewValue * 100)}%";
            }
        }

        #region Interaction & Click Forwarding
        private bool CalculateTargetCoordinates(Point clickPoint, out int targetX, out int targetY)
        {
            targetX = 0;
            targetY = 0;

            if (_targetHwnd == IntPtr.Zero || _engine.TargetWidth <= 0 || _engine.TargetHeight <= 0)
                return false;

            double actualW = RotatedImageHost.ActualWidth;
            double actualH = RotatedImageHost.ActualHeight;

            if (actualW <= 0 || actualH <= 0)
                return false;

            // If click is outside the bounds of the image (in letterbox margins), return false
            if (clickPoint.X < 0 || clickPoint.X > actualW || clickPoint.Y < 0 || clickPoint.Y > actualH)
                return false;

            double u = Math.Clamp(clickPoint.X / actualW, 0.0, 1.0);
            double v = Math.Clamp(clickPoint.Y / actualH, 0.0, 1.0);

            switch (_currentAngle)
            {
                case 90:
                    targetX = (int)(v * _engine.TargetWidth);
                    targetY = (int)((1.0 - u) * _engine.TargetHeight);
                    break;
                case 180:
                    targetX = (int)((1.0 - u) * _engine.TargetWidth);
                    targetY = (int)((1.0 - v) * _engine.TargetHeight);
                    break;
                case 270:
                    targetX = (int)((1.0 - v) * _engine.TargetWidth);
                    targetY = (int)(u * _engine.TargetHeight);
                    break;
                case 0:
                default:
                    targetX = (int)(u * _engine.TargetWidth);
                    targetY = (int)(v * _engine.TargetHeight);
                    break;
            }

            return true;
        }

        private bool _isMouseDownOnImage = false;
        private readonly object _clickLock = new object();

        private void ForwardMouseClick(Point clickPoint, bool isRightClick)
        {
            if (_targetHwnd == IntPtr.Zero) return;

            if (!CalculateTargetCoordinates(clickPoint, out int tx, out int ty))
                return;

            if (Win32.IsIconic(_targetHwnd))
            {
                Win32.ShowWindow(_targetHwnd, Win32.SW_RESTORE);
                Thread.Sleep(40);
            }

            if (!Win32.GetWindowRect(_targetHwnd, out var rect))
                return;

            int screenX = rect.Left + tx;
            int screenY = rect.Top + ty;

            IntPtr myHwnd = new WindowInteropHelper(this).Handle;
            Win32.GetWindowRect(myHwnd, out var myRect);
            bool isCoveredByMe = (screenX >= myRect.Left && screenX <= myRect.Right && screenY >= myRect.Top && screenY <= myRect.Bottom);

            ThreadPool.QueueUserWorkItem(_ =>
            {
                lock (_clickLock)
                {
                    _isSimulatingInput = true;
                    int originalExStyle = 0;
                    try
                    {
                        if (isCoveredByMe)
                        {
                            // If AlwaysRotate is physically sitting on top of the target, make it click-transparent
                            originalExStyle = Win32.GetWindowLong(myHwnd, Win32.GWL_EXSTYLE);
                            Win32.SetWindowLong(myHwnd, Win32.GWL_EXSTYLE, originalExStyle | Win32.WS_EX_TRANSPARENT);
                        }

                        Win32.GetCursorPos(out var oldCursor);

                        // 1. Activate target window so it receives the click
                        Win32.SetForegroundWindow(_targetHwnd);
                        Thread.Sleep(25);

                        // 2. Position cursor exactly over the control in target window
                        Win32.SetCursorPos(screenX, screenY);
                        Thread.Sleep(15);

                        uint downFlag = isRightClick ? Win32.MOUSEEVENTF_RIGHTDOWN : Win32.MOUSEEVENTF_LEFTDOWN;
                        uint upFlag = isRightClick ? Win32.MOUSEEVENTF_RIGHTUP : Win32.MOUSEEVENTF_LEFTUP;

                        // 3. Dispatch real hardware mouse down + hold + up
                        Win32.mouse_event(downFlag, 0, 0, 0, UIntPtr.Zero);
                        Thread.Sleep(45);
                        Win32.mouse_event(upFlag, 0, 0, 0, UIntPtr.Zero);
                        Thread.Sleep(25);

                        // 4. Return cursor to original position
                        Win32.SetCursorPos(oldCursor.X, oldCursor.Y);
                    }
                    finally
                    {
                        if (isCoveredByMe)
                        {
                            Win32.SetWindowLong(myHwnd, Win32.GWL_EXSTYLE, originalExStyle);
                        }
                        Thread.Sleep(20);
                        _isSimulatingInput = false;
                    }
                }
            });
        }

        private void ViewportContainer_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (_isSimulatingInput) return;

            if (!_isInteractive)
            {
                if (e.ButtonState == MouseButtonState.Pressed)
                {
                    DragMove();
                }
                return;
            }

            _isMouseDownOnImage = true;
            e.Handled = true;
        }

        private void ViewportContainer_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_isSimulatingInput || !_isInteractive || !_isMouseDownOnImage)
                return;

            _isMouseDownOnImage = false;
            e.Handled = true;

            // Release capture so mouse is completely free
            Mouse.Capture(null);

            Point pt = e.GetPosition(RotatedImageHost);
            ForwardMouseClick(pt, isRightClick: false);
        }

        private void ViewportContainer_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (_isInteractive)
            {
                e.Handled = true;
            }
        }

        private void ViewportContainer_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_isSimulatingInput || !_isInteractive)
                return;

            e.Handled = true;
            Mouse.Capture(null);
            Point pt = e.GetPosition(RotatedImageHost);
            ForwardMouseClick(pt, isRightClick: true);
        }

        private void ViewportContainer_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (_isSimulatingInput) return;

            if (_isInteractive && _targetHwnd != IntPtr.Zero)
            {
                Point pt = e.GetPosition(RotatedImageHost);
                if (CalculateTargetCoordinates(pt, out int tx, out int ty) && Win32.GetWindowRect(_targetHwnd, out var rect))
                {
                    int screenX = rect.Left + tx;
                    int screenY = rect.Top + ty;

                    // Direct message posting: zero lag, zero cursor jitter, zero thread collisions
                    IntPtr wParam = (IntPtr)((e.Delta << 16) & 0xFFFF0000);
                    IntPtr lParam = Win32.MakeLParam((short)screenX, (short)screenY);

                    Win32.PostMessage(_targetHwnd, Win32.WM_MOUSEWHEEL, wParam, lParam);

                    // Also check for specific child control at coordinates (e.g., chat list in WhatsApp)
                    IntPtr ptChild = Win32.WindowFromPoint(new Win32.POINT { X = screenX, Y = screenY });
                    if (ptChild != IntPtr.Zero && ptChild != _targetHwnd)
                    {
                        Win32.PostMessage(ptChild, Win32.WM_MOUSEWHEEL, wParam, lParam);
                    }
                }
                e.Handled = true;
            }
        }
        #endregion
    }
}
