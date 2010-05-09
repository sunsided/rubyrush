// ID $Id$

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
        public UnknownElement(Color rawColor) : base(rawColor, StoneColor.Unknown)
        {
        }

        /// <summary>
        /// Gibt an, ob dieses Element erkannt wurde
        /// </summary>
        /// <value></value>
        public override bool IsRecognized
        {
            get { return false; }
        }
    }
}
