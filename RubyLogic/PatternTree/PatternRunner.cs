// ID $Id$

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
        public Element StartElement { get; private set; }

        /// <summary>
        /// Die Bewegungsrichtung
        /// </summary>
        public Direction MoveDirection { get; private set; }

        /// <summary>
        /// Der Startknoten
        /// </summary>
        public PatternNode StartNode { get; private set; }

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
        /// <returns></returns>
        public bool TestMatch()
        {
            PatternNode node = StartNode;
            Element element = StartElement;

            // Schnelltest: Wenn die Kette länger ist, als die Anzahl der Steine in dieser Richtung,
            // können wir die Auswertung direkt abbrechen.
            if(node != null)
            {
                int count = 0;
                switch(MoveDirection)
                {
                    case Direction.Up:
                        count = StartElement.SpaceTop;
                        break;
                    case Direction.Down:
                        count = StartElement.SpaceBottom;
                        break;
                    case Direction.Left:
                        count = StartElement.SpaceLeft;
                        break;
                    case Direction.Right:
                        count = StartElement.SpaceRight;
                        break;
                }
                if (node.ChainLength > count) return false;
            }

            // Baum auswerten
            while(node != null)
            {
                if (!node.TestFunction(this, StartElement.Color, element)) return false;
                node = node.NextNode;
            }
            return true;
        }
    }
}
