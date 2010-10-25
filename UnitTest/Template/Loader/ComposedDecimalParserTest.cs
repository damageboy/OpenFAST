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
using OpenFAST.Template.Types;
using OpenFAST.UnitTests.Test;

namespace OpenFAST.UnitTests.Template.Loader
{
    [TestFixture]
    public class ComposedDecimalParserTest : OpenFastTestCase
    {
        #region Setup/Teardown

        [SetUp]
        protected void SetUp()
        {
            _parser = new ComposedDecimalParser();
            _context = XmlMessageTemplateLoader.CreateInitialContext();
        }

        #endregion

        private IFieldParser _parser;
        private ParsingContext _context;

        [Test]
        public void TestFullyDefinedOperators()
        {
            XmlElement decimalDef =
                Document(
                    "<decimal name='composed'><mantissa><delta dictionary='template' key='variable' value='100'/></mantissa><exponent><copy dictionary='template' key='static' value='-2'/></exponent></decimal>");
            Assert.True(_parser.CanParse(decimalDef, _context));
            var decimalt = (ComposedScalar) _parser.Parse(decimalDef, _context);

            AssertComposedScalarField(decimalt, FASTType.Decimal, "composed", OpenFAST.Template.Operator.Operator.Copy,
                                      new IntegerValue(-2), OpenFAST.Template.Operator.Operator.Delta,
                                      new IntegerValue(100));

            Scalar exponent = decimalt.Fields[0];
            Scalar mantissa = decimalt.Fields[1];

            Assert.AreEqual("template", exponent.Dictionary);
            Assert.AreEqual(new QName("static"), exponent.Key);

            Assert.AreEqual("template", exponent.Dictionary);
            Assert.AreEqual(new QName("variable"), mantissa.Key);
        }

        [Test]
        public void TestInheritDictionary()
        {
            XmlElement decimalDef =
                Document(
                    "<decimal name='composed'><mantissa><delta/></mantissa><exponent><constant value='-2'/></exponent></decimal>");
            _context.Dictionary = "template";
            Assert.True(_parser.CanParse(decimalDef, _context));
            var decimalt = (ComposedScalar) _parser.Parse(decimalDef, _context);
            AssertComposedScalarField(decimalt, FASTType.Decimal, "composed",
                                      OpenFAST.Template.Operator.Operator.Constant, new IntegerValue(-2),
                                      OpenFAST.Template.Operator.Operator.Delta, ScalarValue.Undefined);
            Assert.AreEqual("template", decimalt.Fields[0].Dictionary);
            Assert.AreEqual("template", decimalt.Fields[1].Dictionary);
        }

        [Test]
        public void TestParse()
        {
            XmlElement decimalDef =
                Document(
                    "<decimal name='composed'><mantissa><delta/></mantissa><exponent><constant value='-2'/></exponent></decimal>");
            Assert.True(_parser.CanParse(decimalDef, _context));
            var decimalt = (ComposedScalar) _parser.Parse(decimalDef, _context);
            AssertComposedScalarField(decimalt, FASTType.Decimal, "composed",
                                      OpenFAST.Template.Operator.Operator.Constant, new IntegerValue(-2),
                                      OpenFAST.Template.Operator.Operator.Delta, ScalarValue.Undefined);
        }
    }
}