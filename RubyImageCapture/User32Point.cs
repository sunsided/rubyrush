// ID $Id$

using System.Runtime.InteropServices;

namespace RubyImageCapture
{
    /// <summary>
    /// Size-Struktur
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct User32Point
    {
        /// <summary>
        /// Breite
        /// </summary>
        public int X;

        /// <summary>
        /// Höhe
        /// </summary>
        public int Y;
    }
}
