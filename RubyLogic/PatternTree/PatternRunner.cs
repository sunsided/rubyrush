// ID $Id$

using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using RubyElement;

namespace RubyLogic.PatternTree
{
    /// <summary>
    /// Interpreter für Pattern-Nodes
    /// </summary>
    public sealed class PatternRunner
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PatternRunner"/> class.
        /// </summary>
        /// <param name="startNode">The start node.</param>
        /// <param name="moveDirection">The move direction.</param>
        /// <param name="startElement">The start element.</param>
        public PatternRunner(PatternNode startNode, Direction moveDirection, Element startElement)
        {
            StartElement = startElement;
            StartNode = startNode;
            MoveDirection = moveDirection;
        }

        /// <summary>
        /// Das Startelement
        /// </summary>
        public Element StartElement { [Pure] get; private set; }

        /// <summary>
        /// Die Bewegungsrichtung
        /// </summary>
        public Direction MoveDirection { [Pure] get; private set; }

        /// <summary>
        /// Der Startknoten
        /// </summary>
        public PatternNode StartNode { [Pure] get; private set; }

        /// <summary>
        /// Kandidat für einen Zug-Vorschlag, falls dieser Teilbaum erfolgreich ausgewertet wird
        /// </summary>
        public Recommendation RecommendationCandiate { [Pure] get; private set; }

        /// <summary>
        /// Liefert das nächste Element
        /// </summary>
        /// <param name="current">Das aktuelle Element</param>
        /// <returns>Das nächste Element in der vorgegebenen Richtung</returns>
        public Element GetNextElement(Element current)
        {
            switch(MoveDirection)
            {
                case Direction.Up:
                    return current.TopNeighbour;
                case Direction.Down:
                    return current.BottomNeighbour;
                case Direction.Left:
                    return current.LeftNeighbour;
                case Direction.Right:
                    return current.RightNeighbour;
                default:
                    return null;
            }
        }
        
        /// <summary>
        /// Erzeugt <see cref="PatternRunner"/>, die rechtwinklig zur aktuellen Richtung arbeiten
        /// </summary>
        /// <param name="current">Das aktuelle Element</param>
        /// <param name="startNode">Der Knoten, mit dem die Auswertung beginnen soll</param>
        /// <returns>Liste von Runnern</returns>
        public PatternRunner[] GetPerpendicular(Element current, PatternNode startNode)
        {
            switch(MoveDirection)
            {
                case Direction.Up:
                case Direction.Down:
                    {
                        int count = (current.SpaceLeft > 0 ? 1 : 0) + (current.SpaceRight > 0 ? 1 : 0);
                        PatternRunner[] array = new PatternRunner[count];
                        int index = 0;
                        if (current.SpaceLeft > 0) array[index++] = new PatternRunner(startNode, Direction.Left, current.LeftNeighbour);
                        if (current.SpaceRight > 0) array[index] = new PatternRunner(startNode, Direction.Right, current.RightNeighbour);
                        return array;
                    }
                case Direction.Left:
                case Direction.Right:
                    {
                        int count = (current.SpaceTop > 0 ? 1 : 0) + (current.SpaceBottom > 0 ? 1 : 0);
                        PatternRunner[] array = new PatternRunner[count];
                        int index = 0;
                        if (current.SpaceTop > 0) array[index++] = new PatternRunner(startNode, Direction.Up, current.TopNeighbour);
                        if (current.SpaceBottom > 0) array[index] = new PatternRunner(startNode, Direction.Down, current.BottomNeighbour);
                        return array;
                    }
                default:
                    return new PatternRunner[0];
            }
        }

        /// <summary>
        /// Überprüft, ob der Baum zutrifft
        /// </summary>
        /// <param name="reverse">Gibt an, ob der Baum in umgekehrter Richtung aufgerufen werden soll</param>
        /// <returns><c>true</c>, wenn der Baum positiv ausgewertet wurde, ansonsten <c>false</c></returns>
        public bool EvaluateTree(bool reverse = false)
        {
            StoneColor targetColor = StartElement.Color;
            return EvaluateTree(targetColor, reverse);
        }

