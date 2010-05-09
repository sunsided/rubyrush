// ID $Id$

using System;
using RubyElement;

namespace RubyLogic.Pattern
{
    /// <summary>
    /// Das einfachste Pattern.
    /// Drei Elemente, die durch ein Fremdes Element getrennt werden.
    /// </summary>
    public sealed class SimpleXOXXPattern : PatternBase
    {
        /// <summary>
        /// Ermittelt, ob das Element auf das Pattern passt
        /// </summary>
        /// <param name="element">Das Element</param>
        /// <returns></returns>
        public override Recommendation Evaluate(Element element)
        {
            if(element.SpaceLeft >= 3)
            {
                // Wenn der linke Nachbar identisch ist (XX|)
                if(element.LeftNeighbour.Equals(element))
                {
                    // Wenn das dritte Element linkerhand identisch ist (XOXX|)
                    if(element.LeftNeighbour.LeftNeighbour.LeftNeighbour.Equals(element))
                    {
                        // den linken nach rechts schieben
                        return new Recommendation(element.LeftNeighbour.LeftNeighbour.LeftNeighbour, MoveDirection.Right);
                    }
                }
                else
                {
                    // Wenn der Nachbar des linken Elementes identisch ist (XOX|)
                    if(element.LeftNeighbour.LeftNeighbour.Equals(element))
                    {
                        if(element.LeftNeighbour.LeftNeighbour.LeftNeighbour.Equals(element))
                    }
                }
            }
        }

        /// <summary>
        /// Überprüft, ob eine XOXX|-Kombination vorliegt
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        private bool TestXOXX_Left(Element element)
        {
            return element.LeftNeighbour.Equals(element) && // XX|
                   element.LeftNeighbour.LeftNeighbour.LeftNeighbour.Equals(element); // X?XX|
        }

        /// <summary>
        /// Überprüft, ob eine XXOX|-Kombination vorliegt
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        private bool TestXXOX_Left(Element element)
        {
            return !element.LeftNeighbour.Equals(element) && // ?X|
                   element.LeftNeighbour.LeftNeighbour.Equals(element) && // X?X|
                   element.LeftNeighbour.LeftNeighbour.LeftNeighbour.Equals(element); // XX?X|
        }

        /// <summary>
        /// Das Ranking des Patterns. Je höher, desto gut.
        /// </summary>
        public override int Ranking
        {
            get { return 1; }
        }
    }
}
