// ID $Id$

using System;
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
        public PatternNode(PatternTest.TestDelegate testFunction = null)
        {
            TestFunction = testFunction;
        }

        /// <summary>
        /// Die Testfunktion
        /// </summary>
        public PatternTest.TestDelegate TestFunction { [Pure] get; set; }

        /// <summary>
        /// Der nächste Knoten
        /// </summary>
        public PatternNode NextNode { [Pure] get; private set; }

        /// <summary>
        /// Schwache Referenz auf den Vorgängerknoten
        /// </summary>
        private WeakReference _prevNode;

        /// <summary>
        /// Der Vorgängerknoten
        /// </summary>
        public PatternNode PrevNode
        {
            [Pure]
            get
            {
                return _prevNode != null ? _prevNode.Target as PatternNode : null;
            }
            private set
            {
                _prevNode = new WeakReference(value);
            }
        }

        /// <summary>
        /// Schwache Referenz auf den Vorgängerknoten
        /// </summary>
        private WeakReference _perpendicularPrevNode;

        /// <summary>
        /// Der Vorgängerknoten in waagerechter Richtung
        /// </summary>
        public PatternNode PrevNodePerpendicular
        {
            [Pure]
            get
            {
                return _perpendicularPrevNode != null ? _perpendicularPrevNode.Target as PatternNode : null;
            }
            private set
            {
                _perpendicularPrevNode = new WeakReference(value);
            }
        }

        /// <summary>
        /// Der nächste Knoten in waagerechter Richtung (auf die aktuelle Achse)
        /// </summary>
        public PatternNode PerpendicularNode { get; private set; }

        /// <summary>
        /// Der nächste Knoten in waagerechter Richtung (auf die aktuelle Achse)
        /// </summary>
        public PatternNode PerpendicularNode2 { get; private set; }

        /// <summary>
        /// Ermittelt, ob dieser Knoten einen Folgeknoten in waagerechter Richtung hat
        /// </summary>
        public bool HasPerpendicularNode
        {
            [Pure]
            get
            {
                return PerpendicularNode != null;
            }
        }

        /// <summary>
        /// Ermittelt, ob dieser Knoten einen Folgeknoten in waagerechter Richtung hat
        /// </summary>
        public bool HasSecondaryPerpendicularNode
        {
            [Pure]
            get
            {
                return PerpendicularNode2 != null;
            }
        }

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
        /// Gibt an, ob dieses der erste Knoten ist
        /// </summary>
        public bool IsFirstNode
        {
            [Pure]
            get
            {
                return PrevNode == null;
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
        public PatternNode AppendSimpleNode(PatternTest.TestDelegate testFunction = null)
        {
            PatternNode node = new PatternNode {TestFunction = testFunction};
            NextNode = node;
            node.PrevNode = this;
            return node;
        }

        /// <summary>
        /// Erzeugt einen Kindknoten
        /// </summary>
        /// <param name="node">Der zu verwendende Knoten</param>
        /// <returns></returns>
        public PatternNode AppendSimpleNode(PatternNode node)
        {
            NextNode = node;
            node.PrevNode = this;
            return node;
        }

        /// <summary>
        /// Erzeugt einen Kindknoten in waagerechter Richtung auf die aktuelle Achse
        /// </summary>
        /// <param name="testFunction">Die zu verwendende Testfunktion</param>
        /// <returns></returns>
        public PatternNode AppendPerpendicularNode(PatternTest.TestDelegate testFunction = null)
        {
            PatternNode node = new PatternNode { TestFunction = testFunction };

            // Option für sekundäre ... Nebenknoten
            if (PerpendicularNode == null) PerpendicularNode = node;
            else PerpendicularNode2 = node;

            node.PrevNodePerpendicular = this;
            return node;
        }

        /// <summary>
        /// Erzeugt einen Kindknoten in waagerechter Richtung auf die aktuelle Achse
        /// </summary>
        /// <param name="node">Der zu verwendende Knoten</param>
        /// <returns></returns>
        public PatternNode AppendPerpendicularNode(PatternNode node)
        {
            if (PerpendicularNode == null) PerpendicularNode = node;
            else PerpendicularNode2 = node;

            node.PrevNodePerpendicular = this;
            return node;
        }

        /// <summary>
        /// Signalisiert das Ende einer Kette und liefert den Beginn der Kette zurück.
        /// </summary>
        /// <returns><see cref="PatternNode"/>, der den Anfang der Kette darstellt.</returns>
        public PatternNode EndChain()
        {
            PatternNode current = this;
            while (current.PrevNode != null) current = current.PrevNode;
            return current.PrevNodePerpendicular ?? current;
        }
    }
}
