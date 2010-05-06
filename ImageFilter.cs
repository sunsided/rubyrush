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
                    byte red = *src;
                    byte green = *(src + 1);
                    byte blue = *(src + 2);

                    // ... und kopieren
                    byte* target = targetOffset + targetBase + x;
                    *target = red;
                    *(target + 1) = green;
                    *(target + 2) = blue;
                }
            }

            // Bilder freigeben
            bitmap.UnlockBits(targetData);
            capture.UnlockBits(captureData);

            return bitmap;
        }
    }
}
