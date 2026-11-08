using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Microsoft.Win32;

namespace AlwaysRotate
{
    public partial class MainWindow : Window
    {
        private const int HOTKEY_ID = 9001;
        private const string RUN_REG_KEY = @"Software\Microsoft\Windows\CurrentVersion\Run";
        private const string APP_NAME = "AlwaysRotate";
        private static readonly string ConfigPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json");

        private readonly WindowCaptureEngine _engine = new WindowCaptureEngine();
        private readonly DispatcherTimer _captureTimer = new DispatcherTimer();
        private readonly DispatcherTimer _hudFadeTimer = new DispatcherTimer();
        private Win32.LowLevelKeyboardProc? _keyboardProc;
        private IntPtr _keyboardHookId = IntPtr.Zero;

        private IntPtr _targetHwnd = IntPtr.Zero;
        private string _targetTitle = string.Empty;
        private int _currentAngle = 90;
        private bool _isAlwaysOnTop = true;
        private bool _isInteractive = true;
        private bool _isUpdatingSelection = false;
        private bool _isGhostEnabled = false;
        private IntPtr _ghostedHwnd = IntPtr.Zero;
        private volatile bool _isSimulatingInput = false;
        private bool _rememberState = true;
        private string _lastRememberedTarget = string.Empty;
        private int _targetFps = 60;
        private string _scalingMode = "HighQuality";
        private string _currentTheme = "Indigo";

        public record ThemeDefinition(
            string Id,
            string Name,
            Color WindowBg,
            Color HeaderBg,
            Color HeaderBorder,
            Color FooterBg,
            Color AccentDark,
            Color AccentBorder,
            Color AccentLight,
            Color BadgeBg,
            Color BadgeBorder,
            Color HudBorder
        );

        private static readonly Dictionary<string, ThemeDefinition> AvailableThemes = new(StringComparer.OrdinalIgnoreCase)
        {
            ["Indigo"] = new(
                "Indigo",
                "Índigo Nocturno",
                Color.FromRgb(12, 13, 20),
                Color.FromRgb(18, 19, 31),
                Color.FromRgb(32, 34, 53),
                Color.FromRgb(16, 17, 27),
                Color.FromRgb(79, 70, 229),
                Color.FromRgb(99, 102, 241),
                Color.FromRgb(165, 180, 252),
                Color.FromRgb(30, 27, 75),
                Color.FromRgb(55, 48, 163),
                Color.FromRgb(79, 70, 229)
            ),
            ["Emerald"] = new(
                "Emerald",
                "Esmeralda Cíber",
                Color.FromRgb(10, 16, 13),
                Color.FromRgb(15, 25, 20),
                Color.FromRgb(25, 48, 38),
                Color.FromRgb(12, 20, 16),
                Color.FromRgb(5, 150, 105),
                Color.FromRgb(16, 185, 129),
                Color.FromRgb(110, 231, 183),
                Color.FromRgb(6, 78, 59),
                Color.FromRgb(4, 120, 87),
                Color.FromRgb(5, 150, 105)
            ),
            ["Amethyst"] = new(
                "Amethyst",
                "Amatista Neón",
                Color.FromRgb(14, 10, 20),
                Color.FromRgb(23, 16, 34),
                Color.FromRgb(46, 26, 69),
                Color.FromRgb(18, 12, 28),
                Color.FromRgb(147, 51, 234),
                Color.FromRgb(168, 85, 247),
                Color.FromRgb(216, 180, 254),
                Color.FromRgb(59, 7, 100),
                Color.FromRgb(107, 33, 168),
                Color.FromRgb(147, 51, 234)
            ),
            ["Crimson"] = new(
                "Crimson",
                "Carmesí Rubí",
                Color.FromRgb(19, 9, 11),
                Color.FromRgb(28, 13, 16),
                Color.FromRgb(56, 23, 30),
                Color.FromRgb(23, 10, 13),
                Color.FromRgb(220, 38, 38),
                Color.FromRgb(239, 68, 68),
                Color.FromRgb(252, 165, 165),
                Color.FromRgb(69, 10, 10),
                Color.FromRgb(153, 27, 27),
                Color.FromRgb(220, 38, 38)
            ),
            ["Amber"] = new(
                "Amber",
                "Ámbar Dorado",
                Color.FromRgb(17, 13, 6),
                Color.FromRgb(28, 20, 9),
                Color.FromRgb(61, 42, 16),
                Color.FromRgb(23, 16, 8),
                Color.FromRgb(217, 119, 6),
                Color.FromRgb(245, 158, 11),
                Color.FromRgb(253, 230, 138),
                Color.FromRgb(69, 26, 3),
                Color.FromRgb(146, 64, 14),
                Color.FromRgb(217, 119, 6)
            ),
            ["Glacier"] = new(
                "Glacier",
                "Glaciar Nórdico",
                Color.FromRgb(8, 16, 20),
                Color.FromRgb(12, 24, 31),
                Color.FromRgb(20, 48, 62),
                Color.FromRgb(10, 19, 26),
                Color.FromRgb(8, 145, 178),
                Color.FromRgb(6, 182, 212),
                Color.FromRgb(103, 232, 249),
                Color.FromRgb(22, 78, 99),
                Color.FromRgb(14, 116, 144),
                Color.FromRgb(8, 145, 178)
            )
        };

        public enum ViewState
        {
            Normal,
            WindowZen,
            Fullscreen
        }

        private ViewState _currentView = ViewState.Normal;
        private bool _isFullscreen => _currentView == ViewState.Fullscreen;
        private WindowState _prevWindowState = WindowState.Normal;
        private WindowStyle _prevWindowStyle = WindowStyle.SingleBorderWindow;
        private ResizeMode _prevResizeMode = ResizeMode.CanResize;

        private HwndSource? _hwndSource;

