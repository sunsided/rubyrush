// ID $Id$ 

using System;
using System.Diagnostics.Contracts;
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
        public Element Element { [Pure] get; private set; }

        /// <summary>
        /// Die Bewegungsrichtung
        /// </summary>
        public Direction Move { [Pure] get; private set; }

        /// <summary>
        /// Gibt an, ob der Vorschlag endgültig ist
        /// </summary>
        public bool IsFinal { [Pure] get; private set; }

        /// <summary>
        /// Macht einen Vorschlag endgültig
        /// </summary>
        public void MakeFinal()
        {
            IsFinal = true;
        }

        /// <summary>
        /// Kehrt die Bewegungsrichtung um, z.B. bei Revers-Auswertung eines Musters
        /// </summary>
        internal void RevertDirection()
        {
            Move = Move.GetOpposite();
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        [Pure]
        public bool Equals(Recommendation other)
        {
            if (other == null) throw new ArgumentNullException("other");

            // Identität testen
            return Element.Equals(other.Element) && Move.Equals(other.Move);
        }

        /// <summary>
        /// Gibt an, ob die andere Empfehlung denselben Zug darstellt
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool IsSameMove(Recommendation other)
        {
            bool countertest = false;
            switch (Move)
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
            return countertest;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        [Pure]
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
        [Pure]
        public override string ToString()
        {
            return Element + " " + Move;
        }
    }
}
