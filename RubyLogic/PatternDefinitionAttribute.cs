// ID $Id$

using System;
using System.Collections.Generic;
using System.Reflection;
using RubyLogic.PatternTree;

namespace RubyLogic
{
    /// <summary>
    /// Ein Attribut
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class PatternDefinitionAttribute : Attribute
    {
        /// <summary>
        /// Findet alle Pattern-Definitionen in dieser Assembly
        /// </summary>
        /// <returns></returns>
        public static IList<PatternNode> GetPatternDefinitions()
        {
            List<PatternNode> nodes = new List<PatternNode>();
            Assembly assembly = Assembly.GetExecutingAssembly();
            
            // Typen ermitteln
            Type[] types = assembly.GetTypes();
            for (int i = 0; i < types.Length; ++i)
            {
                Type type = types[i];

                // Methoden ermitteln
                MethodInfo[] methods = type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic |
                                BindingFlags.InvokeMethod);
                for (int m = 0; m < methods.Length; ++m)
                {
                    MethodInfo method = methods[m];

                    // Auf Attribut testen
                    PatternDefinitionAttribute[] attributes = (PatternDefinitionAttribute[])method.GetCustomAttributes(typeof(PatternDefinitionAttribute), true);
                    if (attributes.Length == 0) continue;

                    // Funktion auswerten
                    PatternNode node = method.Invoke(null, null) as PatternNode;
                    if (node == null) continue;
                    nodes.Add(node);
                }
            }

            return nodes;
        }
    }
}