        public MainWindow()
        {
            InitializeComponent();

            _captureTimer.Interval = TimeSpan.FromMilliseconds(16); // 60 FPS por defecto
            _captureTimer.Tick += CaptureTimer_Tick;

            _hudFadeTimer.Interval = TimeSpan.FromSeconds(2.5);
            _hudFadeTimer.Tick += (s, e) =>
            {
                if (_currentView != ViewState.Normal && !FullscreenHud.IsMouseOver)
                {
                    var anim = new System.Windows.Media.Animation.DoubleAnimation(0.0, TimeSpan.FromMilliseconds(400));
                    FullscreenHud.BeginAnimation(UIElement.OpacityProperty, anim);
                }
            };
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            IntPtr handle = new WindowInteropHelper(this).Handle;
            _hwndSource = HwndSource.FromHwnd(handle);
            _hwndSource?.AddHook(HwndHook);

            // Register global hotkey Ctrl + Alt + R
            Win32.RegisterHotKey(handle, HOTKEY_ID, (uint)(Win32.MOD_CONTROL | Win32.MOD_ALT), 0x52);

            // Install low-level keyboard hook for 100% reliable hotkeys (Esc, F11, Ctrl+Alt+R, Ctrl+Alt+G, etc.)
            try
            {
                _keyboardProc = KeyboardHookCallback;
                using var curProcess = Process.GetCurrentProcess();
                using var curModule = curProcess.MainModule;
                _keyboardHookId = Win32.SetWindowsHookEx(Win32.WH_KEYBOARD_LL, _keyboardProc, Win32.GetModuleHandle(curModule?.ModuleName), 0);
            }
            catch { }

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
                    int cxBig = Win32.GetSystemMetrics(Win32.SM_CXICON);
                    if (cxBig <= 0) cxBig = 32;
                    int cyBig = Win32.GetSystemMetrics(Win32.SM_CYICON);
                    if (cyBig <= 0) cyBig = 32;

                    int cxSm = Win32.GetSystemMetrics(Win32.SM_CXSMICON);
                    if (cxSm <= 0) cxSm = 16;
                    int cySm = Win32.GetSystemMetrics(Win32.SM_CYSMICON);
                    if (cySm <= 0) cySm = 16;

                    IntPtr hIconBig = Win32.LoadImage(IntPtr.Zero, icoPath, 1, cxBig, cyBig, 0x0010);
                    IntPtr hIconSm = Win32.LoadImage(IntPtr.Zero, icoPath, 1, cxSm, cySm, 0x0010);
                    if (hIconBig != IntPtr.Zero) Win32.SendMessage(handle, Win32.WM_SETICON, (IntPtr)1, hIconBig);
                    if (hIconSm != IntPtr.Zero) Win32.SendMessage(handle, Win32.WM_SETICON, (IntPtr)0, hIconSm);

                    this.Icon = BitmapFrame.Create(new Uri(icoPath, UriKind.Absolute));
                }
            }
            catch { }

            LoadConfig();
            try
            {
                ChkAutoStart.IsChecked = IsAutoStartEnabled();
                ChkRemember.IsChecked = _rememberState;
            }
            catch { }

            ApplyTheme(_currentTheme);
            SetFps(_targetFps);
            SetQuality(_scalingMode);
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
            SaveConfig();

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

            if (_keyboardHookId != IntPtr.Zero)
            {
                Win32.UnhookWindowsHookEx(_keyboardHookId);
                _keyboardHookId = IntPtr.Zero;
            }

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
                if (_rememberState && !string.IsNullOrWhiteSpace(_lastRememberedTarget))
                {
                    var rem = items.FirstOrDefault(w =>
                        w.Title.Contains(_lastRememberedTarget, StringComparison.OrdinalIgnoreCase) ||
                        w.ProcessName.Contains(_lastRememberedTarget, StringComparison.OrdinalIgnoreCase));
                    if (rem != null)
                    {
                        SetTarget(rem.Handle);
                        return;
                    }
                }

                var waCall = items.FirstOrDefault(w =>
                    (w.Title.Contains("WhatsApp", StringComparison.OrdinalIgnoreCase) || w.ProcessName.Contains("WhatsApp", StringComparison.OrdinalIgnoreCase)) &&
                    (w.Title.Contains("Call", StringComparison.OrdinalIgnoreCase) || w.Title.Contains("Llamada", StringComparison.OrdinalIgnoreCase) || w.Title.Contains("Video", StringComparison.OrdinalIgnoreCase)));

