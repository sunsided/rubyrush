// ID $Id$ 

using RubyElement;

namespace RubyLogic
{
    /// <summary>
    /// Eine Empfehlung für einen Zug
    /// </summary>
    public sealed class Recommendation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Recommendation"/> class.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="move">The move.</param>
        public Recommendation(Element element, MoveDirection move)
        {
            Element = element;
            Move = move;
        }

        /// <summary>
        /// Das betroffene Element
        /// </summary>
        public Element Element { get; private set; }

        /// <summary>
        /// Die Bewegungsrichtung
        /// </summary>
        public MoveDirection Move { get; private set; }
    }
}
