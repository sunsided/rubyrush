// ID $Id$

using System;
using System.Diagnostics.Contracts;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;
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
        /// <param name="gridStartX">The grid start X.</param>
        /// <param name="gridStartY">The grid start Y.</param>
        /// <param name="gridWidth">Width of the grid.</param>
        /// <param name="gridHeight">Height of the grid.</param>
        /// <param name="elementsX">The elements X.</param>
        /// <param name="elementsY">The elements Y.</param>
        /// <returns></returns>
        [Pure]
        public static Checkerboard GenerateGrid(Bitmap input, int gridStartX, int gridStartY, int gridWidth, int gridHeight, int elementsX, int elementsY)
        {
            lock (input)
            {
                // Der Sampling-Radius, innerhalb dessen die Farbe bewertet werden soll
                const int samplingRadius = 15;

                // Gitter erzeugen
                Checkerboard grid = new Checkerboard(elementsX, elementsY);

                // Schrittweite ermitteln
                int widthSteps = gridWidth/(elementsX - 1);
                    // -1, da das Gitter auf den Elementen liegt (nicht um sie herum)
                int heightSteps = gridHeight/(elementsY - 1);

                // Bild-Lock erstellen
                ColorSampler sampler = new ColorSampler();
                BitmapData bitmapData = sampler.LockBitmap(input);

                // Erzeugen der Elemente in einzelnen Threads
                Parallel.For(0, elementsY, y =>
                                               {
                                                   int ypos = heightSteps*y + gridStartY;
                                                   for (int x = 0; x < elementsX; x++)
                                                   {
                                                       int xpos = widthSteps*x + gridStartX;

                                                       Color rawcolor = sampler.SampleColorFromLockedBitmap(bitmapData,
                                                                                                            xpos, ypos,
                                                                                                            samplingRadius);
                                                       StoneColor color = DetermineElementType(rawcolor);

                                                       // Element erzeugen
                                                       Element element = color == StoneColor.Unknown
                                                                             ? (Element) new UnknownElement(grid, x, y, rawcolor)
                                                                             : new KnownElement(grid, x, y, rawcolor, color);

                                                       // Und zuweisen
                                                       grid[x, y] = element;
                                                   }
                                               });

                // Entsperrt das Bild
                sampler.UnlockBitmap(input, bitmapData);

                // Und raus
                return grid;
            }
        }

        /// <summary>
        /// Ermittelt die Differenz zweier Farben
        /// </summary>
        /// <param name="color1">Farbe 1</param>
        /// <param name="color2">Farbe 2</param>
        /// <returns></returns>
        [Pure]
        private static int GetDifference(Color color1, Color color2)
        {
            return Math.Abs(color1.R - color2.R) + Math.Abs(color1.G - color2.G) + Math.Abs(color1.B - color2.B);
        }

        /// <summary>
        /// Ermittelt den Elementtyp aus der Basisfarbe
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        [Pure]
        private static StoneColor DetermineElementType(Color color)
        {
            // Farbdifferenzen ermitteln
            int redDifference = GetDifference(Color.Red, color);
            int greenDifference = GetDifference(Color.Green, color);
            int blueDifference = GetDifference(Color.Blue, color);
            int violetDifference = GetDifference(Color.Violet, color);
            int orangeDifference = GetDifference(Color.Orange, color);
            int yellowDifference = GetDifference(Color.Yellow, color);
            int whiteDifference = GetDifference(Color.White, color);

            // Minimum ermitteln
            int smallestValue = Min(redDifference, greenDifference, blueDifference, violetDifference, orangeDifference,
                                    yellowDifference, whiteDifference);

            // Minimum vergleichen
            if (smallestValue == redDifference) return StoneColor.Red;
            if (smallestValue == greenDifference) return StoneColor.Green;
            if (smallestValue == blueDifference) return StoneColor.Blue;
            if (smallestValue == violetDifference) return StoneColor.Violet;
            if (smallestValue == orangeDifference) return StoneColor.Orange;
            if (smallestValue == yellowDifference) return StoneColor.Yellow;
            if (smallestValue == whiteDifference) return StoneColor.White;
            return StoneColor.Unknown;
        }

        /// <summary>
        /// Ermittelt das Minimum beliebig vieler Werte
        /// </summary>
        /// <param name="value1">Wert 1</param>
        /// <param name="value2">Wert 2</param>
        /// <param name="values">weitere Werte</param>
        /// <returns></returns>
        [Pure]
        private static int Min(int value1, int value2, params int[] values)
        {
            int min = Math.Min(value1, value2);
            for (int i = 0; i < values.Length; ++i)
            {
                min = Math.Min(min, values[i]);
            }
            return min;
        }
    }
}
