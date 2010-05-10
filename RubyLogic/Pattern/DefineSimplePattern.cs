// ID $Id$

using RubyLogic.PatternTree;

namespace RubyLogic.Pattern
{
    /// <summary>
    /// Das einfachste Pattern.
    /// Drei Elemente, die durch ein Fremdes Element getrennt werden.
    /// </summary>
    public static class DefineSimplePattern
    {
        /// <summary>
        /// Erzeugt ein XXOX-Muster
        /// </summary>
        /// <returns></returns>
        [PatternDefinition]
        public static PatternNode CreateXXOXPattern()
        {
            return new PatternNode((runner, color, element) => element.Equals(color))       // Erstes Element stimmt
                .AppendSimpleNode((runner, color, element) => element.Equals(color))         // Zweites Element stimmt
                .AppendSimpleNode((runner, color, element) => !element.Equals(color))        // Drittes Element stimmt nicht
                .AppendSimpleNode((runner, color, element) => element.Equals(color));        // Viertes Element stimmt.
        }

        /// <summary>
        /// Erzeugt ein XOXX-Muster
        /// </summary>
        /// <returns></returns>
        [PatternDefinition]
        public static PatternNode CreateXOXXPattern()
        {
            return new PatternNode((runner, color, element) => element.Equals(color))       // Erstes Element stimmt
                .AppendSimpleNode((runner, color, element) => !element.Equals(color))        // Zweites Element stimmt nicht
                .AppendSimpleNode((runner, color, element) => element.Equals(color))         // Drittes Element stimmt
                .AppendSimpleNode((runner, color, element) => element.Equals(color));        // Viertes Element stimmt.
        }
    }
}
