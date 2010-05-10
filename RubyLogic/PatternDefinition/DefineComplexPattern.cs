// ID $Id$

using RubyLogic.PatternTree;

namespace RubyLogic.PatternDefinition
{
    /// <summary>
    /// Das einfachste Pattern.
    /// Drei Elemente, die durch ein Fremdes Element getrennt werden.
    /// </summary>
    [PatternProvider]
    public static class DefineComplexPattern
    {
        /// <summary>
        /// Erzeugt ein XXOX2-Muster
        /// 
        /// <code>
        /// - - X -
        /// X X - X
        /// </code>
        /// 
        /// </summary>
        /// <returns></returns>
        [PatternDefinition(4, "XXOX2")]
        public static PatternNode CreateXXOX2Pattern()
        {
            return new PatternNode(PatternTest.Equality)
                .AppendSimpleNode(PatternTest.Equality)
                .AppendSimpleNode(PatternTest.Inequality)
                    .AppendPerpendicularNode(PatternTest.EqualityAndCandidate)
                    .EndChain()
                .AppendSimpleNode(PatternTest.Equality)
                .EndChain();
        }

        /// <summary>
        /// Erzeugt ein XXOXX-Muster
        /// 
        /// <code>
        /// - - X - -
        /// X X - X X
        /// </code>
        /// 
        /// </summary>
        /// <returns></returns>
        [PatternDefinition(7, "XXOXX-Bomb")]
        public static PatternNode CreateXXOXXPattern()
        {
            return new PatternNode(PatternTest.Equality)
                .AppendSimpleNode(PatternTest.Equality)
                .AppendSimpleNode(PatternTest.Inequality)
                    .AppendPerpendicularNode(PatternTest.EqualityAndCandidate)
                    .EndChain()
                .AppendSimpleNode(PatternTest.Equality)
                .AppendSimpleNode(PatternTest.Equality)
                .EndChain();
        }

        /// <summary>
        /// Erzeugt ein XXOXX-Muster
        /// 
        /// <code>
        /// - - X -
        /// - - X -
        /// X X - X
        /// </code>
        /// 
        /// </summary>
        /// <returns></returns>
        [PatternDefinition(6, "XXOX-Cross")]
        public static PatternNode CreateXXOXXCrossPattern()
        {
            return new PatternNode(PatternTest.Equality)
                .AppendSimpleNode(PatternTest.Equality)
                .AppendSimpleNode(PatternTest.Inequality)
                    .AppendPerpendicularNode(PatternTest.Equality)
                    .AppendSimpleNode(PatternTest.Equality)
                    .EndChain()
                .AppendSimpleNode(PatternTest.EqualityAndCandidate)
                .EndChain();
        }

        /// <summary>
        /// Erzeugt ein XXOXX-Muster
        /// 
        /// <code>
        /// - - X -
        /// - - X -
        /// X X - X
        /// - - X -
        /// - - X -
        /// </code>
        /// 
        /// </summary>
        /// <returns></returns>
        [PatternDefinition(100, "XXOXX-MegaCross")]
        public static PatternNode CreateXXOXXMegaCrossPattern()
        {
            return new PatternNode(PatternTest.Equality)
                .AppendSimpleNode(PatternTest.Equality)
                .AppendSimpleNode(PatternTest.Inequality)
                    .AppendPerpendicularNode(PatternTest.Equality)
                    .AppendSimpleNode(PatternTest.Equality)
                    .EndChain()
                    .AppendPerpendicularNode(PatternTest.Equality)
                    .AppendSimpleNode(PatternTest.Equality)
                    .EndChain()
                 .AppendSimpleNode(PatternTest.EqualityAndCandidate)
                .EndChain();
        }
    }
}
