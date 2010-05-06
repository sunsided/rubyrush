// ID $Id$

using System.Drawing;
using System.Drawing.Imaging;

namespace Ruby_Rush
{
    public static class ImageFilter
    {
        public unsafe static Bitmap FilterBitmap(Bitmap capture)
        {
            // Neues Bild erstellen
            Bitmap bitmap = new Bitmap(capture.Width, capture.Height);
            Rectangle imageRect = new Rectangle(0, 0,bitmap.Width, bitmap.Height);

            // Bilder locken
            BitmapData captureData = capture.LockBits(imageRect, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            BitmapData targetData = bitmap.LockBits(imageRect, ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);

            // Schleifen und Spaß haben
            for (int y = 0; y < captureData.Height; ++y )
            {
                for(int x=0; x<captureData.Width; ++x)
                {
                    // auslesen ...
                    int srcpos = y*captureData.Stride + x*3;
                    byte* src = (byte*)captureData.Scan0.ToPointer() + srcpos;
                    byte red = *src;
                    byte green = *(src + 1);
                    byte blue = *(src + 2);

                    // ... und kopieren
                    int targetpos = y * targetData.Stride + x * 3;
                    byte* target = (byte*)targetData.Scan0.ToPointer() + targetpos;
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
