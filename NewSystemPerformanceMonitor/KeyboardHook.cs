using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

public static class KeyboardHook
{
    // Constants for Windows messages and hotkey registration
    private const int WM_HOTKEY = 0x0312;
    private static IntPtr hwnd = IntPtr.Zero;
    private static int hotkeyId = 1;
    private static Action HotkeyAction;

    /// <summary>
    /// Initializes and registers a hotkey.
    /// </summary>
    /// <param name="window">The window to associate the hotkey with.</param>
    /// <param name="virtualKeyCode">The virtual key code of the hotkey.</param>
    /// <param name="modifier">The modifier key(s) for the hotkey.</param>
    /// <param name="action">The action to invoke when the hotkey is pressed.</param>
    public static void SetHotkey(Window window, int virtualKeyCode, KeyModifier modifier, Action action)
    {
        hwnd = new WindowInteropHelper(window).Handle; // Get the window handle

        // Check if hwnd is valid
        if (hwnd == IntPtr.Zero)
        {
            MessageBox.Show("Hwnd of zero is not valid.");
            return;
        }

        // Register the hotkey
        RegisterHotKey(hwnd, hotkeyId, (uint)modifier, (uint)virtualKeyCode);

        // Get the HwndSource from the window handle
        HwndSource source = HwndSource.FromHwnd(hwnd);
        if (source != null)
        {
            // Add the window procedure hook
            source.AddHook(WndProc);
        }

        HotkeyAction = action; // Set the action to be invoked on hotkey press
    }

    /// <summary>
    /// Unregisters the hotkey.
    /// </summary>
    public static void UnregisterHotkey()
    {
        if (hwnd != IntPtr.Zero)
        {
            UnregisterHotKey(hwnd, hotkeyId); // Unregister the hotkey
        }
    }

    /// <summary>
    /// Window procedure to handle Windows messages.
    /// </summary>
    /// <param name="hwnd">The window handle.</param>
    /// <param name="msg">The message ID.</param>
    /// <param name="wParam">Message parameters.</param>
    /// <param name="lParam">Message parameters.</param>
    /// <param name="handled">Indicates whether the message was handled.</param>
    /// <returns>IntPtr.Zero to indicate that the message has been processed.</returns>
    private static IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
        if (msg == WM_HOTKEY)
        {
            if (wParam.ToInt32() == hotkeyId)
            {
                HotkeyAction?.Invoke(); // Invoke the hotkey action
                handled = true; // Mark the message as handled
            }
        }
        return IntPtr.Zero;
    }

    // Importing necessary functions from user32.dll for hotkey registration and unregistration
    [DllImport("user32.dll")]
    private static extern int RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

    [DllImport("user32.dll")]
    private static extern bool UnregisterHotKey(IntPtr hWnd, int id);
}

/// <summary>
/// Enumeration for key modifier flags.
/// </summary>
[Flags]
public enum KeyModifier
{
    None = 0,
    Alt = 1,
    Control = 2,
    Shift = 4,
    Windows = 8
}
