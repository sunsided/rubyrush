// ID $Id$

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RubyElement;

namespace RubyLogic
{
    /// <summary>
    /// Klasse für Spiellogik
    /// </summary>
    public static class GameLogic
    {
        /// <summary>
        /// Ermittelt, ob mit einem Element potentiell Punkte gemacht werden können
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public static bool IsCandidate(Element element)
        {
            // Ranking 1
            // X O X X

            // Ranking 1
            // X X O X

            // Ranking 1
            // O X O
            // X O X

            // Ranking 1
            // X O X
            // O X O

            // Ranking 1
            // X X O X X

            // Ranking 2
            // O O X
            // X X O
            // O O X
            // O O X

            // Ranking 2
            // X X O X
            // O O X O
            // O O X O

            // Ranking 3
            // X X O X X
            // O O X O O

            // Ranking 3
            // O O X O O
            // X X O X X
        }
    }
}
