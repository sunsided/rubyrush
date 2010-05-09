// ID $Id$

using System.Diagnostics.Contracts;

namespace RubyLogic.PatternTree
{
    /// <summary>
    /// Ein Knoten für ein Muster
    /// </summary>
    public sealed class PatternNode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PatternNode"/> class.
        /// </summary>
        /// <param name="testFunction">The test function.</param>
        public PatternNode(PatternTestDelegate testFunction = null)
        {
            TestFunction = testFunction;
        }

        /// <summary>
        /// Die Testfunktion
        /// </summary>
        public PatternTestDelegate TestFunction { get; set; }

        /// <summary>
        /// Der nächste Knoten
        /// </summary>
        public PatternNode NextNode { get; set; }

        /// <summary>
        /// Gibt an, ob dieses der letzte Knoten ist
        /// </summary>
        public bool IsLastNode
        {
            [Pure]
            get
            {
                return NextNode == null;
            }
        }

        /// <summary>
        /// Ermittelt die Länge der Musterkette
        /// </summary>
        public int ChainLength
        {
            [Pure]
            get
            {
                if (NextNode == null) return 1;
                return 1 + NextNode.ChainLength;
            }
        }

        /// <summary>
        /// Erzeugt einen Kindknoten
        /// </summary>
        /// <param name="testFunction">Die zu verwendende Testfunktion</param>
        /// <returns></returns>
        public PatternNode CreateChildNode(PatternTestDelegate testFunction = null)
        {
            PatternNode node = new PatternNode {TestFunction = testFunction};
            NextNode = node;
            return node;
        }
    }
}
