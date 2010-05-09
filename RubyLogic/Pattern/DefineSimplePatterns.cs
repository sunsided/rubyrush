// ID $Id$

using RubyLogic.PatternTree;

namespace RubyLogic.Pattern
{
    /// <summary>
    /// Das einfachste Pattern.
    /// Drei Elemente, die durch ein Fremdes Element getrennt werden.
    /// </summary>
    public static class DefineSimplePatterns
    {
        /// <summary>
        /// Erzeugt ein XXOX-Muster
        /// </summary>
        /// <returns></returns>
        public static PatternNode CreateXXOXPattern()
        {
            return new PatternNode((runner, color, element) => element.Equals(color))       // Erstes Element stimmt
                .CreateChildNode((runner, color, element) => element.Equals(color))         // Zweites Element stimmt
                .CreateChildNode((runner, color, element) => !element.Equals(color))        // Drittes Element stimmt nicht
                .CreateChildNode((runner, color, element) => element.Equals(color));        // Viertes Element stimmt.
        }

        /// <summary>
        /// Erzeugt ein XOXX-Muster
        /// </summary>
        /// <returns></returns>
        public static PatternNode CreateXOXXPattern()
        {
            return new PatternNode((runner, color, element) => element.Equals(color))       // Erstes Element stimmt
                .CreateChildNode((runner, color, element) => !element.Equals(color))        // Zweites Element stimmt nicht
                .CreateChildNode((runner, color, element) => element.Equals(color))         // Drittes Element stimmt
                .CreateChildNode((runner, color, element) => element.Equals(color));        // Viertes Element stimmt.
        }
    }
}
