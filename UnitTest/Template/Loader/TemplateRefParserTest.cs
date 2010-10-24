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
using OpenFAST.Error;

namespace OpenFAST.UnitTests.Template.Loader
{
    [TestFixture]
    public class TemplateRefParserTest : OpenFastTestCase
    {
        private TemplateRefParser _parser;
        private ParsingContext _context;
        [SetUp]
        protected void SetUp()
        {
            _parser = new TemplateRefParser();
            _context = XmlMessageTemplateLoader.CreateInitialContext();
        }
        [Test]
        public void TestParseDynamic()
        {
            XmlElement dynTempRefDef = Document("<templateRef/>").DocumentElement;
            Assert.AreEqual(DynamicTemplateReference.Instance, _parser.Parse(dynTempRefDef, _context));
        }
        [Test]
        public void TestParseStatic()
        {
            XmlElement statTempRefDef = Document("<templateRef name=\"base\"/>").DocumentElement;
            var baset = new MessageTemplate("base", new Field[] { });
            _context.TemplateRegistry.Define(baset);
            var statTempRef = (StaticTemplateReference)_parser.Parse(statTempRefDef, _context);
            Assert.AreEqual(baset, statTempRef.Template);
        }
        [Test]
        public void TestParseStaticWithUndefinedTemplate()
        {
            XmlElement statTempRefDef = Document("<templateRef name=\"base\"/>").DocumentElement;
            try
            {
                _parser.Parse(statTempRefDef, _context);
            }
            catch (DynErrorException)
            {
                return;
            }
            Assert.Fail("An Exception was expected");
        }
    }
}
