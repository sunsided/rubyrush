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

        #region Nachbarn

        /// <summary>
        /// Liefert den Nachbarn in einer bestimmten Richtung
        /// </summary>
        /// <param name="direction"></param>
        /// <returns></returns>
        public Element GetNeighbourInDirection(Direction direction)
        {
            switch (direction)
            {
                case Direction.Up:
                    return TopNeighbour;
                case Direction.Down:
                    return BottomNeighbour;
                case Direction.Left:
                    return LeftNeighbour;
                case Direction.Right:
                    return RightNeighbour;
                default:
                    throw new ArgumentException("direction");
            }
        }

        /// <summary>
        /// Liefert die Anzahl Steine in der gegebenen Richtung
        /// </summary>
        /// <param name="direction">Die Richtung</param>
        /// <returns></returns>
        public int GetSpaceInDirection(Direction direction)
        {
            switch (direction)
            {
                case Direction.Up:
                    return SpaceTop;
                case Direction.Down:
                    return SpaceBottom;
                case Direction.Left:
                    return SpaceLeft;
                case Direction.Right:
                    return SpaceRight;
                default:
                    throw new ArgumentException("direction");
            }
        }

        /// <summary>
        /// Ermittelt die Anzahl der Elemente zur Rechten
        /// </summary>
        public int SpaceRight
        {
            [Pure]
            get { return Parent.ElementCountX - 1 - ParentXIndex; }
        }

        /// <summary>
        /// Liefert den rechten Nachbarn
        /// </summary>
        public Element RightNeighbour
        {
            [Pure]
            get
            {
                if (ParentXIndex == Parent.ElementCountX - 1) return null;
                return Parent[ParentXIndex + 1, ParentYIndex];
            }
        }

        /// <summary>
        /// Ermittelt die Anzahl der Elemente zur Linken
        /// </summary>
        public int SpaceLeft
        {
            [Pure]
            get { return ParentXIndex; }
        }

        /// <summary>
        /// Liefert den linken Nachbarn
        /// </summary>
        public Element LeftNeighbour
        {
            [Pure]
            get
            {
                if (ParentXIndex == 0) return null;
                return Parent[ParentXIndex - 1, ParentYIndex];
            }
        }

        /// <summary>
        /// Ermittelt die Anzahl der Elemente nach oben
        /// </summary>
        public int SpaceTop
        {
            [Pure]
            get { return ParentYIndex; }
        }

        /// <summary>
        /// Liefert den oberen Nachbarn
        /// </summary>
        public Element TopNeighbour
        {
            [Pure]
            get
            {
                if (ParentYIndex == 0) return null;
                return Parent[ParentXIndex, ParentYIndex - 1];
            }
        }

        /// <summary>
        /// Ermittelt die Anzahl der Elemente nach unten
        /// </summary>
        public int SpaceBottom
        {
            [Pure]
            get { return Parent.ElementCountY - 1 - ParentYIndex; }
        }

        /// <summary>
        /// Liefert den unteren Nachbarn
        /// </summary>
        public Element BottomNeighbour
        {
            [Pure]
            get
            {
                if (ParentYIndex == Parent.ElementCountY - 1) return null;
                return Parent[ParentXIndex, ParentYIndex + 1];
            }
        }

        #endregion

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
            return ParentXIndex == other.ParentXIndex && ParentYIndex == other.ParentYIndex && Color.Equals(other.Color);
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

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format("[{0}, {1}] {2}", ParentXIndex, ParentYIndex, Color);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return Color.GetHashCode() ^ ParentXIndex.GetHashCode() ^ ParentYIndex.GetHashCode();
        }
    }
}
