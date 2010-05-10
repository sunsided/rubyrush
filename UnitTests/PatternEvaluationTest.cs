// ID $Id$

using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using NUnit.Framework;
using RubyElement;
using RubyLogic;
using RubyLogic.PatternTree;

namespace UnitTests
{
    /// <summary>
    /// Testet die Patternauswertung
    /// </summary>
    [TestFixture]
    public sealed class PatternEvaluationTest
    {
        /// <summary>
        /// Elemente in X-Richtung
        /// </summary>
        public const int ElementsX = 8;

        /// <summary>
        /// Elemente in Y-Richtung
        /// </summary>
        public const int ElementsY = 8;

        /// <summary>
        /// Das Spielbrett
        /// </summary>
        private readonly Checkerboard _board = new Checkerboard(ElementsX, ElementsY);

        /// <summary>
        /// Die Board-Definition entsprechend dem Beispielscreenshot
        /// </summary>
        public readonly StoneColor[,] BoardDefinition = new StoneColor[ElementsX, ElementsY]
                                                         {
                                                             {StoneColor.Orange, StoneColor.Blue, StoneColor.Blue, StoneColor.Red, StoneColor.White, StoneColor.Orange, StoneColor.Yellow, StoneColor.Green},
                                                             {StoneColor.Orange, StoneColor.White, StoneColor.White, StoneColor.Blue, StoneColor.Green, StoneColor.Green, StoneColor.Yellow, StoneColor.Yellow},
                                                             {StoneColor.Violet, StoneColor.Red, StoneColor.White, StoneColor.Green, StoneColor.White, StoneColor.Yellow, StoneColor.Red, StoneColor.White},
                                                             {StoneColor.Violet, StoneColor.Violet, StoneColor.Yellow, StoneColor.Orange, StoneColor.Green, StoneColor.Green, StoneColor.White, StoneColor.White},
                                                             {StoneColor.White, StoneColor.Orange, StoneColor.Red, StoneColor.Yellow, StoneColor.Green, StoneColor.White, StoneColor.Violet, StoneColor.Yellow},
                                                             {StoneColor.Violet, StoneColor.Green, StoneColor.Violet, StoneColor.White, StoneColor.White, StoneColor.Violet, StoneColor.White, StoneColor.Orange},
                                                             {StoneColor.Orange, StoneColor.Orange, StoneColor.White, StoneColor.Yellow, StoneColor.Red, StoneColor.Blue, StoneColor.White, StoneColor.Violet},
                                                             {StoneColor.Yellow, StoneColor.Green, StoneColor.Blue, StoneColor.Green, StoneColor.Red, StoneColor.White, StoneColor.Green, StoneColor.Red},
                                                         };

        /// <summary>
        /// Die Pattern-Definitionen
        /// </summary>
        private IList<PatternNode> _patternDefinitions;

        /// <summary>
        /// Erzeugt ein Element
        /// </summary>
        /// <param name="x">X-Index</param>
        /// <param name="y">Y-Index</param>
        /// <param name="color">Die Steinfarbe</param>
        private void CreateElement(int x, int y, StoneColor color)
        {
            _board[x, y] = new KnownElement(_board, x, y, Color.Empty, color);
        }

        /// <summary>
        /// Bereitet das Spielfeld entsprechend der <see cref="BoardDefinition"/> vor.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            // Spielbrett erzeugen
            for (int y = 0; y < ElementsY; ++y)
            {
                for (int x = 0; x < ElementsX; ++x)
                {
                    StoneColor color = BoardDefinition[y, x]; // Die Boarddefinition ist gespiegelt! think about it! :D
                    CreateElement(x, y, color);
                }
            }

            // Pattern ermitteln
            _patternDefinitions = PatternDefinitionAttribute.GetPatternDefinitions();
        }

        /// <summary>
        /// Testet die Auswertung des Spielbretts
        /// </summary>
        [Test]
        public void EvaluateBoard()
        {
            for (int y = 0; y < ElementsY; ++y)
            {
                for (int x = 0; x < ElementsX; ++x)
                {
                    Element element = _board[x, y];

                    // Alle Definitionen durchlaufen
                    for (int n = 0; n < _patternDefinitions.Count; ++n)
                    {
                        PatternNode pattern = _patternDefinitions[n];

                        // Nach rechts auswerten
                        PatternRunner node = new PatternRunner(pattern, Direction.Right, element);
                        if (node.EvaluateTree()) Trace.WriteLine(string.Format("Match {0}, Pattern #{1}, Vorschlag: {2}", element, n, node.RecommendationCandiate));

                        // Nach links auswerten
                        node = new PatternRunner(pattern, Direction.Left, element);
                        if (node.EvaluateTree()) Trace.WriteLine(string.Format("Match {0}, Pattern #{1}, Vorschlag: {2}", element, n, node.RecommendationCandiate));

                        // Nach oben auswerten
                        node = new PatternRunner(pattern, Direction.Up, element);
                        if (node.EvaluateTree()) Trace.WriteLine(string.Format("Match {0}, Pattern #{1}, Vorschlag: {2}", element, n, node.RecommendationCandiate));

                        // Nach unten auswerten
                        node = new PatternRunner(pattern, Direction.Down, element);
                        if (node.EvaluateTree()) Trace.WriteLine(string.Format("Match {0}, Pattern #{1}, Vorschlag: {2}", element, n, node.RecommendationCandiate));
                    }
                }
            }
        }
    }
}
