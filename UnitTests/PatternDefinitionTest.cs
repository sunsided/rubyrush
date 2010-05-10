// ID $Id$

using System.Collections.Generic;
using System.Diagnostics;
using NUnit.Framework;
using RubyLogic;
using RubyLogic.Pattern;
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
            Trace.WriteLine(nodes.Count + " Pattern-Definitionen gefunden.");
        }

        /// <summary>
        /// Testet das Erstellen einer (beliebigen) Patterndefinition
        /// </summary>
        [Test]
        public void TestPatternDefinitionCreation()
        {
            PatternNode node = DefineSimplePattern.CreateXXOXPattern();
            Assert.NotNull(node);
            Assert.IsTrue(node.IsFirstNode);
            Assert.IsFalse(node.IsLastNode);
        }
    }
}
