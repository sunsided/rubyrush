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
        /// <param name="radius">Der Samplingradius.</param>
        /// <returns></returns>
        public unsafe Color SampleColorFromLockedBitmap(BitmapData captureData, int targetX, int targetY, int radius)
        {
            // Tatsächliche Breiten in Pixeln berechnen
            int pixelHeight = captureData.Height;
            int pixelWidth = captureData.Width * 3;
            int realTargetY = targetY;
            int realTargetX = targetX*3;
            int radiusY = radius;
            int radiusX = radius * 3;

            // Gesamtoffsets berechnen
            byte* srcOffset = (byte*)captureData.Scan0.ToPointer();

            int totalR = 0;
            int totalG = 0;
            int totalB = 0;
            int count = 0;

            int startY = Math.Max(0, realTargetY - radiusY);
            int endY = Math.Min(realTargetY + radiusY, pixelHeight);
            int startX = Math.Max(0, realTargetX - radiusX);
            int endX = Math.Min(realTargetX + radiusX, pixelWidth);

            // Schleifen und Spaß haben
            for (int y = startY; y < endY; ++y)
            {
                // Y-Offsets berechnen
                int srcBase = y * captureData.Stride;

                // X-Position schleifen
                for (int x = startX; x < endX; x += 3)
                {
                    // Position wichten
                    // Der Wichtungsalgorithmus ist recht simpel. Es wird die absolute Differenz von jeweils
                    // X- und Y-Position zum Mittelpunkt berechnet und addiert. Ist diese Differenz größer
                    // als der Radius, wird der Wert übersprungen. Dadurch entsteht eine Raute.
                    // Beispiel mit Radius 4:
                    //                                      
                    //  -   -   -   -   4   -   -   -   -   
                    //  -   -   -   4   3   4   -   -   -   
                    //  -   -   4   3   2   3   4   -   -   
                    //  -   4   3   2   1   2   3   4   -   
                    //  4   3   2   1   0   1   2   3   4   
                    //  -   4   3   2   1   2   3   4   -   
                    //  -   -   4   3   2   3   4   -   -   
                    //  -   -   -   4   3   4   -   -   -   
                    //  -   -   -   -   4   -   -   -   -   
                    //                                      
                    //  -   -   -   -   x   -   -   -   -   
                    //  -   -   -   x   x   4|  -   -   -   
                    //  -   -   x   x   x   3|  x   -   -   
                    //  -   x   x   x   x   2|  x   x   -   
                    //  x   x   x   x   0---1+  x   x   x   
                    //  -   x   x   x  |1   x   x   x   -   
                    //  -   -   4---3--+2   x   x   -   -   
                    //  -   -   -   x   x   x   -   -   -   
                    //  -   -   -   -   x   -   -   -   -   
                    //                                      
                    // Der Vorteil der Rautenform ist der, dass "spitze" Steine wie Dreieck und Raute
                    // besser erfasst werden, da weniger Hintergrund einbezogen wird, der das Ergebnis
                    // verfälscht.
                    int diffX = Math.Abs(realTargetX - x);
                    int diffY = Math.Abs(realTargetY - y);
                    if (diffX + diffY > radius) continue;

                    // Werte ermitteln und addieren
                    byte* src = srcOffset + srcBase + x;
                    totalB += *src;
                    totalG += *(src + 1);
                    totalR += *(src + 2);
                    ++count;
                }
            }

            // Farbanteile arithmetisch mitteln und zurückgeben
            return Color.FromArgb(totalR / count, totalG / count, totalB / count);
        }

        #endregion
    }
}
