// ID $Id$

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using RubyLogic.PatternTree;
using RubyLogic.Properties;

namespace RubyLogic.PatternDefinition
{
    /// <summary>
    /// Klasse zum Importieren von Musterdefinitionen aus XML-Dateien
    /// </summary>
    public static class PatternImport
    {
        private static void ValidationEventHandler(object sender, ValidationEventArgs args)
        {
            ;
        }

        /// <summary>
        /// Lädt Definitionen aus einer XML-Datei
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static IList<Pattern> LoadFromXml(FileInfo file)
        {
            if (file == null) throw new ArgumentNullException("file");

            // XML-Schema
            TextReader stringReader = new StringReader(Resources.PatternDefinitionsXsd);
            XmlSchema schema = XmlSchema.Read(stringReader, ValidationEventHandler);

            // Settings
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.DtdProcessing = DtdProcessing.Ignore;
            settings.IgnoreComments = true;
            settings.IgnoreWhitespace = true;
            settings.IgnoreProcessingInstructions = true;
            settings.ConformanceLevel = ConformanceLevel.Document;
            settings.Schemas = new XmlSchemaSet();
            settings.Schemas.Add(schema);
            settings.ValidationType = ValidationType.Schema;
            settings.ValidationFlags = XmlSchemaValidationFlags.ProcessInlineSchema |
                                       XmlSchemaValidationFlags.AllowXmlAttributes |
                                       XmlSchemaValidationFlags.ProcessSchemaLocation |
                                       XmlSchemaValidationFlags.ProcessIdentityConstraints;

            using(FileStream stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.Read))
            using (XmlReader reader = XmlReader.Create(stream, settings))
            {
                XmlDocument document = new XmlDocument();
                document.Load(reader);
                XmlNode root = document.DocumentElement;
                if (root == null) return null;

                // Namespace hinzufügen
                XmlNamespaceManager nsmgr = new XmlNamespaceManager(document.NameTable);
                nsmgr.AddNamespace("pattern", "urn:RubyRush:PatternDefinition:1.0");

                // Pattern ermitteln
                XmlNodeList patternDefinitions = root.SelectNodes("child::pattern:Pattern", nsmgr);
                if (patternDefinitions == null || patternDefinitions.Count == 0) return null;

                // Patternliste erzeugen
                List<Pattern> patternList = new List<Pattern>(patternDefinitions.Count);

                // Alle Pattern durchlaufen
                Trace.WriteLine(patternDefinitions.Count + " Musterdefinitionen gefunden.");
                for (int i = 0; i < patternDefinitions.Count; i++)
                {
                    XmlNode patternNode = patternDefinitions[i];

                    // Attribute beziehen
                    int rank = Int32.Parse(patternNode.Attributes["rank"].Value, CultureInfo.InvariantCulture);
                    string name = patternNode.Attributes["name"].Value;
                    bool isSymmetric = patternNode.Attributes["isSymmetric"].Value == null
                                           ? false
                                           : Boolean.Parse(patternNode.Attributes["isSymmetric"].Value);

                    // Matches laden
                    PatternNode rootMatch = ReadMatches(patternNode, nsmgr);
                    if (rootMatch == null) continue;

                    // Pattern erzeugen
                    Trace.WriteLine("Erzeuge Muster: " + name + " (Rang " + rank + ")");
                    Pattern pattern = new Pattern(rootMatch, rank, name, isSymmetric);
                    patternList.Add(pattern);
                }

                return patternList;
            }
        }

        /// <summary>
        /// Liest die Matches des aktuellen Subknotens
        /// </summary>
        /// <param name="patternNode"></param>
        /// <param name="nsmgr"></param>
        /// <returns></returns>
        private static PatternNode ReadMatches(XmlNode patternNode, XmlNamespaceManager nsmgr)
        {
            XmlNodeList matches = patternNode.SelectNodes("child::pattern:Match", nsmgr);
            if (matches == null || matches.Count == 0) return null;

            // Root-Match
            PatternNode rootMatch = null;
            PatternNode lastNode = null;

            // Matches auswerten
            for (int m = 0; m < matches.Count; ++m)
            {
                XmlNode matchNode = matches[m];

                // Knoten lesen
                PatternNode node = ReadPatternNode(matchNode, nsmgr);

                // In bestehende Struktur einhängen
                if (lastNode != null)
                {
                    // Knoten an den letzten Knoten anhängen
                    lastNode.AppendSimpleNode(node);
                    lastNode = node;
                }
                else
                {
                    // Root-Knoten setzen
                    rootMatch = node;
                    lastNode = node;
                }
            }
            return rootMatch;
        }

        /// <summary>
        /// Liefert einen PatternNode
        /// </summary>
        /// <param name="matchNode"></param>
        /// <returns></returns>
        private static PatternNode ReadPatternNode(XmlNode matchNode, XmlNamespaceManager nsmgr)
        {
            if (matchNode == null) throw new ArgumentNullException("matchNode");

            // Attribute auswerten
            string type = matchNode.Attributes["type"].Value;
            string candidateType = matchNode.Attributes["candidateType"].Value;

            // Typ auswerten
            PatternTest.TestDelegate function;
            switch (type)
            {
                case "equality":
                    {
                        // Kandidatentyp auswerten
                        switch (candidateType)
                        {
                            case "backward":
                                function = PatternTest.EqualityAndBackwardCandidate;
                                break;
                            case "forward":
                                function = PatternTest.EqualityAndForwardCandidate;
                                break;
                            case "none":
                                function = PatternTest.Equality;
                                break;
                            default:
                                throw new InvalidOperationException("Unbekannter candidateType: " +
                                                                    candidateType);
                        }
                        break;
                    }
                case "inequality":
                    {
                        function = PatternTest.Inequality;
                        break;
                    }
                case "invariant":
                    {
                        function = PatternTest.Invariant;
                        break;
                    }
                default:
                    {
                        throw new InvalidOperationException("Unbekannter type: " + type);
                    }
            }

            // Node erzeugen
            PatternNode patternNode = new PatternNode(function);

            // Testen, ob Seitenmuster vorliegen
            XmlNodeList subPatternList = matchNode.SelectNodes("child::pattern:SubPattern", nsmgr);
            if(subPatternList != null && subPatternList.Count > 0)
            {
                // Es liegen Subpattern vor
                for (int sp = 0; sp < subPatternList.Count; ++sp)
                {
                    XmlNode subPatternNode = subPatternList[sp];
                    PatternNode subPattern = ReadMatches(subPatternNode, nsmgr);
                    patternNode.AppendPerpendicularNode(subPattern);
                }
            }

            // Und raus.
            return patternNode;
        }
    }
}
