// ID $Id$

using NUnit.Framework;
using RubyElement;
using RubyLogic;

namespace UnitTests
{
    /// <summary>
    /// Tests, die Richtungen betreffen
    /// </summary>
    [TestFixture]
    public sealed class DirectionTests
    {
        /// <summary>
        /// Testet die <see cref="DirectionExtension.GetOpposite"/>-Funktion
        /// </summary>
        [Test]
        public void TestOpposite()
        {
            Assert.AreEqual(Direction.Left, Direction.Right.GetOpposite());
            Assert.AreEqual(Direction.Right, Direction.Left.GetOpposite());
            Assert.AreEqual(Direction.Up, Direction.Down.GetOpposite());
            Assert.AreEqual(Direction.Down, Direction.Up.GetOpposite());
        }
    }
}
