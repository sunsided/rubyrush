// ID $Id$

using RubyElement;

namespace RubyLogic.PatternTree
{
    /// <summary>
    /// Delegat für einen Mustertest
    /// </summary>
    /// <param name="runner">Der aufrufende <see cref="PatternRunner"/></param>
    /// <param name="stoneColor">Die betroffene Steinfarbe</param>
    /// <param name="element">Das zu testende Element</param>
    /// <returns><c>true</c>, wenn der Test erfolgreich war, ansonsten <c>false</c></returns>
    public delegate bool PatternTestDelegate(PatternRunner runner, StoneColor stoneColor, Element element);
}
