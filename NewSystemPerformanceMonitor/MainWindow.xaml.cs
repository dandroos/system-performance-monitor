using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Threading;

namespace PerformanceMonitorApp
{
    public partial class PerformanceMonitorWindow : Window
    {
        // Instance of PerformanceMonitor to fetch performance data and configuration settings
        private readonly PerformanceMonitor monitor = new PerformanceMonitor();
        private readonly DispatcherTimer updateTimer = new DispatcherTimer();

        // Constants for window styles
        private const int GWL_EXSTYLE = -20;
        private const int WS_EX_LAYERED = 0x00080000;
        private const int WS_EX_TRANSPARENT = 0x00000020;
        private const int WS_EX_TOPMOST = 0x00000008;

        // Constants for window positioning
        private static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        private const uint SWP_NOSIZE = 0x0001;
        private const uint SWP_NOMOVE = 0x0002;
        private const uint SWP_SHOWWINDOW = 0x0040;

        // Importing necessary functions from user32.dll for window manipulation
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        // Constructor
        public PerformanceMonitorWindow()
        {
            InitializeComponent();
            this.Loaded += OnWindowLoaded; // Event handler for when the window is loaded
            this.Closed += OnWindowClosed; // Event handler for when the window is closed
            StartMonitoring(); // Start monitoring performance data
        }

        // Event handler for when the window is loaded
        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            ConfigureWindow(); // Configure the window settings
        }

        // Event handler for when the window is closed
        private void OnWindowClosed(object sender, EventArgs e)
        {
            KeyboardHook.UnregisterHotkey(); // Clean up hotkey registration
        }

        // Configure window settings
        private void ConfigureWindow()
        {
            PerformanceTextBlock.FontSize = 16; // Set font size

            // Set up the hotkey to toggle window visibility
            KeyboardHook.SetHotkey(this, KeyInterop.VirtualKeyFromKey(Key.F10), KeyModifier.None, ToggleVisibility);

            // Make the window always on top
            this.Topmost = true;

            // Make the window click-through
            MakeClickThrough();
        }

        // Make the window click-through
        private void MakeClickThrough()
        {
            WindowInteropHelper helper = new WindowInteropHelper(this);
            IntPtr hwnd = helper.Handle;
            int exStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
            int newExStyle = exStyle | WS_EX_LAYERED | WS_EX_TRANSPARENT | WS_EX_TOPMOST;
            SetWindowLong(hwnd, GWL_EXSTYLE, newExStyle);
            SetWindowPos(hwnd, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOSIZE | SWP_NOMOVE | SWP_SHOWWINDOW);
        }

        // Start monitoring performance data
        private void StartMonitoring()
        {
            updateTimer.Interval = TimeSpan.FromSeconds(1); // Set update interval to 1 second
            updateTimer.Tick += UpdatePerformanceData; // Event handler for timer tick
            updateTimer.Start(); // Start the timer
        }

        // Update performance data
        private void UpdatePerformanceData(object sender, EventArgs e)
        {
            var cpuUsage = monitor.GetCpuUsage();
            var ramUsage = monitor.GetRamUsage();
            var gpuUsage = monitor.GetGpuUsage();
            var cpuTemp = monitor.GetCpuTemperature();
            var gpuTemp = monitor.GetGpuTemperature();

            // Clear the existing content
            PerformanceTextBlock.Inlines.Clear();

            // Create and add the formatted content
            AddFormattedText("CPU: ", true);
            AddFormattedText($"{cpuUsage}%", false);
            AddFormattedText(" | ", false);
            AddFormattedText($"{cpuTemp}°C   ", false);
            AddFormattedText("GPU: ", true);
            AddFormattedText($"{gpuUsage}%", false);
            AddFormattedText(" | ", false);
            AddFormattedText($"{gpuTemp}°C   ", false);
            AddFormattedText("RAM: ", true);
            AddFormattedText($"{ramUsage}%", false);

            
        }

        // Add formatted text to the PerformanceTextBlock
        private void AddFormattedText(string text, bool isBold)
        {
            Run run = new Run(text)
            {
                FontWeight = isBold ? FontWeights.Bold : FontWeights.Normal
            };
            PerformanceTextBlock.Inlines.Add(run);
        }

        // Toggle the visibility of the window
        private void ToggleVisibility()
        {
            this.Visibility = this.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
        }
    }
}
