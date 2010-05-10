// ID $Id$

using System.Drawing;
using NUnit.Framework;
using RubyElement;
using RubyLogic;

namespace UnitTests
{
    /// <summary>
    /// Unit Tests für <see cref="ElementTests"/>
    /// </summary>
    [TestFixture]
    public class ElementTests
    {
        /// <summary>
        /// Testet, ob zwei Elemente identisch sind
        /// </summary>
        [Test]
        public void TestEquality()
        {
            Checkerboard board = new Checkerboard(8, 8);
            Element a = new KnownElement(board, 0, 0, Color.Empty, StoneColor.Blue);
            Element b = new KnownElement(board, 0, 0, Color.Empty, StoneColor.Blue);

            Assert.IsTrue(a.Equals(b));
        }

        /// <summary>
        /// Testet, ob zwei Elemente identisch bzw. nicht identisch sind
        /// </summary>
        [Test]
        public void TestEquality2()
        {
            Checkerboard board = new Checkerboard(8, 8);
            board[0, 0] = new KnownElement(board, 0, 0, Color.Empty, StoneColor.Blue);
            board[0, 1] = new KnownElement(board, 0, 1, Color.Empty, StoneColor.Blue);

            Assert.IsTrue(board[0, 0].Equals(board[0, 1].Color));
            Assert.IsFalse(board[0, 0].Equals(board[0, 1]));
        }
    }
}
