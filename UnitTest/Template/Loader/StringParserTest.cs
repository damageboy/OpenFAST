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
using OpenFAST.Template.Operators;
using OpenFAST.Template.Types;
using OpenFAST.UnitTests.Test;

namespace OpenFAST.UnitTests.Template.Loader
{
    [TestFixture]
    public class StringParserTest : OpenFastTestCase
    {
        #region Setup/Teardown

        [SetUp]
        protected void SetUp()
        {
            _parser = new StringParser();
            _context = XmlMessageTemplateLoader.CreateInitialContext();
        }

        #endregion

        private ScalarParser _parser;
        private ParsingContext _context;

        [Test]
        public void TestParse()
        {
            XmlElement unicodeDef = Document("<string name='message' charset='unicode'/>");
            Assert.True(_parser.CanParse(unicodeDef, _context));
            var unicode = (Scalar) _parser.Parse(unicodeDef, _context);
            AssertScalarField(unicode, FastType.Unicode, "message", null, "", DictionaryFields.Global, "message",
                              Operator.None, ScalarValue.Undefined, false);
        }
    }
}