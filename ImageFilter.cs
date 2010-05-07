// ID $Id$

using System.Drawing;
using System.Drawing.Imaging;

namespace Ruby_Rush
{
    /// <summary>
    /// Klasse, die Bilder filtert
    /// </summary>
    public static class ImageFilter
    {
        private unsafe static byte HorizontalLineDetectionKernel(int x, int y, int stride, byte* srcOffset, int width, int height)
        {
            int yCurrentLine = y * stride;
            int yLineAbove = yCurrentLine - stride;
            int yLineBelow = yCurrentLine + stride;

            int sum = 0;

            // [-1, 0]
            if(x > 0)
            {
                byte* src = srcOffset + yCurrentLine + x - 1;
                byte blue = *src;
                byte green = *(src + 1);
                byte red = *(src + 2);
                int total = (blue + green + red) / 3;
                sum += total*-1;
            }

            // [0, 0]
            {
                byte* src = srcOffset + yCurrentLine + x;
                byte blue = *src;
                byte green = *(src + 1);
                byte red = *(src + 2);
                int total = (blue + green + red) / 3;
                sum += total * 2;
            }

            // [+1, 0]
            if(x < width - 1)
            {
                byte* src = srcOffset + yCurrentLine + x + 1;
                byte blue = *src;
                byte green = *(src + 1);
                byte red = *(src + 2);
                int total = (blue + green + red) / 3;
                sum += total * -1;
            }

            // [-1, -1]
            if (y > 0 && x > 0)
            {
                byte* src = srcOffset + yLineAbove + x - 1;
                byte blue = *src;
                byte green = *(src + 1);
                byte red = *(src + 2);
                int total = (blue + green + red) / 3;
                sum += total * -1;
            }

            // [0, -1]
            if (y > 0 && x < width - 1)
            {
                byte* src = srcOffset + yLineAbove + x;
                byte blue = *src;
                byte green = *(src + 1);
                byte red = *(src + 2);
                int total = (blue + green + red) / 3;
                sum += total * 2;
            }

            // [+1, -1]
            if (y > 0)
            {
                byte* src = srcOffset + yLineAbove + x + 1;
                byte blue = *src;
                byte green = *(src + 1);
                byte red = *(src + 2);
                int total = (blue + green + red) / 3;
                sum += total * -1;
            }

            // [-1, +1]
            if (y < height-1 && x > 0)
            {
                byte* src = srcOffset + yLineBelow + x - 1;
                byte blue = *src;
                byte green = *(src + 1);
                byte red = *(src + 2);
                int total = (blue + green + red) / 3;
                sum += total * -1;
            }

            // [0, +1]
            if (y < height - 1)
            {
                byte* src = srcOffset + yLineBelow + x;
                byte blue = *src;
                byte green = *(src + 1);
                byte red = *(src + 2);
                int total = (blue + green + red) / 3;
                sum += total * 2;
            }

            // [+1, +1]
            if (y < height - 1 && x < width - 1)
            {
                byte* src = srcOffset + yLineBelow + x + 1;
                byte blue = *src;
                byte green = *(src + 1);
                byte red = *(src + 2);
                int total = (blue + green + red) / 3;
                sum += total * -1;
            }

            return (byte)(sum / 9);
        }

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
    }
}
