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
using OpenFAST;
using OpenFAST.Template;
using OpenFAST.Template.Loader;
using OpenFAST.Template.Type;
using UnitTest.Test;

namespace UnitTest.Template.Loader
{
    [TestFixture]
    public class StringParserTest : OpenFastTestCase
    {
        private ScalarParser _parser;
        private ParsingContext _context;
        [SetUp]
        protected void SetUp()
        {
            _parser = new StringParser();
            _context = XmlMessageTemplateLoader.CreateInitialContext();
        }
        [Test]
        public void TestParse()
        {
            XmlElement unicodeDef = Document("<string name=\"message\" charset=\"unicode\"/>").DocumentElement;
            Assert.True(_parser.CanParse(unicodeDef, _context));
            var unicode = (Scalar)_parser.Parse(unicodeDef, _context);
            AssertScalarField(unicode, FASTType.Unicode, "message", null, "", DictionaryFields.Global, "message", OpenFAST.Template.Operator.Operator.None,
                    ScalarValue.Undefined, false);
        }
    }
}
