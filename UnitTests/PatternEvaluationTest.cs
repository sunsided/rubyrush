// ID $Id$

// #define MATCH_PICTURE

using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using NUnit.Framework;
using RubyElement;
using RubyLogic;
using RubyLogic.PatternDefinition;
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

#if MATCH_PICTURE

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

#else

        /// <summary>
        /// Die Board-Definition entsprechend dem Beispielscreenshot
        /// </summary>
        public readonly StoneColor[,] BoardDefinition = new StoneColor[ElementsX, ElementsY]
                                                         {
                                                             {StoneColor.Orange, StoneColor.Blue, StoneColor.Blue, StoneColor.Green, StoneColor.White, StoneColor.Orange, StoneColor.Yellow, StoneColor.Green},
                                                             {StoneColor.Orange, StoneColor.White, StoneColor.White, StoneColor.Green, StoneColor.Green, StoneColor.Green, StoneColor.Yellow, StoneColor.Yellow},
                                                             {StoneColor.Violet, StoneColor.Orange, StoneColor.Green, StoneColor.White, StoneColor.Green, StoneColor.Green, StoneColor.Red, StoneColor.White},
                                                             {StoneColor.Orange, StoneColor.Violet, StoneColor.Yellow, StoneColor.Green, StoneColor.Green, StoneColor.Green, StoneColor.White, StoneColor.White},
                                                             {StoneColor.Orange, StoneColor.Orange, StoneColor.Red, StoneColor.Green, StoneColor.Green, StoneColor.White, StoneColor.Blue, StoneColor.Yellow},
                                                             {StoneColor.Violet, StoneColor.Green, StoneColor.Violet, StoneColor.White, StoneColor.White, StoneColor.Violet, StoneColor.Blue, StoneColor.Orange},
                                                             {StoneColor.Orange, StoneColor.Orange, StoneColor.White, StoneColor.Yellow, StoneColor.Blue, StoneColor.Blue, StoneColor.White, StoneColor.Blue},
                                                             {StoneColor.Yellow, StoneColor.Green, StoneColor.Blue, StoneColor.Green, StoneColor.Red, StoneColor.White, StoneColor.Green, StoneColor.Red},
                                                         };
#endif

        /// <summary>
        /// Die Pattern-Definitionen
        /// </summary>
        private IList<Pattern> _patternDefinitions;

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
        /// Wertet das Spielbrett nach einem bestimmten Muster aus
        /// </summary>
        public void EvaluateBoard(PatternNode pattern)
        {
            for (int y = 0; y < ElementsY; ++y)
            {
                for (int x = 0; x < ElementsX; ++x)
                {
                    Element element = _board[x, y];

                    // Nach rechts auswerten
                    PatternRunner node = new PatternRunner(pattern, Direction.Right, element);
                    if (node.EvaluateTree()) Trace.WriteLine(string.Format("Match {0}, Vorschlag: {1}", element, node.RecommendationCandiate));
                    if (node.EvaluateTree(true)) Trace.WriteLine(string.Format("Match {0} (Reverse), Vorschlag: {1}", element, node.RecommendationCandiate));

                    // Nach links auswerten
                    node = new PatternRunner(pattern, Direction.Left, element);
                    if (node.EvaluateTree()) Trace.WriteLine(string.Format("Match {0}, Vorschlag: {1}", element, node.RecommendationCandiate));
                    if (node.EvaluateTree(true)) Trace.WriteLine(string.Format("Match {0} (Reverse), Vorschlag: {1}", element, node.RecommendationCandiate));

                    // Nach oben auswerten
                    node = new PatternRunner(pattern, Direction.Up, element);
                    if (node.EvaluateTree()) Trace.WriteLine(string.Format("Match {0}, Vorschlag: {1}", element, node.RecommendationCandiate));
                    if (node.EvaluateTree(true)) Trace.WriteLine(string.Format("Match {0} (Reverse), Vorschlag: {1}", element, node.RecommendationCandiate));

                    // Nach unten auswerten
                    node = new PatternRunner(pattern, Direction.Down, element);
                    if (node.EvaluateTree()) Trace.WriteLine(string.Format("Match {0}, Vorschlag: {1}", element, node.RecommendationCandiate));
                    if (node.EvaluateTree(true)) Trace.WriteLine(string.Format("Match {0} (Reverse), Vorschlag: {1}", element, node.RecommendationCandiate));
                }
            }
        }

        /// <summary>
        /// Wertet das Spielbrett nach einem bestimmten Muster aus
        /// </summary>
        [Test]
        public void EvaluateAll()
        {
            // Alle Empfehlungen durchlaufen
            for (int p = 0; p < _patternDefinitions.Count; ++p)
            {
                Pattern pattern = _patternDefinitions[p];
                Trace.WriteLine("** Teste Muster " + pattern.Description + " (Rang " + pattern.Ranking + ")");
                List<Recommendation> recommendations = new List<Recommendation>();

                for (int y = 0; y < ElementsY; ++y)
                {
                    for (int x = 0; x < ElementsX; ++x)
                    {
                        Element element = _board[x, y];
                        IEnumerable<Recommendation> recs = pattern.Evaluate(element);
                        recommendations.AddRange(recs);
                    }
                }

                // Empfehlungen auswerten
                Trace.WriteLine(recommendations.Count + " Treffer gefunden.");
                recommendations = recommendations.Distinct().ToList();
                for (int r = 0; r < recommendations.Count; ++r)
                {
                    Recommendation rec = recommendations[r];
                    Trace.WriteLine(string.Format("Vorschlag: {0}", rec));
                }
                Trace.WriteLine("");
            }
        }

        /// <summary>
        /// Testet das XXOX-Muster
        /// </summary>
        [Test]
        public void TestXXOXPattern()
        {
            Trace.WriteLine("Teste XXOX-Muster");
            EvaluateBoard(DefineSimplePattern.CreateXXOXPattern());
        }

        /// <summary>
        /// Testet das XXO-Muster
        /// </summary>
        [Test]
        public void TestXXOPattern()
        {
            Trace.WriteLine("Teste XXO-Muster");
            EvaluateBoard(DefineSimplePattern.CreateXXOPattern());
        }

        /// <summary>
        /// Testet das XOX-Muster
        /// </summary>
        [Test]
        public void TestXOXPattern()
        {
            Trace.WriteLine("Teste XOX-Muster");
            EvaluateBoard(DefineSimplePattern.CreateXOXPattern());
        }
    }
}
