using System;
using System.IO;
using System.Windows;
using System.Windows.Threading;

namespace AlwaysRotate
{
    public partial class App : Application
    {
        private static readonly string LogPath = @"E:\anti\Utilidades\bin\AlwaysRotate\app_lifecycle.log";

        protected override void OnStartup(StartupEventArgs e)
        {
            try
            {
                Win32.SetCurrentProcessExplicitAppUserModelID("Rocaman.AlwaysRotate");
            }
            catch { }

            File.AppendAllText(LogPath, $"[{DateTime.Now:HH:mm:ss.fff}] App OnStartup\n");

            AppDomain.CurrentDomain.UnhandledException += (s, ev) =>
            {
                File.AppendAllText(LogPath, $"[{DateTime.Now:HH:mm:ss.fff}] AppDomain Unhandled: {ev.ExceptionObject}\n");
            };

            DispatcherUnhandledException += (s, ev) =>
            {
                File.AppendAllText(LogPath, $"[{DateTime.Now:HH:mm:ss.fff}] Dispatcher Unhandled: {ev.Exception}\n");
                ev.Handled = false;
            };

            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            File.AppendAllText(LogPath, $"[{DateTime.Now:HH:mm:ss.fff}] App OnExit with code: {e.ApplicationExitCode}\n");
            base.OnExit(e);
        }
    }
}
