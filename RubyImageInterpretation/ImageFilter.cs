// ID $Id$

using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace RubyImageInterpretation
{
    /// <summary>
    /// Klasse, die Bilder filtert
    /// </summary>
    public static class ImageFilter
    {
        /// <summary>
        /// Filtert ein Bild ... irgendwie.
        /// </summary>
        /// <param name="capture">Das Originalbild</param>
        /// <returns>Das gefilterte Bild</returns>
        public unsafe static Bitmap FilterBitmap(Bitmap capture)
        {
            // Neues Bild erstellen
            Bitmap bitmap = new Bitmap(capture.Width, capture.Height);
            Rectangle imageRect = new Rectangle(0, 0,bitmap.Width, bitmap.Height);

            // Bilder locken
            BitmapData captureData = capture.LockBits(imageRect, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            BitmapData targetData = bitmap.LockBits(imageRect, ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);

            // Tatsächliche Breiten in Pixeln berechnen
            int pixelWidth = captureData.Width*3;

            // Gesamtoffsets berechnen
            byte* srcOffset = (byte*) captureData.Scan0.ToPointer();
            byte* targetOffset = (byte*)targetData.Scan0.ToPointer();
            
            // Schleifen und Spaß haben
            for (int y = 0; y < captureData.Height; ++y )
            {
                // Y-Offsets berechnen
                int srcBase = y*captureData.Stride;
                int targetBase = y * targetData.Stride;

                // X-Position schleifen
                for (int x = 0; x < pixelWidth; x+=3)
                {
                    // auslesen ...
                    byte* src = srcOffset + srcBase + x;
                    byte blue = *src;
                    byte green = *(src + 1);
                    byte red = *(src + 2);
                    
                    // Farben diskretisieren
                    int color = (red + green + blue)/3;

                    // grüner Stein
                    if (green > 100 && green > red && green > blue && red < 200 && blue < 210)
                    {
                        green = 255;
                        red = 0;
                        blue = 0;
                    }
                    // weißer Stein
                    else if (color > 100 && (red == green) && (green == blue))
                    {
                        red = green = blue = 255;
                    }
                    // lila Stein
                    else if (color > 99 && (red == blue) && (green < red) && (green < blue))
                    {
                        red = blue = 255;
                        green = 0;
                    }
                    // gelber Stein
                    else if (color > 99 && (blue < green) && (green < red) && (red - green < 90))
                    {
                        red = 255;
                        green = 255;
                        blue = 0;
                    }
                    // orangener Stein
                    else if (color > 60 && (blue < green) && (green < red) && (red > 149) && (blue < 170))
                    {
                        red = 255;
                        green = 64;
                        blue = 0;
                    }
                    // blauer Stein
                    else if (color > 29 && (red < green) && (green < blue))
                    {
                        red = 0;
                        green = 0;
                        blue = 255;
                    }
                    // roter Stein
                    else if (color > 48 && (green < blue) && (blue < red) && (red - blue) > 64)
                    {
                        red = 255;
                        green = 0;
                        blue = 0;
                    }
                    else
                    {
                        red = green = blue = 0;
                    }

                    // ... und kopieren
                    byte* target = targetOffset + targetBase + x;
                    *target = blue;
                    *(target + 1) = green;
                    *(target + 2) = red;
                }
            }

            // Bilder freigeben
            bitmap.UnlockBits(targetData);
            capture.UnlockBits(captureData);

            return bitmap;
        }

        public unsafe static Color SampleColor(Bitmap bitmap, int targetX, int targetY, int width)
        {
            // Neues Bild erstellen
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
            int startX = Math.Max(0, targetX*3 - width*3);
            int endX = Math.Min(targetX*3 + width*3, pixelWidth);

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
    }
}
