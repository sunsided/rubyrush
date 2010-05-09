// ID $Id$

using System.Diagnostics.Contracts;
using System.Drawing;

namespace RubyElement
{
    /// <summary>
    /// Klasse, die einen Stein beschreibt
    /// </summary>
    public sealed class KnownElement : Element
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="KnownElement"/> class.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="indexX">The X index.</param>
        /// <param name="indexY">The Y index.</param>
        /// <param name="rawColor">Die ursprüngliche Farbe.</param>
        /// <param name="color">The color.</param>
        public KnownElement(Checkerboard parent, int indexX, int indexY, Color rawColor, StoneColor color)
            : base(parent, indexX, indexY, rawColor, color)
        {
        }

        /// <summary>
        /// Gibt an, ob dieses Element erkannt wurde
        /// </summary>
        /// <value></value>
        public override bool IsRecognized
        {
            [Pure]
            get { return true; }
        }
    }
}
