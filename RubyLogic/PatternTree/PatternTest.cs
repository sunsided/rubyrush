// ID $Id$

using RubyElement;

namespace RubyLogic.PatternTree
{
    /// <summary>
    /// Pattern-Testfunktionen nach dem Schema der <see cref="TestDelegate"/>
    /// </summary>
    public static class PatternTest
    {
        /// <summary>
        /// Delegat für einen Mustertest
        /// </summary>
        /// <param name="runner">Der aufrufende <see cref="PatternRunner"/></param>
        /// <param name="stoneColor">Die betroffene Steinfarbe</param>
        /// <param name="element">Das zu testende Element</param>
        /// <returns><c>true</c>, wenn der Test erfolgreich war, ansonsten <c>false</c></returns>
        public delegate bool TestDelegate(PatternRunner runner, StoneColor stoneColor, Element element);

        /// <summary>
        /// Ermittelt, ob ein Element ungleich der Testfarbe ist
        /// </summary>
        /// <param name="runner">Der <see cref="PatternRunner"/>, der die Auswertung steuert</param>
        /// <param name="stoneColor">Die Zielfarbe</param>
        /// <param name="element">Das aktuelle Element</param>
        /// <returns><c>true</c>, wenn die Bedingung erfüllt ist, ansonsten <c>false</c></returns>
        public static bool Inequality(PatternRunner runner, StoneColor stoneColor, Element element)
        {
            return !element.Equals(stoneColor);
        }

        /// <summary>
        /// Ermittelt, ob ein Element irrelevant ist
        /// </summary>
        /// <param name="runner">Der <see cref="PatternRunner"/>, der die Auswertung steuert</param>
        /// <param name="stoneColor">Die Zielfarbe</param>
        /// <param name="element">Das aktuelle Element</param>
        /// <returns><c>true</c>, wenn die Bedingung erfüllt ist, ansonsten <c>false</c></returns>
        public static bool Invariant(PatternRunner runner, StoneColor stoneColor, Element element)
        {
            return true;
        }

        /// <summary>
        /// Ermittelt, ob ein Element gleich der Testfarbe ist
        /// </summary>
        /// <param name="runner">Der <see cref="PatternRunner"/>, der die Auswertung steuert</param>
        /// <param name="stoneColor">Die Zielfarbe</param>
        /// <param name="element">Das aktuelle Element</param>
        /// <returns><c>true</c>, wenn die Bedingung erfüllt ist, ansonsten <c>false</c></returns>
        public static bool Equality(PatternRunner runner, StoneColor stoneColor, Element element)
        {
            return element.Equals(stoneColor);
        }

        /// <summary>
        /// Ermittelt, ob ein Element gleich der Testfarbe ist.
        /// Trifft die Bedingung zu, wird das Element außerdem als Kandidat für eine Rückwärtsbewegung registriert.
        /// </summary>
        /// <param name="runner">Der <see cref="PatternRunner"/>, der die Auswertung steuert</param>
        /// <param name="stoneColor">Die Zielfarbe</param>
        /// <param name="element">Das aktuelle Element</param>
        /// <returns><c>true</c>, wenn die Bedingung erfüllt ist, ansonsten <c>false</c></returns>
        public static bool EqualityAndCandidate(PatternRunner runner, StoneColor stoneColor, Element element)
        {
            return EqualityAndBackwardCandidate(runner, stoneColor, element);
        }

        /// <summary>
        /// Ermittelt, ob ein Element gleich der Testfarbe ist.
        /// Trifft die Bedingung zu, wird das Element außerdem als Kandidat für eine Rückwärtsbewegung registriert.
        /// </summary>
        /// <param name="runner">Der <see cref="PatternRunner"/>, der die Auswertung steuert</param>
        /// <param name="stoneColor">Die Zielfarbe</param>
        /// <param name="element">Das aktuelle Element</param>
        /// <returns><c>true</c>, wenn die Bedingung erfüllt ist, ansonsten <c>false</c></returns>
        public static bool EqualityAndBackwardCandidate(PatternRunner runner, StoneColor stoneColor, Element element)
        {
            // Verwendet lazy evaluation, um den Kandiaten zu registrieren, falls nötig
            return Equality(runner, stoneColor, element) && runner.RegisterCandidate(element);
        }

        /// <summary>
        /// Ermittelt, ob ein Element gleich der Testfarbe ist.
        /// Trifft die Bedingung zu, wird das Element außerdem als Kandidat für eine Vorwärtsbewegung registriert.
        /// </summary>
        /// <param name="runner">Der <see cref="PatternRunner"/>, der die Auswertung steuert</param>
        /// <param name="stoneColor">Die Zielfarbe</param>
        /// <param name="element">Das aktuelle Element</param>
        /// <returns><c>true</c>, wenn die Bedingung erfüllt ist, ansonsten <c>false</c></returns>
        public static bool EqualityAndForwardCandidate(PatternRunner runner, StoneColor stoneColor, Element element)
        {
            // Verwendet lazy evaluation, um den Kandiaten zu registrieren, falls nötig
            return Equality(runner, stoneColor, element) && runner.RegisterCandidate(element, false);
        }
    }
}
