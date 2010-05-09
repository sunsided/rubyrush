// ID $Id$

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
        /// <param name="rawColor">Die ursprüngliche Farbe.</param>
        /// <param name="color">The color.</param>
        public KnownElement(Color rawColor, StoneColor color)
            : base(rawColor, color)
        {
        }

        /// <summary>
        /// Gibt an, ob dieses Element erkannt wurde
        /// </summary>
        /// <value></value>
        public override bool IsRecognized
        {
            get { return true; }
        }
    }
}
