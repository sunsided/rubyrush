// ID $Id$

using System.Diagnostics.Contracts;
using System.Drawing;

namespace RubyElement
{
    /// <summary>
    /// Klasse, die einen Stein beschreibt
    /// </summary>
    public sealed class UnknownElement : Element
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnknownElement"/> class.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="indexX">The index X.</param>
        /// <param name="indexY">The index Y.</param>
        /// <param name="rawColor">Color of the raw.</param>
        public UnknownElement(Checkerboard parent, int indexX, int indexY, Color rawColor)
            : base(parent, indexX, indexY, rawColor, StoneColor.Unknown)
        {
        }

        /// <summary>
        /// Gibt an, ob dieses Element erkannt wurde
        /// </summary>
        /// <value></value>
        public override bool IsRecognized
        {
            [Pure]
            get { return false; }
        }
    }
}
