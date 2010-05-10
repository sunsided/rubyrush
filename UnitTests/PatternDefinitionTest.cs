// ID $Id$

using System.Collections.Generic;
using NUnit.Framework;
using RubyLogic;
using RubyLogic.PatternTree;

namespace UnitTests
{
    /// <summary>
    /// Test der Pattern-Definitionen
    /// </summary>
    [TestFixture]
    public sealed class PatternDefinitionTest
    {
        /// <summary>
        /// Testet, ob die Pattern-Definitionen korrekt ermittelt werden
        /// </summary>
        [Test]
        public void TestPatternNodeExistence()
        {
            IList<PatternNode> nodes = PatternDefinitionAttribute.GetPatternDefinitions();
            Assert.IsNotNull(nodes);
            Assert.Greater(nodes.Count, 0);
        }
    }
}
