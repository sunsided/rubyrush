// ID $Id$

using System;
using System.Diagnostics.Contracts;
using System.Drawing;

namespace RubyElement
{
    /// <summary>
    /// Klasse, die einen Stein beschreibt
    /// </summary>
    public abstract class Element : IEquatable<StoneColor>, IEquatable<Element>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Element"/> class.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="indexX">The X index.</param>
        /// <param name="indexY">The Y index.</param>
        /// <param name="rawColor">Die ursprüngliche Farbe</param>
        /// <param name="color">The color.</param>
        protected Element(Checkerboard parent, int indexX, int indexY, Color rawColor, StoneColor color)
        {
            Parent = parent;
            ParentXIndex = indexX;
            ParentYIndex = indexY;

            RawColor = rawColor;
            Color = color;
        }

        /// <summary>
        /// Der X-Index im Spielfeld
        /// </summary>
        public int ParentXIndex { get; private set; }

        /// <summary>
        /// Der Y-Index im Spielfeld
        /// </summary>
        public int ParentYIndex { get; private set; }

        /// <summary>
        /// Das Eltern-Grid
        /// </summary>
        public Checkerboard Parent { get; private set; }

        /// <summary>
        /// Gibt an, ob dieses Element erkannt wurde
        /// </summary>
        public abstract bool IsRecognized { get; }

        /// <summary>
        /// Die ursprüngliche Farbe
        /// </summary>
        public Color RawColor { [Pure] get; private set; }

        /// <summary>
        /// Die Farbe/Form des Steins
        /// </summary>
        public StoneColor Color { [Pure] get; private set; }

        /// <summary>
        /// Liefert die stereotype Farbe für dieses Element
        /// </summary>
        [Pure]
        public Color StereotypeColor
        {
            get
            {
                switch(Color)
                {
                    case StoneColor.White:
                        return System.Drawing.Color.White;
                    case StoneColor.Red:
                        return System.Drawing.Color.DarkRed;
                    case StoneColor.Green:
                        return System.Drawing.Color.Green;
                    case StoneColor.Blue:
                        return System.Drawing.Color.Blue;
                    case StoneColor.Violet:
                        return System.Drawing.Color.Violet;
                    case StoneColor.Orange:
                        return System.Drawing.Color.OrangeRed;
                    case StoneColor.Yellow:
                        return System.Drawing.Color.Yellow;
                    default:
                        return System.Drawing.Color.Black;
                }
            }
        }

        /// <summary>
        /// Equalses the specified other.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns></returns>
        [Pure]
        public bool Equals(StoneColor other)
        {
            return Color.Equals(other);
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        [Pure]
        public bool Equals(Element other)
        {
            if (other == null) return false;
            return Color.Equals(other.Color);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="RubyElement.Element"/> to <see cref="System.Drawing.Color"/>.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>The result of the conversion.</returns>
        [Pure]
        public static implicit operator Color(Element element)
        {
            return element.RawColor;
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="RubyElement.Element"/> to <see cref="StoneColor"/>.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>The result of the conversion.</returns>
        [Pure]
        public static implicit operator StoneColor(Element element)
        {
            return element.Color;
        }
    }
}