                if (waCall != null)
                {
                    SetTarget(waCall.Handle);
                    return;
                }

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
            SaveConfig();
        }

        private void CaptureTimer_Tick(object? sender, EventArgs e)
        {
            if (_targetHwnd == IntPtr.Zero)
            {
                PlaceholderOverlay.Visibility = Visibility.Visible;
                return;
            }

            if (_isAlwaysOnTop && !_isSimulatingInput && !_isForwardingWheel)
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
                BtnPin.BorderBrush = new SolidColorBrush(Color.FromRgb(245, 158, 11));
                BtnPin.Foreground = new SolidColorBrush(Color.FromRgb(251, 191, 36));
                TxtPin.Foreground = new SolidColorBrush(Color.FromRgb(251, 191, 36));
                TxtPin.Text = "Fijo";
                if (BdFsPin != null)
                {
                    BdFsPin.Background = new SolidColorBrush(Color.FromRgb(45, 32, 6));
                    BdFsPin.BorderBrush = new SolidColorBrush(Color.FromRgb(245, 158, 11));
                }
                if (IcoFsPin != null)
                {
                    IcoFsPin.Fill = new SolidColorBrush(Color.FromRgb(251, 191, 36));
                }
            }
            else
            {
                BtnPin.Background = new SolidColorBrush(Color.FromRgb(30, 30, 38));
                BtnPin.BorderBrush = new SolidColorBrush(Color.FromRgb(51, 51, 66));
                BtnPin.Foreground = new SolidColorBrush(Color.FromRgb(148, 163, 184));
                TxtPin.Foreground = new SolidColorBrush(Color.FromRgb(148, 163, 184));
                TxtPin.Text = "Fijo";
                if (BdFsPin != null)
                {
                    BdFsPin.Background = new SolidColorBrush(Color.FromRgb(30, 30, 38));
                    BdFsPin.BorderBrush = new SolidColorBrush(Color.FromRgb(51, 51, 66));
                }
                if (IcoFsPin != null)
                {
                    IcoFsPin.Fill = new SolidColorBrush(Color.FromRgb(148, 163, 184));
                }
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
                BtnInteract.BorderBrush = new SolidColorBrush(Color.FromRgb(16, 185, 129));
                BtnInteract.Foreground = new SolidColorBrush(Color.FromRgb(52, 211, 153));
                TxtInteract.Foreground = new SolidColorBrush(Color.FromRgb(52, 211, 153));
                TxtInteract.Text = "Clics";
                if (BdFsInteract != null)
                {
                    BdFsInteract.Background = new SolidColorBrush(Color.FromRgb(6, 40, 30));
                    BdFsInteract.BorderBrush = new SolidColorBrush(Color.FromRgb(16, 185, 129));
                }
                if (IcoFsInteract != null)
                {
                    IcoFsInteract.Fill = new SolidColorBrush(Color.FromRgb(52, 211, 153));
                }
            }
            else
            {
                BtnInteract.Background = new SolidColorBrush(Color.FromRgb(30, 30, 38));
                BtnInteract.BorderBrush = new SolidColorBrush(Color.FromRgb(51, 51, 66));
                BtnInteract.Foreground = new SolidColorBrush(Color.FromRgb(148, 163, 184));
                TxtInteract.Foreground = new SolidColorBrush(Color.FromRgb(148, 163, 184));
                TxtInteract.Text = "Clics";
                if (BdFsInteract != null)
                {
                    BdFsInteract.Background = new SolidColorBrush(Color.FromRgb(30, 30, 38));
                    BdFsInteract.BorderBrush = new SolidColorBrush(Color.FromRgb(51, 51, 66));
                }
                if (IcoFsInteract != null)
                {
                    IcoFsInteract.Fill = new SolidColorBrush(Color.FromRgb(148, 163, 184));
                }
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
                BtnGhost.Foreground = new SolidColorBrush(Color.FromRgb(216, 180, 254));
                TxtGhost.Foreground = new SolidColorBrush(Color.FromRgb(216, 180, 254));
                TxtGhost.Text = "Ocultar";
                if (BdFsGhost != null)
                {
                    BdFsGhost.Background = new SolidColorBrush(Color.FromRgb(58, 20, 80));
                    BdFsGhost.BorderBrush = new SolidColorBrush(Color.FromRgb(168, 85, 247));
                }
                if (IcoFsGhost != null)
                {
                    IcoFsGhost.Fill = new SolidColorBrush(Color.FromRgb(216, 180, 254));
                }
            }
            else
            {
                BtnGhost.Background = new SolidColorBrush(Color.FromRgb(30, 30, 38));
                BtnGhost.BorderBrush = new SolidColorBrush(Color.FromRgb(51, 51, 66));
                BtnGhost.Foreground = new SolidColorBrush(Color.FromRgb(148, 163, 184));
                TxtGhost.Foreground = new SolidColorBrush(Color.FromRgb(148, 163, 184));
                TxtGhost.Text = "Ocultar";
                if (BdFsGhost != null)
                {
                    BdFsGhost.Background = new SolidColorBrush(Color.FromRgb(30, 30, 38));
                    BdFsGhost.BorderBrush = new SolidColorBrush(Color.FromRgb(51, 51, 66));
                }
                if (IcoFsGhost != null)
                {
                    IcoFsGhost.Fill = new SolidColorBrush(Color.FromRgb(148, 163, 184));
                }
            }
        }

        #region Info & Settings Modal
        private void BtnInfo_Click(object sender, RoutedEventArgs e)
        {
            ChkAutoStart.IsChecked = IsAutoStartEnabled();
            ChkRemember.IsChecked = _rememberState;
            UpdateThemeButtonStyles();
            UpdateFpsButtonStyles();
            UpdateQualityButtonStyles();
            SetActiveTab(0);
            InfoOverlay.Visibility = Visibility.Visible;
        }

        private void BtnCloseInfo_Click(object sender, RoutedEventArgs e)
        {
            InfoOverlay.Visibility = Visibility.Collapsed;
        }

        private void ApplyTheme(string themeId)
        {
            if (!AvailableThemes.TryGetValue(themeId, out var theme))
            {
                theme = AvailableThemes["Indigo"];
                themeId = "Indigo";
            }
            _currentTheme = themeId;

            // 1. Window & Main Surfaces
            this.Background = new SolidColorBrush(theme.WindowBg);
            if (HeaderBorder != null)
            {
                HeaderBorder.Background = new SolidColorBrush(theme.HeaderBg);
                HeaderBorder.BorderBrush = new SolidColorBrush(theme.HeaderBorder);
            }
            if (FooterBorder != null)
            {
                FooterBorder.Background = new SolidColorBrush(theme.FooterBg);
                FooterBorder.BorderBrush = new SolidColorBrush(theme.HeaderBorder);
            }

            // 2. Footer Angle Badge & Hotkey Text
            if (BdAngleBadge != null)
            {
                BdAngleBadge.Background = new SolidColorBrush(theme.BadgeBg);
                BdAngleBadge.BorderBrush = new SolidColorBrush(theme.BadgeBorder);
            }
            if (TxtAngleBadge != null)
            {
                TxtAngleBadge.Foreground = new SolidColorBrush(theme.AccentLight);
            }
            if (TxtHotkeyHint != null)
            {
                TxtHotkeyHint.Foreground = new SolidColorBrush(theme.AccentBorder);
            }

            // 3. Fullscreen HUD Elements
            if (BdFsRotate != null)
            {
                BdFsRotate.Background = new SolidColorBrush(theme.BadgeBg);
                BdFsRotate.BorderBrush = new SolidColorBrush(theme.AccentBorder);
            }
            if (TxtFsAngle != null)
            {
                TxtFsAngle.Foreground = new SolidColorBrush(theme.AccentLight);
            }
            if (FullscreenHud != null)
            {
                FullscreenHud.BorderBrush = new SolidColorBrush(theme.HudBorder);
            }

            // 4. Modal Surfaces
            if (InfoModalBorder != null)
            {
                InfoModalBorder.Background = new SolidColorBrush(theme.WindowBg);
                InfoModalBorder.BorderBrush = new SolidColorBrush(theme.HeaderBorder);
            }
            if (InfoHeaderBorder != null)
            {
                InfoHeaderBorder.Background = new SolidColorBrush(theme.HeaderBg);
                InfoHeaderBorder.BorderBrush = new SolidColorBrush(theme.HeaderBorder);
            }
            if (InfoFooterBorder != null)
            {
                InfoFooterBorder.Background = new SolidColorBrush(theme.FooterBg);
                InfoFooterBorder.BorderBrush = new SolidColorBrush(theme.HeaderBorder);
            }

            // 5. Update Active Controls
            UpdateRotationButtonStyles();
            UpdateFpsButtonStyles();
            UpdateQualityButtonStyles();
            UpdateThemeButtonStyles();
            SaveConfig();
        }

        private void UpdateThemeButtonStyles()
        {
            var buttons = new (string id, Button? btn)[]
            {
                ("Indigo", BtnThemeIndigo),
                ("Emerald", BtnThemeEmerald),
                ("Amethyst", BtnThemeAmethyst),
                ("Crimson", BtnThemeCrimson),
                ("Amber", BtnThemeAmber),
                ("Glacier", BtnThemeGlacier)
            };

            var cur = AvailableThemes.TryGetValue(_currentTheme, out var t) ? t : AvailableThemes["Indigo"];

            foreach (var (id, btn) in buttons)
            {
                if (btn == null) continue;
                bool isSelected = string.Equals(_currentTheme, id, StringComparison.OrdinalIgnoreCase);
                if (isSelected)
                {
                    btn.Background = new SolidColorBrush(cur.BadgeBg);
                    btn.BorderBrush = new SolidColorBrush(cur.AccentBorder);
                    btn.Foreground = Brushes.White;
                }
                else
                {
                    btn.Background = new SolidColorBrush(Color.FromRgb(26, 26, 36));
                    btn.BorderBrush = new SolidColorBrush(Color.FromRgb(45, 45, 60));
                    btn.Foreground = new SolidColorBrush(Color.FromRgb(148, 163, 184));
                }
            }
        }

        private void BtnTheme_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is string themeId)
            {
                ApplyTheme(themeId);
                TxtStatus.Text = $"Tema decorativo aplicado: {AvailableThemes.GetValueOrDefault(themeId)?.Name ?? themeId}";
            }
        }

        private void SetActiveTab(int tabIndex)
        {
            if (TabSettingsView != null) TabSettingsView.Visibility = tabIndex == 0 ? Visibility.Visible : Visibility.Collapsed;
            if (TabShortcutsView != null) TabShortcutsView.Visibility = tabIndex == 1 ? Visibility.Visible : Visibility.Collapsed;
            if (TabAboutView != null) TabAboutView.Visibility = tabIndex == 2 ? Visibility.Visible : Visibility.Collapsed;

            var cur = AvailableThemes.TryGetValue(_currentTheme, out var t) ? t : AvailableThemes["Indigo"];

            void StyleTab(Button? btn, bool active)
            {
                if (btn == null) return;
                if (active)
                {
                    btn.Background = new SolidColorBrush(cur.BadgeBg);
                    btn.BorderBrush = new SolidColorBrush(cur.AccentBorder);
                    btn.Foreground = Brushes.White;
                }
                else
                {
                    btn.Background = Brushes.Transparent;
                    btn.BorderBrush = Brushes.Transparent;
                    btn.Foreground = new SolidColorBrush(Color.FromRgb(148, 163, 184));
                }
            }

            StyleTab(BtnTabSettings, tabIndex == 0);
            StyleTab(BtnTabShortcuts, tabIndex == 1);
            StyleTab(BtnTabAbout, tabIndex == 2);
        }

        private void BtnTabSettings_Click(object sender, RoutedEventArgs e) => SetActiveTab(0);
        private void BtnTabShortcuts_Click(object sender, RoutedEventArgs e) => SetActiveTab(1);
        private void BtnTabAbout_Click(object sender, RoutedEventArgs e) => SetActiveTab(2);

        private void SetFps(int fps)
        {
            _targetFps = fps switch
            {
                <= 30 => 30,
                <= 60 => 60,
                _ => 120
            };

            int intervalMs = _targetFps switch
            {
                30 => 33,
                60 => 16,
                120 => 8,
                _ => 16
            };

            _captureTimer.Interval = TimeSpan.FromMilliseconds(intervalMs);
            UpdateFpsButtonStyles();
            SaveConfig();
        }

        private void UpdateFpsButtonStyles()
        {
            var cur = AvailableThemes.TryGetValue(_currentTheme, out var t) ? t : AvailableThemes["Indigo"];

            void StyleBtn(Button? btn, bool isActive)
            {
                if (btn == null) return;
                if (isActive)
                {
                    btn.Background = new SolidColorBrush(cur.AccentDark);
                    btn.BorderBrush = new SolidColorBrush(cur.AccentBorder);
                    btn.Foreground = Brushes.White;
                }
                else
                {
                    btn.Background = new SolidColorBrush(Color.FromRgb(26, 26, 36));
                    btn.BorderBrush = new SolidColorBrush(Color.FromRgb(45, 45, 60));
                    btn.Foreground = new SolidColorBrush(Color.FromRgb(148, 163, 184));
                }
            }

            StyleBtn(BtnFps30, _targetFps == 30);
            StyleBtn(BtnFps60, _targetFps == 60);
            StyleBtn(BtnFps120, _targetFps == 120);
        }

        private void BtnFps30_Click(object sender, RoutedEventArgs e) => SetFps(30);
        private void BtnFps60_Click(object sender, RoutedEventArgs e) => SetFps(60);
        private void BtnFps120_Click(object sender, RoutedEventArgs e) => SetFps(120);

        private void SetQuality(string mode)
        {
            _scalingMode = mode;
            BitmapScalingMode scaling = mode switch
            {
                "Linear" => BitmapScalingMode.Linear,
                "NearestNeighbor" => BitmapScalingMode.NearestNeighbor,
                _ => BitmapScalingMode.HighQuality
            };

            RenderOptions.SetBitmapScalingMode(RotatedImageHost, scaling);
            UpdateQualityButtonStyles();
            SaveConfig();
        }

        private void UpdateQualityButtonStyles()
        {
            var cur = AvailableThemes.TryGetValue(_currentTheme, out var t) ? t : AvailableThemes["Indigo"];

            void StyleBtn(Button? btn, bool isActive)
            {
                if (btn == null) return;
                if (isActive)
                {
                    btn.Background = new SolidColorBrush(cur.AccentDark);
                    btn.BorderBrush = new SolidColorBrush(cur.AccentBorder);
                    btn.Foreground = Brushes.White;
                }
                else
                {
                    btn.Background = new SolidColorBrush(Color.FromRgb(26, 26, 36));
                    btn.BorderBrush = new SolidColorBrush(Color.FromRgb(45, 45, 60));
                    btn.Foreground = new SolidColorBrush(Color.FromRgb(148, 163, 184));
                }
            }

            void StyleRow(Border? bd, bool isSelected)
            {
                if (bd == null) return;
                if (isSelected)
                {
                    bd.Background = new SolidColorBrush(Color.FromArgb(45, cur.AccentDark.R, cur.AccentDark.G, cur.AccentDark.B));
                    bd.BorderBrush = new SolidColorBrush(cur.AccentBorder);
                    bd.BorderThickness = new Thickness(1);
                    bd.Opacity = 1.0;
                }
                else
                {
                    bd.Background = Brushes.Transparent;
                    bd.BorderBrush = Brushes.Transparent;
                    bd.BorderThickness = new Thickness(0);
                    bd.Opacity = 0.55;
                }
            }

            StyleBtn(BtnQualityHigh, _scalingMode == "HighQuality");
            StyleBtn(BtnQualityLinear, _scalingMode == "Linear");
            StyleBtn(BtnQualityNearest, _scalingMode == "NearestNeighbor");

            StyleRow(BdQualHigh, _scalingMode == "HighQuality");
            StyleRow(BdQualLinear, _scalingMode == "Linear");
            StyleRow(BdQualNearest, _scalingMode == "NearestNeighbor");
        }

        private void BtnQualityHigh_Click(object sender, RoutedEventArgs e) => SetQuality("HighQuality");
        private void BtnQualityLinear_Click(object sender, RoutedEventArgs e) => SetQuality("Linear");
        private void BtnQualityNearest_Click(object sender, RoutedEventArgs e) => SetQuality("NearestNeighbor");

        private void BdQualHigh_Click(object sender, MouseButtonEventArgs e) => SetQuality("HighQuality");
        private void BdQualLinear_Click(object sender, MouseButtonEventArgs e) => SetQuality("Linear");
        private void BdQualNearest_Click(object sender, MouseButtonEventArgs e) => SetQuality("NearestNeighbor");

        private void BdQual_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is Border bd && bd.Opacity < 1.0)
            {
                bd.Opacity = 0.85;
            }
        }

        private void BdQual_MouseLeave(object sender, MouseEventArgs e)
        {
            if (sender is Border bd && bd.Opacity < 1.0)
            {
                bd.Opacity = 0.55;
            }
        }

        private void OpenUrl(string url)
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = url,
                    UseShellExecute = true
                });
            }
            catch { }
        }

        private void LnkRocaman_Click(object sender, RoutedEventArgs e) => OpenUrl("https://rocaman.vercel.app/");

        private void ChkAutoStart_Click(object sender, RoutedEventArgs e)
        {
            SetAutoStart(ChkAutoStart.IsChecked == true);
        }

        private void ChkRemember_Click(object sender, RoutedEventArgs e)
        {
            _rememberState = ChkRemember.IsChecked == true;
            SaveConfig();
        }

        private bool IsAutoStartEnabled()
        {
            try
            {
                using var key = Registry.CurrentUser.OpenSubKey(RUN_REG_KEY, false);
                return key?.GetValue(APP_NAME) != null;
            }
            catch { return false; }
        }

        private void SetAutoStart(bool enable)
        {
            try
            {
                using var key = Registry.CurrentUser.OpenSubKey(RUN_REG_KEY, true);
                if (key == null) return;
                if (enable)
                {
                    string exePath = Process.GetCurrentProcess().MainModule?.FileName ?? System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AlwaysRotate.exe");
                    key.SetValue(APP_NAME, $"\"{exePath}\"");
                }
                else
                {
                    key.DeleteValue(APP_NAME, false);
                }
            }
            catch { }
        }

        private void LoadConfig()
        {
            try
            {
                if (System.IO.File.Exists(ConfigPath))
                {
                    string json = System.IO.File.ReadAllText(ConfigPath);
                    using var doc = JsonDocument.Parse(json);
                    if (doc.RootElement.TryGetProperty("rememberState", out var rElem))
                    {
                        _rememberState = rElem.GetBoolean();
                    }
                    if (doc.RootElement.TryGetProperty("targetFps", out var fpsElem))
                    {
                        _targetFps = fpsElem.GetInt32();
                    }
                    if (doc.RootElement.TryGetProperty("scalingMode", out var scElem))
                    {
                        _scalingMode = scElem.GetString() ?? "HighQuality";
                    }
                    if (doc.RootElement.TryGetProperty("theme", out var thElem))
                    {
                        string? savedTheme = thElem.GetString();
                        if (!string.IsNullOrWhiteSpace(savedTheme) && AvailableThemes.ContainsKey(savedTheme))
                        {
                            _currentTheme = savedTheme;
                        }
                    }
                    if (_rememberState)
                    {
                        if (doc.RootElement.TryGetProperty("lastAngle", out var aElem))
                        {
                            _currentAngle = aElem.GetInt32();
                        }
                        if (doc.RootElement.TryGetProperty("lastTarget", out var tElem))
                        {
                            _lastRememberedTarget = tElem.GetString() ?? string.Empty;
                        }
                        if (doc.RootElement.TryGetProperty("windowWidth", out var wElem) && wElem.GetDouble() >= 460)
                        {
                            this.Width = wElem.GetDouble();
                        }
                        if (doc.RootElement.TryGetProperty("windowHeight", out var hElem) && hElem.GetDouble() >= 420)
                        {
                            this.Height = hElem.GetDouble();
                        }
                    }
                }
            }
            catch { }
        }

        private void SaveConfig()
        {
            try
            {
                double saveW = (this.WindowState == WindowState.Normal && this.ActualWidth >= 460) ? this.ActualWidth : 880;
                double saveH = (this.WindowState == WindowState.Normal && this.ActualHeight >= 420) ? this.ActualHeight : 640;

                var obj = new
                {
                    rememberState = _rememberState,
                    lastAngle = _rememberState ? _currentAngle : 90,
                    lastTarget = _rememberState ? _targetTitle : string.Empty,
                    targetFps = _targetFps,
                    scalingMode = _scalingMode,
                    theme = _currentTheme,
                    windowWidth = saveW,
                    windowHeight = saveH
                };
                string json = JsonSerializer.Serialize(obj, new JsonSerializerOptions { WriteIndented = true });
                System.IO.File.WriteAllText(ConfigPath, json);
            }
            catch { }
        }
        #endregion

        #region Fullscreen & Window Zen Modes
        private void BtnFullscreen_Click(object sender, RoutedEventArgs e)
        {
            SetViewState(_currentView == ViewState.Fullscreen ? ViewState.Normal : ViewState.Fullscreen);
        }

        private void BtnWindowZen_Click(object sender, RoutedEventArgs e)
        {
            SetViewState(_currentView == ViewState.WindowZen ? ViewState.Normal : ViewState.WindowZen);
        }

        private void BtnExitFullscreen_Click(object sender, RoutedEventArgs e)
        {
            SetViewState(ViewState.Normal);
        }

        private void BtnFsToggleMode_Click(object sender, RoutedEventArgs e)
        {
            if (_currentView == ViewState.Fullscreen)
            {
                SetViewState(ViewState.WindowZen);
            }
            else
            {
                SetViewState(ViewState.Fullscreen);
            }
        }

        private void BtnFsRotate_Click(object sender, RoutedEventArgs e)
        {
            ApplyAngle(_currentAngle + 90);
        }

        private void ToggleFullscreen(bool enable)
        {
            SetViewState(enable ? ViewState.Fullscreen : ViewState.Normal);
        }

        private void SetViewState(ViewState target)
        {
            if (_currentView == target) return;

            if (_currentView == ViewState.Normal)
            {
                _prevWindowState = WindowState;
                _prevWindowStyle = WindowStyle;
                _prevResizeMode = ResizeMode;
            }

            _currentView = target;

            switch (target)
            {
                case ViewState.Normal:
                    _hudFadeTimer.Stop();
                    FullscreenHud.BeginAnimation(UIElement.OpacityProperty, null);
                    FullscreenHud.Opacity = 1.0;
                    FullscreenHud.Visibility = Visibility.Collapsed;

                    // Restore menus
                    HeaderBorder.Visibility = Visibility.Visible;
                    FooterBorder.Visibility = Visibility.Visible;

                    // Ensure standard window frame
                    if (WindowState == WindowState.Maximized)
                    {
                        WindowState = WindowState.Normal;
                    }
                    WindowStyle = WindowStyle.SingleBorderWindow;
                    ResizeMode = ResizeMode.CanResize;
                    break;

                case ViewState.WindowZen:
                    // Hide menus (Header and Footer)
                    HeaderBorder.Visibility = Visibility.Collapsed;
                    FooterBorder.Visibility = Visibility.Collapsed;

                    // Keep normal window frame so titlebar and borders are 100% native Windows
                    if (WindowState == WindowState.Maximized)
                    {
                        WindowState = WindowState.Normal;
                    }
                    WindowStyle = WindowStyle.SingleBorderWindow;
                    ResizeMode = ResizeMode.CanResize;

                    // Configure HUD toggle button
                    if (IcoFsToggleMode != null)
                    {
                        IcoFsToggleMode.Data = Geometry.Parse("M7 14H5v5h5v-2H7v-3zm-2-4h2V7h3V5H5v5zm12 7h-3v2h5v-5h-2v3zM14 5v2h3v3h2V5h-5z");
                    }
                    BtnFsToggleMode.ToolTip = "Pantalla Completa total (F11)";

                    // Show HUD with auto-hide timer
                    FullscreenHud.BeginAnimation(UIElement.OpacityProperty, null);
                    FullscreenHud.Opacity = 1.0;
                    FullscreenHud.Visibility = Visibility.Visible;
                    _hudFadeTimer.Stop();
                    _hudFadeTimer.Start();
                    break;

                case ViewState.Fullscreen:
                    HeaderBorder.Visibility = Visibility.Collapsed;
                    FooterBorder.Visibility = Visibility.Collapsed;

                    // Borderless Fullscreen
                    WindowStyle = WindowStyle.None;
                    ResizeMode = ResizeMode.NoResize;
                    WindowState = WindowState.Maximized;

                    // Configure HUD toggle button
                    if (IcoFsToggleMode != null)
                    {
                        IcoFsToggleMode.Data = Geometry.Parse("M19 4H5a2 2 0 0 0-2 2v12a2 2 0 0 0 2 2h14a2 2 0 0 0 2-2V6a2 2 0 0 0-2-2zm0 14H5V8h14v10z");
                    }
                    BtnFsToggleMode.ToolTip = "Modo Solo Ventana (F10)";

                    // Show HUD with auto-hide timer
                    FullscreenHud.BeginAnimation(UIElement.OpacityProperty, null);
                    FullscreenHud.Opacity = 1.0;
                    FullscreenHud.Visibility = Visibility.Visible;
                    _hudFadeTimer.Stop();
                    _hudFadeTimer.Start();
                    break;
            }

            EnforceAlwaysOnTop();
        }

        private void Window_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (_currentView == ViewState.Normal) return;

            if (FullscreenHud.Opacity < 1.0)
            {
                var anim = new System.Windows.Media.Animation.DoubleAnimation(1.0, TimeSpan.FromMilliseconds(150));
                FullscreenHud.BeginAnimation(UIElement.OpacityProperty, anim);
            }

            _hudFadeTimer.Stop();
            if (!FullscreenHud.IsMouseOver)
            {
                _hudFadeTimer.Start();
            }
        }

        private void FullscreenHud_MouseEnter(object sender, MouseEventArgs e)
        {
            if (_currentView == ViewState.Normal) return;
            _hudFadeTimer.Stop();
            var anim = new System.Windows.Media.Animation.DoubleAnimation(1.0, TimeSpan.FromMilliseconds(120));
            FullscreenHud.BeginAnimation(UIElement.OpacityProperty, anim);
        }

        private void FullscreenHud_MouseLeave(object sender, MouseEventArgs e)
        {
            if (_currentView == ViewState.Normal) return;
            _hudFadeTimer.Stop();
            _hudFadeTimer.Start();
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                if (InfoOverlay.Visibility == Visibility.Visible)
                {
                    InfoOverlay.Visibility = Visibility.Collapsed;
                    e.Handled = true;
                    return;
                }
                if (_currentView != ViewState.Normal)
                {
                    SetViewState(ViewState.Normal);
                    e.Handled = true;
                }
            }
            else if (e.Key == Key.F10)
            {
                SetViewState(_currentView == ViewState.WindowZen ? ViewState.Normal : ViewState.WindowZen);
                e.Handled = true;
            }
            else if (e.Key == Key.F11)
            {
                SetViewState(_currentView == ViewState.Fullscreen ? ViewState.Normal : ViewState.Fullscreen);
                e.Handled = true;
            }
        }

        private IntPtr KeyboardHookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && (wParam == (IntPtr)Win32.WM_KEYDOWN_MSG || wParam == (IntPtr)Win32.WM_SYSKEYDOWN_MSG))
            {
                var hookStruct = Marshal.PtrToStructure<Win32.KBDLLHOOKSTRUCT>(lParam);
                uint vk = hookStruct.vkCode;

                bool isCtrl = (Win32.GetAsyncKeyState(0x11) & 0x8000) != 0;
                bool isAlt = (Win32.GetAsyncKeyState(0x12) & 0x8000) != 0;

                // Escape: Close Info overlay or restore normal menus
                if (vk == 0x1B) // VK_ESCAPE
                {
                    if (InfoOverlay.Visibility == Visibility.Visible)
                    {
                        Dispatcher.BeginInvoke(() => InfoOverlay.Visibility = Visibility.Collapsed);
                        return (IntPtr)1; // swallow
                    }
                    if (_currentView != ViewState.Normal)
                    {
                        Dispatcher.BeginInvoke(() => SetViewState(ViewState.Normal));
                        return (IntPtr)1; // swallow
                    }
                }

                // F10: Toggle Window Zen (Solo Ventana)
                if (vk == 0x79) // VK_F10
                {
                    IntPtr fg = Win32.GetForegroundWindow();
                    IntPtr myHwnd = new WindowInteropHelper(this).Handle;
                    if (fg == myHwnd || (_targetHwnd != IntPtr.Zero && fg == _targetHwnd) || _currentView != ViewState.Normal)
                    {
                        Dispatcher.BeginInvoke(() => SetViewState(_currentView == ViewState.WindowZen ? ViewState.Normal : ViewState.WindowZen));
                        return (IntPtr)1; // swallow
                    }
                }

                // F11: Toggle Fullscreen (Monitor Completo)
                if (vk == 0x7A) // VK_F11
                {
                    IntPtr fg = Win32.GetForegroundWindow();
                    IntPtr myHwnd = new WindowInteropHelper(this).Handle;
                    if (fg == myHwnd || (_targetHwnd != IntPtr.Zero && fg == _targetHwnd) || _currentView != ViewState.Normal)
                    {
                        Dispatcher.BeginInvoke(() => SetViewState(_currentView == ViewState.Fullscreen ? ViewState.Normal : ViewState.Fullscreen));
                        return (IntPtr)1; // swallow
                    }
                }

                // Global hotkeys with Ctrl + Alt:
                if (isCtrl && isAlt)
                {
                    // Ctrl + Alt + R -> Capture active window
                    if (vk == 0x52) // 'R'
                    {
                        IntPtr fg = Win32.GetForegroundWindow();
                        IntPtr myHwnd = new WindowInteropHelper(this).Handle;
                        if (fg != IntPtr.Zero && fg != myHwnd)
                        {
                            Dispatcher.BeginInvoke(() => SetTarget(fg));
                            return (IntPtr)1;
                        }
                    }
                    // Ctrl + Alt + G -> Toggle Ghost mode (Hide original)
                    else if (vk == 0x47) // 'G'
                    {
                        Dispatcher.BeginInvoke(() => BtnGhost_Click(this, new RoutedEventArgs()));
                        return (IntPtr)1;
                    }
                    // Ctrl + Alt + Right Arrow OR Ctrl + Alt + Space -> Rotate +90
                    else if (vk == 0x27 || vk == 0x20) // VK_RIGHT or VK_SPACE
                    {
                        Dispatcher.BeginInvoke(() => ApplyAngle(_currentAngle + 90));
                        return (IntPtr)1;
                    }
                    // Ctrl + Alt + Left Arrow -> Rotate -90
                    else if (vk == 0x25) // VK_LEFT
                    {
                        Dispatcher.BeginInvoke(() => ApplyAngle(_currentAngle - 90));
                        return (IntPtr)1;
                    }
                    // Ctrl + Alt + P -> Toggle Pin (Always on top)
                    else if (vk == 0x50) // 'P'
                    {
                        Dispatcher.BeginInvoke(() => BtnPin_Click(this, new RoutedEventArgs()));
                        return (IntPtr)1;
                    }
                    // Ctrl + Alt + I -> Toggle Interact (Click forwarding)
                    else if (vk == 0x49) // 'I'
                    {
                        Dispatcher.BeginInvoke(() => BtnInteract_Click(this, new RoutedEventArgs()));
                        return (IntPtr)1;
                    }
                }
            }

            return Win32.CallNextHookEx(_keyboardHookId, nCode, wParam, lParam);
        }
        #endregion

        private void ApplyAngle(int angle)
        {
            _currentAngle = ((angle % 360) + 360) % 360;
            TxtAngleBadge.Text = $"{_currentAngle}°";
            if (TxtFsAngle != null) TxtFsAngle.Text = $"{_currentAngle}°";
            UpdateRotationButtonStyles();
            TxtStatus.Text = $"Rotación fijada en: {_currentAngle}°";
            SaveConfig();
        }

        private void UpdateRotationButtonStyles()
        {
            var cur = AvailableThemes.TryGetValue(_currentTheme, out var t) ? t : AvailableThemes["Indigo"];

            void StyleBtn(Button btn, bool isActive)
            {
                if (isActive)
                {
                    btn.Background = new SolidColorBrush(cur.AccentDark);
                    btn.BorderBrush = new SolidColorBrush(cur.AccentBorder);
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
        private readonly object _wheelLock = new object();
        private volatile bool _isForwardingWheel = false;

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
            int origX = myRect.Left;
            int origY = myRect.Top;
            bool isCoveredByMe = (screenX >= myRect.Left && screenX <= myRect.Right && screenY >= myRect.Top && screenY <= myRect.Bottom);

            ThreadPool.QueueUserWorkItem(_ =>
            {
                lock (_clickLock)
                {
                    _isSimulatingInput = true;
                    int targetOriginalExStyle = 0;
                    try
                    {
                        if (_isGhostEnabled)
                        {
                            // Temporarily allow target to receive mouse click (target is otherwise click-through so user can use the screen)
                            targetOriginalExStyle = Win32.GetWindowLong(_targetHwnd, Win32.GWL_EXSTYLE);
                            Win32.SetWindowLong(_targetHwnd, Win32.GWL_EXSTYLE, targetOriginalExStyle & ~Win32.WS_EX_TRANSPARENT);
                            Win32.SetWindowPos(_targetHwnd, IntPtr.Zero, 0, 0, 0, 0, Win32.SWP_NOMOVE | Win32.SWP_NOSIZE | Win32.SWP_NOZORDER | Win32.SWP_FRAMECHANGED | Win32.SWP_NOACTIVATE);
                        }

                        Win32.GetCursorPos(out var oldCursor);

                        // 1. Activate target window in foreground (WhatsApp)
                        Win32.SetForegroundWindow(_targetHwnd);
                        Thread.Sleep(20);

                        // 2. Position cursor exactly over the control in target window
                        Win32.SetCursorPos(screenX, screenY);

                        // 3. If AlwaysRotate is physically covering the click point, shift it for 0ms (instantaneous sub-frame, zero flicker)
                        if (isCoveredByMe)
                        {
                            Win32.SetWindowPos(myHwnd, IntPtr.Zero, -15000, -15000, 0, 0, Win32.SWP_NOSIZE | Win32.SWP_NOZORDER | Win32.SWP_NOACTIVATE);
                        }

                        uint downFlag = isRightClick ? Win32.MOUSEEVENTF_RIGHTDOWN : Win32.MOUSEEVENTF_LEFTDOWN;
                        uint upFlag = isRightClick ? Win32.MOUSEEVENTF_RIGHTUP : Win32.MOUSEEVENTF_LEFTUP;

                        // 4. Dispatch real hardware mouse down + up directly to target
                        Win32.mouse_event(downFlag, 0, 0, 0, UIntPtr.Zero);
                        Win32.mouse_event(upFlag, 0, 0, 0, UIntPtr.Zero);

                        // 5. Restore AlwaysRotate immediately within the same video frame (zero visual disappearance)
                        if (isCoveredByMe)
                        {
                            Win32.SetWindowPos(myHwnd, _isAlwaysOnTop ? Win32.HWND_TOPMOST : Win32.HWND_NOTOPMOST, origX, origY, 0, 0, Win32.SWP_NOSIZE | Win32.SWP_NOACTIVATE);
                        }

                        // 6. Return cursor to original position
                        Win32.SetCursorPos(oldCursor.X, oldCursor.Y);
                    }
                    finally
                    {
                        if (_isGhostEnabled)
                        {
                            Win32.SetWindowLong(_targetHwnd, Win32.GWL_EXSTYLE, targetOriginalExStyle | Win32.WS_EX_TRANSPARENT);
                            Win32.SetWindowPos(_targetHwnd, IntPtr.Zero, 0, 0, 0, 0, Win32.SWP_NOMOVE | Win32.SWP_NOSIZE | Win32.SWP_NOZORDER | Win32.SWP_FRAMECHANGED | Win32.SWP_NOACTIVATE);
                        }

                        if (isCoveredByMe)
                        {
                            Win32.SetWindowPos(myHwnd, _isAlwaysOnTop ? Win32.HWND_TOPMOST : Win32.HWND_NOTOPMOST, origX, origY, 0, 0, Win32.SWP_NOSIZE | Win32.SWP_NOACTIVATE);
                        }
                        Thread.Sleep(15);
                        _isSimulatingInput = false;
                    }
                }
            });
        }

        private void ViewportContainer_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (_isSimulatingInput)
            {
                e.Handled = true;
                return;
            }

            // If user holds Alt or is not interactive, allow moving the window freely
            if (Keyboard.Modifiers.HasFlag(ModifierKeys.Alt) || !_isInteractive)
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
            {
                e.Handled = true;
                return;
            }

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
            {
                e.Handled = true;
                return;
            }

            e.Handled = true;
            Mouse.Capture(null);
            Point pt = e.GetPosition(RotatedImageHost);
            ForwardMouseClick(pt, isRightClick: true);
        }

        private void ViewportContainer_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            // Reentrancy guard: drop any event triggered by our own wheel simulation
            if (_isForwardingWheel || _isSimulatingInput) return;

            if (_isInteractive && _targetHwnd != IntPtr.Zero)
            {
                Point pt = e.GetPosition(RotatedImageHost);
                if (CalculateTargetCoordinates(pt, out int tx, out int ty) && Win32.GetWindowRect(_targetHwnd, out var rect))
                {
                    int screenX = rect.Left + tx;
                    int screenY = rect.Top + ty;
                    int delta = e.Delta;

                    IntPtr myHwnd = new WindowInteropHelper(this).Handle;
                    Win32.GetWindowRect(myHwnd, out var myRect);
                    int origX = myRect.Left;
                    int origY = myRect.Top;
                    bool isCoveredByMe = (screenX >= myRect.Left && screenX <= myRect.Right && screenY >= myRect.Top && screenY <= myRect.Bottom);

                    ThreadPool.QueueUserWorkItem(_ =>
                    {
                        lock (_wheelLock)
                        {
                            _isForwardingWheel = true;
                            int targetOriginalExStyle = 0;
                            try
                            {
                                if (_isGhostEnabled)
                                {
                                    targetOriginalExStyle = Win32.GetWindowLong(_targetHwnd, Win32.GWL_EXSTYLE);
                                    Win32.SetWindowLong(_targetHwnd, Win32.GWL_EXSTYLE, targetOriginalExStyle & ~Win32.WS_EX_TRANSPARENT);
                                    Win32.SetWindowPos(_targetHwnd, IntPtr.Zero, 0, 0, 0, 0, Win32.SWP_NOMOVE | Win32.SWP_NOSIZE | Win32.SWP_NOZORDER | Win32.SWP_FRAMECHANGED | Win32.SWP_NOACTIVATE);
                                }

                                Win32.GetCursorPos(out var oldCursor);
                                Win32.SetCursorPos(screenX, screenY);

                                if (isCoveredByMe)
                                {
                                    Win32.SetWindowPos(myHwnd, IntPtr.Zero, -15000, -15000, 0, 0, Win32.SWP_NOSIZE | Win32.SWP_NOZORDER | Win32.SWP_NOACTIVATE);
                                }

                                Win32.mouse_event(Win32.MOUSEEVENTF_WHEEL, 0, 0, (uint)delta, UIntPtr.Zero);

                                if (isCoveredByMe)
                                {
                                    Win32.SetWindowPos(myHwnd, _isAlwaysOnTop ? Win32.HWND_TOPMOST : Win32.HWND_NOTOPMOST, origX, origY, 0, 0, Win32.SWP_NOSIZE | Win32.SWP_NOACTIVATE);
                                }

                                Win32.SetCursorPos(oldCursor.X, oldCursor.Y);
                            }
                            finally
                            {
                                if (_isGhostEnabled)
                                {
                                    Win32.SetWindowLong(_targetHwnd, Win32.GWL_EXSTYLE, targetOriginalExStyle | Win32.WS_EX_TRANSPARENT);
                                    Win32.SetWindowPos(_targetHwnd, IntPtr.Zero, 0, 0, 0, 0, Win32.SWP_NOMOVE | Win32.SWP_NOSIZE | Win32.SWP_NOZORDER | Win32.SWP_FRAMECHANGED | Win32.SWP_NOACTIVATE);
                                }

                                if (isCoveredByMe)
                                {
                                    Win32.SetWindowPos(myHwnd, _isAlwaysOnTop ? Win32.HWND_TOPMOST : Win32.HWND_NOTOPMOST, origX, origY, 0, 0, Win32.SWP_NOSIZE | Win32.SWP_NOACTIVATE);
                                }
                                Thread.Sleep(8);
                                _isForwardingWheel = false;
                            }
                        }
                    });
                }
                e.Handled = true;
            }
        }
        #endregion
    }
}
