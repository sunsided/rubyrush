// ID $Id$

using System;
using System.Collections.Generic;
using RubyElement;
using RubyLogic.PatternTree;

namespace RubyLogic
{
    public sealed class Pattern
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PatternDefinitionAttribute"/> class.
        /// </summary>
        /// <param name="rootNode">Der Wurzelknoten</param>
        /// <param name="ranking">The ranking.</param>
        /// <param name="description">The description.</param>
        /// <param name="isSymmetric">Gibt an, ob das Muster symmetrisch ist.</param>
        public Pattern(PatternNode rootNode, int ranking, string description = null, bool isSymmetric = false)
        {
            Ranking = ranking;
            Description = description ?? String.Empty;
            IsSymmetric = isSymmetric;
            RootNode = rootNode;
        }

        /// <summary>
        /// Gibt an, ob das Muster symmetrisch ist
        /// </summary>
        public bool IsSymmetric { get; private set; }

        /// <summary>
        /// Die Bewertung des Patterns. Je höher, desto besser.
        /// </summary>
        public int Ranking { get; private set; }

        /// <summary>
        /// Die Bezeichnung des Patterns.
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// Der Wurzelknoten
        /// </summary>
        public PatternNode RootNode { get; private set; }

        /// <summary>
        /// Wertet das Muster aus
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public IEnumerable<Recommendation> Evaluate(Element element)
        {
            if (element == null) throw new ArgumentNullException("element");
            List<Recommendation> recommendations = new List<Recommendation>();

            // Nach rechts auswerten
            PatternRunner runner = new PatternRunner(RootNode, Direction.Right, element);
            if (runner.EvaluateTree() && runner.HasRecommendation) recommendations.Add(runner.RecommendationCandiate.SetRank(Ranking));
            if (!IsSymmetric && runner.EvaluateTree(true) && runner.HasRecommendation) recommendations.Add(runner.RecommendationCandiate.SetRank(Ranking));

            // Nach links auswerten
            runner = new PatternRunner(RootNode, Direction.Left, element);
            if (runner.EvaluateTree() && runner.HasRecommendation) recommendations.Add(runner.RecommendationCandiate.SetRank(Ranking));
            if (!IsSymmetric && runner.EvaluateTree(true) && runner.HasRecommendation) recommendations.Add(runner.RecommendationCandiate.SetRank(Ranking));

            // Nach oben auswerten
            runner = new PatternRunner(RootNode, Direction.Up, element);
            if (runner.EvaluateTree() && runner.HasRecommendation) recommendations.Add(runner.RecommendationCandiate.SetRank(Ranking));
            if (!IsSymmetric && runner.EvaluateTree(true) && runner.HasRecommendation) recommendations.Add(runner.RecommendationCandiate.SetRank(Ranking));

            // Nach unten auswerten
            runner = new PatternRunner(RootNode, Direction.Down, element);
            if (runner.EvaluateTree() && runner.HasRecommendation) recommendations.Add(runner.RecommendationCandiate.SetRank(Ranking));
            if (!IsSymmetric && runner.EvaluateTree(true) && runner.HasRecommendation) recommendations.Add(runner.RecommendationCandiate.SetRank(Ranking));

            // Und raus
            return recommendations;
        }

        /// <summary>
        /// Konvertiert einen <see cref="PatternNode"/> in ein <see cref="Pattern"/>
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static explicit operator Pattern(PatternNode node)
        {
            return new Pattern(node, -1);
        }
    }
}
