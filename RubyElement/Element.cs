// ID $Id$

using System;
using System.Drawing;

namespace RubyElement
{
    /// <summary>
    /// Klasse, die einen Stein beschreibt
    /// </summary>
    public abstract class Element : IEquatable<StoneColor>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Element"/> class.
        /// </summary>
        /// <param name="color">The color.</param>
        protected Element(StoneColor color)
            : this(System.Drawing.Color.Empty, color)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Element"/> class.
        /// </summary>
        /// <param name="rawColor">Die ursprüngliche Farbe</param>
        /// <param name="color">The color.</param>
        protected Element(Color rawColor, StoneColor color)
        {
            RawColor = rawColor;
            Color = color;
        }

        /// <summary>
        /// Gibt an, ob dieses Element erkannt wurde
        /// </summary>
        public abstract bool IsRecognized { get; }

        /// <summary>
        /// Die ursprüngliche Farbe
        /// </summary>
        public Color RawColor { get; private set; }

        /// <summary>
        /// Die Farbe/Form des Steins
        /// </summary>
        public StoneColor Color { get; private set; }

        /// <summary>
        /// Equalses the specified other.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns></returns>
        public bool Equals(StoneColor other)
        {
            return Color.Equals(other);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="RubyElement.Element"/> to <see cref="System.Drawing.Color"/>.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator Color(Element element)
        {
            return element.RawColor;
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="RubyElement.Element"/> to <see cref="StoneColor"/>.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator StoneColor(Element element)
        {
            return element.Color;
        }
    }
}
