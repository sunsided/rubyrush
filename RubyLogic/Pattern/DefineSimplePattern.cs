// ID $Id$

using RubyLogic.PatternTree;

namespace RubyLogic.Pattern
{
    /// <summary>
    /// Das einfachste Pattern.
    /// Drei Elemente, die durch ein Fremdes Element getrennt werden.
    /// </summary>
    [PatternDefinition]
    public static class DefineSimplePattern
    {
        /// <summary>
        /// Erzeugt ein XXOX-Muster
        /// </summary>
        /// <returns></returns>
        [PatternDefinition]
        public static PatternNode CreateXXOXPattern()
        {
            return new PatternNode(PatternTest.Equality)                // Erstes Element stimmt
                .AppendSimpleNode(PatternTest.Equality)                 // Zweites Element stimmt
                .AppendSimpleNode(PatternTest.Inequality)               // Drittes Element stimmt nicht
                .AppendSimpleNode(PatternTest.EqualityAndCandidate)     // Viertes Element stimmt.
                .EndChain();
        }
    }
}
