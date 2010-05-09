// ID $Id$

using RubyElement;

namespace RubyLogic
{
    /// <summary>
    /// Das Patterm
    /// </summary>
    public abstract class PatternBase
    {
        /// <summary>
        /// Ermittelt, ob das Element auf das Pattern passt
        /// </summary>
        /// <param name="element">Das Element</param>
        /// <returns></returns>
        public abstract Recommendation Evaluate(Element element);

        /// <summary>
        /// Das Ranking des Patterns. Je höher, desto gut.
        /// </summary>
        public abstract int Ranking { get; }
    }
}
