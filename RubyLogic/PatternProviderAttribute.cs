// ID $Id$

using System;

namespace RubyLogic
{
    /// <summary>
    /// Ein Attribut
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class PatternProviderAttribute : Attribute
    {
    }
}
