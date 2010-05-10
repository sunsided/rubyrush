// ID $Id$

using System;

namespace RubyLogic.PatternTree
{
    /// <summary>
    /// Gibt die Auswertungsrichtung an
    /// </summary>
    [Flags]
    public enum EvaluationDirection
    {
        /// <summary>
        /// Linkerhand
        /// </summary>
        Left = 1,

        /// <summary>
        /// Rechterhand
        /// </summary>
        Right = 2,
    }
}