        /// <summary>
        /// Überprüft, ob der Baum zutrifft
        /// </summary>
        /// <param name="targetColor">Die Zielfarbe.</param>
        /// <param name="reverse">Gibt an, ob der Baum in umgekehrter Richtung aufgerufen werden soll</param>
        /// <returns><c>true</c>, wenn der Baum positiv ausgewertet wurde, ansonsten <c>false</c></returns>
        private bool EvaluateTree(StoneColor targetColor, bool reverse = false)
        {
            if (StartNode == null || StartElement == null) return false;

            PatternNode node = StartNode;
            Element element = StartElement;

            // Schnelltest: Wenn die Kette länger ist, als die Anzahl der Steine in dieser Richtung,
            // können wir die Auswertung direkt abbrechen.
            int count = StartElement.GetSpaceInDirection(MoveDirection);
            if (node.ChainLength - 1 > count) return false;

            // Letzten Knoten auswählen, falls "Reverse"-Operation gewünscht ist
            if(reverse) while (!node.IsLastNode) node = node.NextNode;

            // Baum auswerten
            while(node != null)
            {
                // TODO: waagerechte Elemente auswerten
                if (!node.TestFunction(this, targetColor, element)) return false;

                // Waagerechte Knoten auswerten
                if(node.HasPerpendicularNode)
                {
                    // Erste Richtungsoption wählen
                    Direction perpendicularDirection = MoveDirection.GetPerpendicular(Option.First);
                    Element perpendicularElement = element.GetNeighbourInDirection(perpendicularDirection);
                    PatternRunner leftRunner = perpendicularElement == null ? null : new PatternRunner(node.PerpendicularNode, perpendicularDirection, perpendicularElement);

                    // Zweite Richtungsoption wählen
                    perpendicularDirection = MoveDirection.GetPerpendicular(Option.Second);
                    perpendicularElement = element.GetNeighbourInDirection(perpendicularDirection);
                    PatternRunner rightRunner = perpendicularElement == null ? null : new PatternRunner(node.PerpendicularNode, perpendicularDirection, perpendicularElement);

                    // Runner auswerten
                    bool leftSuccess = leftRunner == null ? false : leftRunner.EvaluateTree(targetColor);
                    bool rightSuccess = rightRunner == null ? false : rightRunner.EvaluateTree(targetColor);

                    // Wenn beide fehlgeschlagen sind, brauchen wir den Baum nicht weitertesten
                    if (!leftSuccess && !rightSuccess) return false;

                    // Ansonsten merken wir uns den erstbesten Kandidaten
                    if(leftSuccess && leftRunner.RecommendationCandiate != null)
                    {
                        RegisterCandidate(leftRunner.RecommendationCandiate);
                    }
                    else if (rightSuccess && rightRunner.RecommendationCandiate != null)
                    {
                        RegisterCandidate(rightRunner.RecommendationCandiate);
                    }
                }

                // Nächsten Knoten wählen
                if (node.IsLastNode) break;
                node = reverse ? node.PrevNode : node.NextNode;

                // Nächstes Element wählen
                element = element.GetNeighbourInDirection(MoveDirection);
                Debug.Assert(element != null, "Element war null, obwohl der eingehende Test genügend Bewegungsfreiraum ermittelt hat.");
            }

            // Ergebnis anpassen, wenn Reverse-Operation gewünscht ist
            if(reverse && RecommendationCandiate != null) RecommendationCandiate.RevertDirection();

            return true;
        }
        
        /// <summary>
        /// Registriert das Element als Kandidat für einen Zug
        /// </summary>
        /// <param name="element">Das betroffene Element</param>
        /// <param name="moveBackwards">Gibt an, dass das Element rückwärts in Auswärtungsrichtung bewegt werden soll. Wenn <c>false</c>, wird das Element in Auswertungsrichtung bewegt.</param>
        /// <returns>Immer <c>true</c></returns>
        public bool RegisterCandidate(Element element, bool moveBackwards = true)
        {
            RecommendationCandiate = new Recommendation(element, moveBackwards ? MoveDirection.GetOpposite() : MoveDirection);
            return true;
        }

        /// <summary>
        /// Übernimmt eine Empfehlung, z.B. aus einem Unterbaum
        /// </summary>
        /// <param name="recommendation">Die zu übernehmende Empfehlung</param>
        /// <returns>Immer <c>true</c></returns>
        private void RegisterCandidate(Recommendation recommendation)
        {
            RecommendationCandiate = recommendation;
        }
    }
}
