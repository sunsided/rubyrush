// ID $Id$

using System.Collections.Generic;
using System.Diagnostics;
using NUnit.Framework;
using RubyLogic;
using RubyLogic.PatternDefinitions;
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
            IList<Pattern> nodes = PatternDefinitionAttribute.GetPatternDefinitions();
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

        /// <summary>
        /// Testet die EndChain-Markierung
        /// </summary>
        [Test]
        public void TestEndChainMarker()
        {
            /*
            PatternNode example = new PatternNode(PatternTest.Equality)
                .AppendSimpleNode(PatternTest.Inequality)
                    .AppendPerpendicularNode(PatternTest.EqualityAndCandidate)
                    .EndChain()
                .AppendSimpleNode(PatternTest.Equality)
                .EndChain();
            */

            PatternNode root = new PatternNode(PatternTest.Equality);
            PatternNode child1 = new PatternNode(PatternTest.Inequality);
            PatternNode side1 = new PatternNode(PatternTest.EqualityAndCandidate);
            PatternNode child2 = new PatternNode(PatternTest.Equality);

            root.AppendSimpleNode(child1);
            child1.AppendPerpendicularNode(side1);
            child1.AppendSimpleNode(child2);

            Assert.IsTrue(root.IsFirstNode, "root is first");
            Assert.IsFalse(root.IsLastNode, "root is not last");
            Assert.IsFalse(child1.IsFirstNode, "child1 is not first");
            Assert.IsFalse(child1.IsLastNode, "child1 is not last");

            Assert.IsTrue(side1.IsFirstNode, "side1 is first");
            Assert.IsTrue(side1.IsLastNode, "side1 is last");

            Assert.IsFalse(root.HasPerpendicularNode, "root has perpendicular node");
            Assert.IsTrue(child1.HasPerpendicularNode, "child1 has perpendicular node");
            Assert.IsFalse(child2.HasPerpendicularNode, "child2 has perpendicular node");
            Assert.IsFalse(side1.HasPerpendicularNode, "side1 has perpendicular node");

            Assert.AreSame(root, child2.EndChain(), "end of chain (child2) is root");
            Assert.AreSame(root, child1.EndChain(), "end of chain (child1) is root");
            Assert.AreSame(root, root.EndChain(), "end of chain (root) is root");

            Assert.AreSame(child1, side1.EndChain(), "end of chain (side1) is child1");
            Assert.AreSame(root, side1.EndChain().EndChain(), "end of chain² (side1) is child1");
        }

        /// <summary>
        /// Testet das Anhängen von Knoten in Nebenrichtungen
        /// </summary>
        [Test]
        public void TestPerpendicularNode()
        {
            PatternNode root = new PatternNode(PatternTest.Equality);
            PatternNode side1 = new PatternNode(PatternTest.Equality);
            PatternNode side2 = new PatternNode(PatternTest.Equality);

            Assert.IsFalse(root.HasPerpendicularNode);
            Assert.IsFalse(root.HasSecondaryPerpendicularNode);

            root.AppendPerpendicularNode(side1);
            Assert.IsTrue(root.HasPerpendicularNode);
            Assert.IsFalse(root.HasSecondaryPerpendicularNode);

            root.AppendPerpendicularNode(side2);
            Assert.IsTrue(root.HasPerpendicularNode);
            Assert.IsTrue(root.HasSecondaryPerpendicularNode);

            Assert.AreSame(root.PerpendicularNode, side1);
            Assert.AreSame(root.PerpendicularNode2, side2);
        }
    }
}
