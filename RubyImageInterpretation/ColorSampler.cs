// ID $Id$

using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace RubyImageInterpretation
{
    /// <summary>
    /// Klasse, die Farben aus einer Bildposition ermittelt
    /// </summary>
    public sealed class ColorSampler
    {
        #region Atomisches samplen einer Farbe

        /// <summary>
        /// Ermittelt eine Farbe aus einer gegebenen Position
        /// </summary>
        /// <param name="bitmap">Das Ursprungsbild.</param>
        /// <param name="targetX">X-Koordinate.</param>
        /// <param name="targetY">Y-Koordinate.</param>
        /// <param name="width">Der Samplingradius.</param>
        /// <returns></returns>
        public unsafe static Color SampleColorAtomic(Bitmap bitmap, int targetX, int targetY, int width)
        {
            // Bildbereich ermitteln
            Rectangle imageRect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);

            // Bilder locken
            BitmapData captureData = bitmap.LockBits(imageRect, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);

            // Tatsächliche Breiten in Pixeln berechnen
            int pixelWidth = captureData.Width * 3;

            // Gesamtoffsets berechnen
            byte* srcOffset = (byte*)captureData.Scan0.ToPointer();

            int totalR = 0;
            int totalG = 0;
            int totalB = 0;
            int count = 0;

            int startY = Math.Max(0, targetY - width);
            int endY = Math.Min(targetY + width, imageRect.Height);
            int startX = Math.Max(0, targetX * 3 - width * 3);
            int endX = Math.Min(targetX * 3 + width * 3, pixelWidth);

            // Schleifen und Spaß haben
            for (int y = startY; y < endY; ++y)
            {
                // Y-Offsets berechnen
                int srcBase = y * captureData.Stride;

                // X-Position schleifen
                for (int x = startX; x < endX; x += 3)
                {
                    // auslesen ...
                    byte* src = srcOffset + srcBase + x;
                    totalB += *src;
                    totalG += *(src + 1);
                    totalR += *(src + 2);
                    ++count;
                }
            }

            // Bilder freigeben
            bitmap.UnlockBits(captureData);

            return Color.FromArgb(totalR / count, totalG / count, totalB / count);
        }

        #endregion

        #region Aufgeteiltes samplen einer Farbe

        /// <summary>
        /// Lockt ein Bild für spätere Verarbeitung
        /// </summary>
        /// <param name="bitmap">Das Bild</param>
        /// <returns>Die Lock-Informationen</returns>
        public BitmapData LockBitmap(Bitmap bitmap)
        {
            // Bildbereich ermitteln
            Rectangle imageRect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);

            // Bild locken
            return bitmap.LockBits(imageRect, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
        }

        /// <summary>
        /// Unlockt ein Bild
        /// </summary>
        /// <param name="bitmap">Das Bild</param>
        /// <param name="data">Die Lock-Informationen</param>
        public void UnlockBitmap(Bitmap bitmap, BitmapData data)
        {
            // Bilder freigeben
            bitmap.UnlockBits(data);
        }

        /// <summary>
        /// Ermittelt eine Farbe aus einer gegebenen Position
        /// </summary>
        /// <param name="captureData">Das Ursprungsbild.</param>
        /// <param name="targetX">X-Koordinate.</param>
        /// <param name="targetY">Y-Koordinate.</param>
        /// <param name="width">Der Samplingradius.</param>
        /// <returns></returns>
        public unsafe Color SampleColorFromLockedBitmap(BitmapData captureData, int targetX, int targetY, int width)
        {
            // Tatsächliche Breiten in Pixeln berechnen
            int pixelHeight = captureData.Height;
            int pixelWidth = captureData.Width * 3;

            // Gesamtoffsets berechnen
            byte* srcOffset = (byte*)captureData.Scan0.ToPointer();

            int totalR = 0;
            int totalG = 0;
            int totalB = 0;
            int count = 0;

            int startY = Math.Max(0, targetY - width);
            int endY = Math.Min(targetY + width, pixelHeight);
            int startX = Math.Max(0, targetX * 3 - width * 3);
            int endX = Math.Min(targetX * 3 + width * 3, pixelWidth);

            // Schleifen und Spaß haben
            for (int y = startY; y < endY; ++y)
            {
                // Y-Offsets berechnen
                int srcBase = y * captureData.Stride;

                // X-Position schleifen
                for (int x = startX; x < endX; x += 3)
                {
                    // auslesen ...
                    byte* src = srcOffset + srcBase + x;
                    totalB += *src;
                    totalG += *(src + 1);
                    totalR += *(src + 2);
                    ++count;
                }
            }

            return Color.FromArgb(totalR / count, totalG / count, totalB / count);
        }

        #endregion
    }
}
