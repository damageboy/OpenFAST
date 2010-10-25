/*

The contents of this file are subject to the Mozilla Public License
Version 1.1 (the "License"); you may not use this file except in
compliance with the License. You may obtain a copy of the License at
http://www.mozilla.org/MPL/

Software distributed under the License is distributed on an "AS IS"
basis, WITHOUT WARRANTY OF ANY KIND, either express or implied. See the
License for the specific language governing rights and limitations
under the License.

The Original Code is OpenFAST.

The Initial Developer of the Original Code is The LaSalle Technology
Group, LLC.  Portions created by Shariq Muhammad
are Copyright (C) Shariq Muhammad. All Rights Reserved.

Contributor(s): Shariq Muhammad <shariq.muhammad@gmail.com>
                Yuri Astrakhan <FirstName><LastName>@gmail.com
*/
using System.Xml;
using NUnit.Framework;
using OpenFAST.Template;
using OpenFAST.Template.Loader;
using OpenFAST.UnitTests.Test;

namespace OpenFAST.UnitTests.Template.Loader
{
    [TestFixture]
    public class SequenceParserTest : OpenFastTestCase
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            _parser = new SequenceParser();
            _context = ParsingContext.Null;
        }

        #endregion

        private SequenceParser _parser;
        private ParsingContext _context;

        [Test]
        public void TestInheritDictionary()
        {
            var c = new ParsingContext(_context) {Dictionary = "template"};
            XmlElement node = Document("<sequence name='seq'/>");

            Assert.True(_parser.CanParse(node, c));
            var sequence = (Sequence) _parser.Parse(node, c);
            Assert.AreEqual("template", sequence.Length.Dictionary);

            node = Document("<sequence name='seq'><length name='explicitLength'/></sequence>");
            sequence = (Sequence) _parser.Parse(node, c);
            Assert.AreEqual("template", sequence.Length.Dictionary);
        }

        [Test]
        public void TestInheritance()
        {
            const string ns = "http://openfast.org/test";
            const string dictionary = "template";
            var c = new ParsingContext(_context) {Dictionary = dictionary, Namespace = ns};

            XmlElement node = Document("<sequence name='seq'><length name='seqLen'/></sequence>");

            Assert.True(_parser.CanParse(node, c));
            var sequence = (Sequence) _parser.Parse(node, c);
            Assert.AreEqual(dictionary, sequence.Length.Dictionary);
            Assert.AreEqual(ns, sequence.Length.QName.Namespace);
            Assert.AreEqual(ns, sequence.QName.Namespace);
        }

        [Test]
        public void TestOverride()
        {
            var c = new ParsingContext(_context) {Dictionary = "template", Namespace = "http://openfast.org/test"};

            XmlElement node =
                Document(
                    "<sequence name='seq' ns='http://openfast.org/override' dictionary='type'>" +
                    "  <length name='seqLen'/>" +
                    "</sequence>");

            Assert.True(_parser.CanParse(node, c));
            var sequence = (Sequence) _parser.Parse(node, c);
            Assert.AreEqual(DictionaryFields.Type, sequence.Length.Dictionary);
            Assert.AreEqual("http://openfast.org/override", sequence.Length.QName.Namespace);
            Assert.AreEqual("http://openfast.org/override", sequence.QName.Namespace);
        }

        [Test]
        public void TestSequenceWithFields()
        {
            var c = new ParsingContext(XmlMessageTemplateLoader.CreateInitialContext())
                        {
                            Dictionary = "template",
                            Namespace = "http://openfast.org/test"
                        };

            XmlElement node = Document("<sequence name='seq' ns='http://openfast.org/override' dictionary='type'>" +
                                       "  <length name='seqLen'/>" +
                                       "  <string name='value'><copy/></string>" +
                                       "  <uInt32 name='date'><delta/></uInt32>" +
                                       "  <typeRef name='Seq' ns='org.openfast.override'/>" +
                                       "</sequence>");

            Assert.True(_parser.CanParse(node, c));
            var sequence = (Sequence) _parser.Parse(node, c);
            Assert.AreEqual(DictionaryFields.Type, sequence.Length.Dictionary);
            Assert.AreEqual("http://openfast.org/override", sequence.Length.QName.Namespace);
            Assert.AreEqual("http://openfast.org/override", sequence.QName.Namespace);
            Assert.AreEqual(new QName("Seq", "org.openfast.override"), sequence.TypeReference);
        }
    }
}