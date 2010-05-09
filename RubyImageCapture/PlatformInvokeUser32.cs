// ID $Id$

using System;
using System.Runtime.InteropServices;

namespace RubyImageCapture
{
    /// <summary>
    /// Funktionen für User32.dll-Aufrufe
    /// </summary>
    internal static class PlatformInvokeUser32
    {
        #region Class Variables

        public const int SmCxscreen = 0;
        public const int SmCyscreen = 1;

        #endregion

        #region Class Functions

        [DllImport("user32.dll", EntryPoint = "GetDesktopWindow")]
        public static extern IntPtr GetDesktopWindow();

        [DllImport("user32.dll", EntryPoint = "GetDC")]
        public static extern IntPtr GetDC(IntPtr ptr);

        [DllImport("user32.dll", EntryPoint = "GetSystemMetrics")]
        public static extern int GetSystemMetrics(int abc);

        [DllImport("user32.dll", EntryPoint = "GetWindowDC")]
        public static extern IntPtr GetWindowDC(Int32 ptr);

        [DllImport("user32.dll", EntryPoint = "ReleaseDC")]
        public static extern IntPtr ReleaseDC(IntPtr hWnd, IntPtr hDc);

        #endregion

    }
}
