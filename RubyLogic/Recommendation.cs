// ID $Id$ 

using System;
using RubyElement;

namespace RubyLogic
{
    /// <summary>
    /// Eine Empfehlung für einen Zug
    /// </summary>
    public sealed class Recommendation : IEquatable<Recommendation>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Recommendation"/> class.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="move">The move.</param>
        public Recommendation(Element element, Direction move)
        {
            Element = element;
            Move = move;
        }

        /// <summary>
        /// Das betroffene Element
        /// </summary>
        public Element Element { get; private set; }

        /// <summary>
        /// Die Bewegungsrichtung
        /// </summary>
        public Direction Move { get; private set; }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(Recommendation other)
        {
            if (other == null) throw new ArgumentNullException("other");

            // Identität testen
            bool selftest = Element.Equals(other.Element) && Move.Equals(other.Move);

            // Entgegengesetzten Zug testen
            bool countertest = false;
            switch(Move)
            {
                case Direction.Up:
                    countertest = (other.Move == Direction.Down) &&
                                  (other.Element.ParentYIndex == Element.ParentYIndex - 1);
                    break;
                case Direction.Down:
                    countertest = (other.Move == Direction.Up) &&
                                  (other.Element.ParentYIndex == Element.ParentYIndex + 1);
                    break;
                case Direction.Left:
                    countertest = (other.Move == Direction.Right) &&
                                  (other.Element.ParentXIndex == Element.ParentXIndex - 1);
                    break;
                case Direction.Right:
                    countertest = (other.Move == Direction.Left) &&
                                  (other.Element.ParentXIndex == Element.ParentXIndex + 1);
                    break;
            }

            // Eines von beiden sollte für eine Gleichheit zutreffen
            return selftest || countertest;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return Element.GetHashCode() ^ (Move.GetHashCode() * 37);
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return Element + " " + Move;
        }
    }
}
