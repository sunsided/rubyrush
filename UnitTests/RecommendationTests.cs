// ID $Id$

using System.Drawing;
using NUnit.Framework;
using RubyElement;
using RubyLogic;

namespace UnitTests
{
    /// <summary>
    /// Unit Tests für <see cref="Recommendation"/>
    /// </summary>
    [TestFixture]
    public class RecommendationTests
    {
        /// <summary>
        /// Testet, ob zwei Recommendations identisch sind
        /// </summary>
        [Test]
        public void TestSimpleEquality()
        {
            Checkerboard board = new Checkerboard(8, 8);
            board[0, 0] = new KnownElement(board, 0, 0, Color.Empty, StoneColor.Blue);
            board[0, 1] = new KnownElement(board, 0, 1, Color.Empty, StoneColor.Yellow);

            Recommendation recA = new Recommendation(board[0, 0], Direction.Down);
            Recommendation recB = new Recommendation(board[0, 0], Direction.Down);

            Assert.IsTrue(recA.Equals(recB), "Einfache Gleichheit fehlgeschlagen");
            Assert.IsTrue(recB.Equals(recA), "Einfache Gleichheit fehlgeschlagen");
        }

        /// <summary>
        /// Testet, ob zwei Recommendations identisch sind
        /// </summary>
        [Test]
        public void TestEqualityUpDown()
        {
            Checkerboard board = new Checkerboard(8, 8);
            board[0, 0] = new KnownElement(board, 0, 0, Color.Empty, StoneColor.Blue);
            board[0, 1] = new KnownElement(board, 0, 1, Color.Empty, StoneColor.Yellow);
            board[1, 0] = new KnownElement(board, 1, 0, Color.Empty, StoneColor.Yellow);

            Recommendation recA = new Recommendation(board[0, 0], Direction.Down);
            Recommendation recB = new Recommendation(board[0, 1], Direction.Up);
            Recommendation recC = new Recommendation(board[1, 0], Direction.Left);

            Assert.IsTrue(recA.IsSameMove(recB), "Gleichheit up/down fehlgeschlagen");
            Assert.IsTrue(recB.IsSameMove(recA), "Gleichheit up/down fehlgeschlagen");
            Assert.IsFalse(recA.IsSameMove(recC), "Gleichheit up/down fehlgeschlagen");
            Assert.IsFalse(recC.IsSameMove(recA), "Gleichheit up/down fehlgeschlagen");
        }

        /// <summary>
        /// Testet, ob zwei Recommendations identisch sind
        /// </summary>
        [Test]
        public void TestEqualityLeftRight()
        {
            Checkerboard board = new Checkerboard(8, 8);
            board[0, 0] = new KnownElement(board, 0, 0, Color.Empty, StoneColor.Blue);
            board[1, 0] = new KnownElement(board, 1, 0, Color.Empty, StoneColor.Yellow);
            board[0, 1] = new KnownElement(board, 0, 1, Color.Empty, StoneColor.Yellow);

            Recommendation recA = new Recommendation(board[0, 0], Direction.Right);
            Recommendation recB = new Recommendation(board[1, 0], Direction.Left);
            Recommendation recC = new Recommendation(board[0, 1], Direction.Up);

            Assert.IsTrue(recA.IsSameMove(recB), "Gleichheit left/right fehlgeschlagen");
            Assert.IsTrue(recB.IsSameMove(recA), "Gleichheit left/right fehlgeschlagen");
            Assert.IsFalse(recA.IsSameMove(recC), "Gleichheit left/right fehlgeschlagen");
            Assert.IsFalse(recC.IsSameMove(recA), "Gleichheit left/right fehlgeschlagen");
        }
    }
}
