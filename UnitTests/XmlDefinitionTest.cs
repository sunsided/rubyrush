// ID $Id$

using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using RubyLogic;
using RubyLogic.PatternDefinition;

namespace UnitTests
{
    [TestFixture]
    public sealed class XmlDefinitionTest
    {
        /// <summary>
        /// Testet das Laden von Musterdefinitionen aus einer XML-Datei
        /// </summary>
        [Test]
        public void TestFileLoading()
        {
            IList<Pattern> pattern = PatternImport.LoadFromXml(new FileInfo("PatternDefinitions.xml"));
            Assert.IsNotNull(pattern);
        }
    }
}
