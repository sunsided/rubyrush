// ID $Id$

using System.Drawing;
using System.Drawing.Imaging;
using RubyElement;

namespace RubyImageInterpretation
{
    /// <summary>
    /// Generiert ein Elementgitter aus einem Bild und Rahmeninformationen
    /// </summary>
    public static class GridGenerator
    {
        /// <summary>
        /// Erzeugt das Gitter
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="gridWidth">Width of the grid.</param>
        /// <param name="gridHeight">Height of the grid.</param>
        /// <param name="elementsX">The elements X.</param>
        /// <param name="elementsY">The elements Y.</param>
        /// <returns></returns>
        public static Element[,] GenerateGrid(Bitmap input, int gridStartX, int gridStartY, int gridWidth, int gridHeight, int elementsX, int elementsY)
        {
            // Gitter erzeugen
            Element[,] grid = new Element[elementsX, elementsY];

            // Schrittweite ermitteln
            int widthSteps = gridWidth/(elementsX - 1); // -1, da das Gitter auf den Elementen liegt (nicht um sie herum)
            int heightSteps = gridHeight/(elementsY - 1);
            
            // Bild-Lock erstellen
            ColorSampler sampler = new ColorSampler();
            BitmapData bitmapData = sampler.LockBitmap(input);

            // TODO: Parallelisieren; Image-Lock rausziehen)
            for (int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 8; x++)
                {
                    int xpos = widthSteps * x + gridStartX;
                    int ypos = heightSteps * y + gridStartY;

                    Color color = sampler.SampleColorFromLockedBitmap(bitmapData, xpos, ypos, 10);
                    grid[x, y] = new KnownElement(color, StoneColor.Unknown);
                }
            }

            // Entsperrt das Bild
            sampler.UnlockBitmap(input, bitmapData);

            // Und raus);
            return grid;
        }
    }
}
