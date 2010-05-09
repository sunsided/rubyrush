// ID $Id$

using System;
using System.Drawing;

namespace RubyImageCapture
{
    /// <summary>
    /// Klasse zum Grabben eines Screenshots
    /// </summary>
    public static class CaptureScreen
    {
        #region Public Class Functions

        /// <summary>
        /// Erstellt einen Screenshot des Desktops, beginnend ab (0,0)
        /// </summary>
        /// <param name="maxWidth">Maximale Breite</param>
        /// <param name="maxHeight">Maximale Höhe</param>
        /// <returns>Die Bitmap</returns>
        public static Bitmap GetDesktopImage(int maxWidth, int maxHeight)
        {
            return GetDesktopImage(0, 0, maxWidth, maxHeight);
        }

        /// <summary>
        /// Erstellt einen Screenshot des Desktops
        /// </summary>
        /// <param name="left">Oberer, linker Bereich</param>
        /// <param name="top">Oberer, rechter Bereich</param>
        /// <param name="maxWidth">Maximale Breite</param>
        /// <param name="maxHeight">Maximale Höhe</param>
        /// <returns>Die Bitmap</returns>
        public static Bitmap GetDesktopImage(int left, int top, int maxWidth, int maxHeight)
        {
            // In size variable we shall keep the size of the screen.
            Size size;

            // Variable to keep the handle to bitmap.
            IntPtr hBitmap = IntPtr.Zero;
            IntPtr desktopWindow = IntPtr.Zero;
            IntPtr hDc = IntPtr.Zero, hMemDc = IntPtr.Zero;

            try
            {

                // Here we get the handle to the desktop device context.
                desktopWindow = PlatformInvokeUser32.GetDesktopWindow();
                hDc = PlatformInvokeUser32.GetDC(desktopWindow);

                // Here we make a compatible device context in memory for screen device context.
                hMemDc = PlatformInvokeGDI32.CreateCompatibleDC(hDc);

                // We pass SM_CXSCREEN constant to GetSystemMetrics to get the X coordinates of the screen.
                size.Cx = PlatformInvokeUser32.GetSystemMetrics(PlatformInvokeUser32.SmCxscreen);
                size.Cx = Math.Min(maxWidth, size.Cx);

                // We pass SM_CYSCREEN constant to GetSystemMetrics to get the Y coordinates of the screen.
                size.Cy = PlatformInvokeUser32.GetSystemMetrics(PlatformInvokeUser32.SmCyscreen);
                size.Cy = Math.Min(maxHeight, size.Cy);

                // We create a compatible bitmap of the screen size and using the screen device context.
                hBitmap = PlatformInvokeGDI32.CreateCompatibleBitmap(hDc, size.Cx, size.Cy);

                // As hBitmap is IntPtr, we cannot check it against null. For this purpose, IntPtr.Zero is used.
                if (hBitmap != IntPtr.Zero)
                {
                    // Here we select the compatible bitmap in the memeory device context and keep the refrence to the old bitmap.
                    IntPtr hOld = PlatformInvokeGDI32.SelectObject(hMemDc, hBitmap);

                    // We copy the Bitmap to the memory device context.
                    PlatformInvokeGDI32.BitBlt(hMemDc, 0, 0, size.Cx, size.Cy, hDc, left, top, PlatformInvokeGDI32.Srccopy);

                    // We select the old bitmap back to the memory device context.
                    PlatformInvokeGDI32.SelectObject(hMemDc, hOld);

                    // We delete the memory device context.
                    PlatformInvokeGDI32.DeleteDC(hMemDc);
                    hMemDc = IntPtr.Zero;

                    // We release the screen device context.
                    PlatformInvokeUser32.ReleaseDC(desktopWindow, hDc);
                    hDc = IntPtr.Zero;

                    // Image is created by Image bitmap handle and stored in local variable.
                    Bitmap bmp = Image.FromHbitmap(hBitmap);

                    // Return the bitmap 
                    return bmp;
                }

                // If hBitmap is null, return null.
                return null;
            }
            finally
            {
                // release the screen device context.
                if (hMemDc != IntPtr.Zero)
                {
                    PlatformInvokeGDI32.DeleteDC(hMemDc);
                }

                // release the screen device context.
                if (hDc != IntPtr.Zero && desktopWindow != IntPtr.Zero)
                {
                    PlatformInvokeUser32.ReleaseDC(desktopWindow, hDc);
                }

                if (hBitmap != IntPtr.Zero)
                {
                    // Release the memory to avoid memory leaks.
                    PlatformInvokeGDI32.DeleteObject(hBitmap);

                    // This statement runs the garbage collector manually.
                    GC.Collect();
                }
            }
        }
        #endregion
    }

}
