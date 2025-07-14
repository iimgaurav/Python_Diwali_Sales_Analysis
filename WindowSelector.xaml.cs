using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;

namespace ScreenRecorder
{
    public partial class WindowSelector : Window
    {
        public IntPtr SelectedWindowHandle { get; private set; }
        public string SelectedWindowTitle { get; private set; }

        [DllImport("user32.dll")]
        private static extern bool EnumWindows(EnumWindowsProc enumProc, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll")]
        private static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint processId);

        [DllImport("user32.dll")]
        private static extern bool GetWindowRect(IntPtr hwnd, ref RECT rectangle);

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        public WindowSelector()
        {
            InitializeComponent();
            LoadWindows();
            WindowsList.SelectionChanged += WindowsList_SelectionChanged;
        }

        private void WindowsList_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            SelectBtn.IsEnabled = WindowsList.SelectedItem != null;
        }

        private void LoadWindows()
        {
            var windows = new List<WindowInfo>();
            
            EnumWindows((hWnd, lParam) =>
            {
                if (IsWindowVisible(hWnd) && GetWindowTextLength(hWnd) > 0)
                {
                    var title = GetWindowTitle(hWnd);
                    if (!string.IsNullOrEmpty(title) && !IsSystemWindow(title))
                    {
                        var processName = GetProcessName(hWnd);
                        var rect = GetWindowRect(hWnd);
                        
                        // Only include windows with reasonable size
                        if (rect.Width > 100 && rect.Height > 100)
                        {
                            windows.Add(new WindowInfo
                            {
                                Handle = hWnd,
                                Title = title,
                                ProcessName = processName
                            });
                        }
                    }
                }
                return true;
            }, IntPtr.Zero);

            WindowsList.ItemsSource = windows.OrderBy(w => w.Title);
        }

        private string GetWindowTitle(IntPtr hWnd)
        {
            int length = GetWindowTextLength(hWnd);
            if (length == 0) return string.Empty;

            var builder = new StringBuilder(length + 1);
            GetWindowText(hWnd, builder, builder.Capacity);
            return builder.ToString();
        }

        private string GetProcessName(IntPtr hWnd)
        {
            try
            {
                GetWindowThreadProcessId(hWnd, out uint processId);
                var process = Process.GetProcessById((int)processId);
                return process.ProcessName;
            }
            catch
            {
                return "Unknown";
            }
        }

        private RECT GetWindowRect(IntPtr hWnd)
        {
            RECT rect = new RECT();
            GetWindowRect(hWnd, ref rect);
            return rect;
        }

        private bool IsSystemWindow(string title)
        {
            // Filter out common system windows
            var systemTitles = new[]
            {
                "Program Manager",
                "Desktop Window Manager",
                "Windows Input Experience",
                "Microsoft Text Input Application",
                "Windows Shell Experience"
            };

            return systemTitles.Any(sysTitle => 
                title.Equals(sysTitle, StringComparison.OrdinalIgnoreCase) ||
                title.Contains(sysTitle));
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            LoadWindows();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void Select_Click(object sender, RoutedEventArgs e)
        {
            if (WindowsList.SelectedItem is WindowInfo selectedWindow)
            {
                SelectedWindowHandle = selectedWindow.Handle;
                SelectedWindowTitle = selectedWindow.Title;
                DialogResult = true;
                Close();
            }
        }
    }

    public class WindowInfo
    {
        public IntPtr Handle { get; set; }
        public string Title { get; set; }
        public string ProcessName { get; set; }
    }
}