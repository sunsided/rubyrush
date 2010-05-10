// ID $Id$

using System;

namespace RubyLogic
{
    /// <summary>
    /// Bewegungsrichtung
    /// </summary>
    public enum Direction
    {
        /// <summary>
        /// Hoch
        /// </summary>
        Up,

        /// <summary>
        /// Runter
        /// </summary>
        Left,

        /// <summary>
        /// Links
        /// </summary>
        Right,

        /// <summary>
        /// Rechts
        /// </summary>
        Down
    }

    /// <summary>
    /// Extension Methods für Directions
    /// </summary>
    public static class DirectionExtension
    {
        /// <summary>
        /// Ermittelt die entgegengesetzte Richtung
        /// </summary>
        /// <param name="direction">Die Richtung</param>
        /// <returns>Die entgegengesetzte Richtung</returns>
        public static Direction GetOpposite(this Direction direction)
        {
            switch (direction)
            {
                case Direction.Up:
                    return Direction.Down;
                case Direction.Down:
                    return Direction.Up;
                case Direction.Left:
                    return Direction.Right;
                case Direction.Right:
                    return Direction.Left;
                default:
                    throw new ArgumentException("direction");
            }
        }
    }
}
