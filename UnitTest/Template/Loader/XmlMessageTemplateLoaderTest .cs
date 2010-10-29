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
using System.IO;
using System.Text;
using NUnit.Framework;
using OpenFAST.Error;
using OpenFAST.Template;
using OpenFAST.Template.Loader;
using OpenFAST.Template.Operators;
using OpenFAST.Template.Types;
using OpenFAST.Template.Types.Codec;
using OpenFAST.UnitTests.Test;

namespace OpenFAST.UnitTests.Template.Loader
{
    [TestFixture]
    public class XmlMessageTemplateLoaderTest : OpenFastTestCase
    {
        [Test]
        public void TestByteVector()
        {
            const string templateXml = "<template name='bvt'>" +
                                       "  <byteVector name='data'>" +
                                       "    <length name='dataLength'/>" +
                                       "    <tail/>" +
                                       "  </byteVector>" +
                                       "</template>";
            IMessageTemplateLoader loader = new XmlMessageTemplateLoader();
            MessageTemplate bvt = loader.Load(new MemoryStream(Encoding.ASCII.GetBytes(templateXml)))[0];

            AssertScalarField(bvt, 1, FastType.ByteVector, "data", Operator.Tail);
        }

        [Test]
        public void TestDynamicTemplateReference()
        {
            const string template1Xml = "<template name='t1'>" +
                                        "  <string name='string'/>" +
                                        "</template>";
            const string template2Xml = "<template name='t2'>" +
                                        "  <uInt32 name='quantity'/>" +
                                        "  <templateRef/>" +
                                        "  <decimal name='price'/>" +
                                        "</template>";

            IMessageTemplateLoader loader = new XmlMessageTemplateLoader();
            loader.Load(new MemoryStream(Encoding.ASCII.GetBytes(template1Xml)));
            MessageTemplate[] templates = loader.Load(new MemoryStream(Encoding.ASCII.GetBytes(template2Xml)));

            Assert.AreEqual(4, templates[0].FieldCount);
            AssertScalarField(templates[0], 1, FastType.U32, "quantity", Operator.None);
            AssertScalarField(templates[0], 3, FastType.Decimal, "price", Operator.None);
            Assert.True(templates[0].GetField(2) is DynamicTemplateReference);
        }

        [Test]
        public void TestLoadMdIncrementalRefreshTemplate()
        {
            MessageTemplate messageTemplate;
            using (var stream = File.OpenRead("FPL/mdIncrementalRefreshTemplate.xml"))
                messageTemplate = new XmlMessageTemplateLoader().Load(stream)[0];

            Assert.AreEqual("MDIncrementalRefresh", messageTemplate.TypeReference.Name);
            Assert.AreEqual("MDRefreshSample", messageTemplate.Name);
            Assert.AreEqual(10, messageTemplate.FieldCount);

            /********************************** TEMPLATE FIELDS **********************************/
            int index = 0;
            AssertScalarField(messageTemplate, index++, FastType.U32, "templateId", Operator.Copy);
            AssertScalarField(messageTemplate, index++, FastType.Ascii, "8", Operator.Constant);
            AssertScalarField(messageTemplate, index++, FastType.U32, "9", Operator.Constant);
            AssertScalarField(messageTemplate, index++, FastType.Ascii, "35", Operator.Constant);
            AssertScalarField(messageTemplate, index++, FastType.Ascii, "49", Operator.Constant);
            AssertScalarField(messageTemplate, index++, FastType.U32, "34", Operator.Increment);
            AssertScalarField(messageTemplate, index++, FastType.Ascii, "52", Operator.Delta);
            AssertScalarField(messageTemplate, index++, FastType.U32, "75", Operator.Copy);

            /************************************* SEQUENCE **************************************/
            AssertSequence(messageTemplate, index, 17);

            var sequence = (Sequence) messageTemplate.GetField(index++);
            Assert.AreEqual("MDEntries", sequence.TypeReference.Name);
            AssertSequenceLengthField(sequence, "268", FastType.U32, Operator.None);

            /********************************** SEQUENCE FIELDS **********************************/
            int seqIndex = 0;
            AssertScalarField(sequence, seqIndex++, FastType.Decimal, "270", Operator.Delta);
            AssertScalarField(sequence, seqIndex++, FastType.I32, "271", Operator.Delta);
            AssertScalarField(sequence, seqIndex++, FastType.U32, "273", Operator.Delta);
            AssertOptionalScalarField(sequence, seqIndex++, FastType.U32, "346", Operator.None);
            AssertScalarField(sequence, seqIndex++, FastType.U32, "1023", Operator.Increment);
            AssertScalarField(sequence, seqIndex++, FastType.Ascii, "279", Operator.Copy);
            AssertScalarField(sequence, seqIndex++, FastType.Ascii, "269", Operator.Copy);
            AssertScalarField(sequence, seqIndex++, FastType.Ascii, "107", Operator.Copy);
            AssertScalarField(sequence, seqIndex++, FastType.Ascii, "48", Operator.Delta);
            AssertScalarField(sequence, seqIndex++, FastType.Ascii, "276", Operator.Copy);
            AssertScalarField(sequence, seqIndex++, FastType.Ascii, "274", Operator.Copy);
            AssertScalarField(sequence, seqIndex++, FastType.Decimal, "451", Operator.Copy);
            AssertScalarField(sequence, seqIndex++, FastType.Ascii, "277", Operator.Default);
            AssertOptionalScalarField(sequence, seqIndex++, FastType.U32, "1020", Operator.None);
            AssertScalarField(sequence, seqIndex++, FastType.I32, "537", Operator.Default);
            AssertScalarField(sequence, seqIndex++, FastType.Ascii, "1024", Operator.Default);
            AssertScalarField(sequence, seqIndex, FastType.Ascii, "336", Operator.Default);

            AssertScalarField(messageTemplate, index, FastType.Ascii, "10", Operator.None);
        }

        [Test]
        public void TestLoadTemplateThatContainsDecimalWithTwinOperators()
        {
            const string templateXml = "<templates xmlns='http://www.fixprotocol.org/ns/template-definition'" +
                                       "	ns='http://www.fixprotocol.org/ns/templates/sample'>" +
                                       "	<template name='SampleTemplate'>" +
                                       "		<decimal name='bid'><mantissa><delta/></mantissa><exponent><copy value='-2'/></exponent></decimal>" +
                                       "		<decimal name='ask'><mantissa><delta/></mantissa></decimal>" +
                                       "		<decimal name='high'><exponent><copy/></exponent></decimal>" +
                                       "		<decimal name='low'><mantissa><delta value='10'/></mantissa><exponent><copy value='-2'/></exponent></decimal>" +
                                       "		<decimal name='open'><copy/></decimal>" +
                                       "		<decimal name='close'><copy/></decimal>" +
                                       "	</template>" +
                                       "</templates>";
            IMessageTemplateLoader loader = new XmlMessageTemplateLoader();
            MessageTemplate[] templates = loader.Load(new MemoryStream(Encoding.ASCII.GetBytes(templateXml)));
            MessageTemplate messageTemplate = templates[0];
            Assert.AreEqual("SampleTemplate", messageTemplate.Name);
            Assert.AreEqual(7, messageTemplate.FieldCount);
            AssertComposedScalarField(messageTemplate, 1, FastType.Decimal, "bid", Operator.Copy, Int(-2),
                                      Operator.Delta, ScalarValue.Undefined);
            AssertComposedScalarField(messageTemplate, 2, FastType.Decimal, "ask", Operator.None, ScalarValue.Undefined,
                                      Operator.Delta, ScalarValue.Undefined);
            AssertComposedScalarField(messageTemplate, 3, FastType.Decimal, "high", Operator.Copy, ScalarValue.Undefined,
                                      Operator.None, ScalarValue.Undefined);
            AssertComposedScalarField(messageTemplate, 4, FastType.Decimal, "low", Operator.Copy, Int(-2),
                                      Operator.Delta, Int(10));
            AssertScalarField(messageTemplate, 5, FastType.Decimal, "open", Operator.Copy);
            AssertScalarField(messageTemplate, 6, FastType.Decimal, "close", Operator.Copy);
        }

        [Test]
        public void TestLoadTemplateThatContainsGroups()
        {
            const string templateXml = "<templates xmlns='http://www.fixprotocol.org/ns/template-definition'" +
                                       "	ns='http://www.fixprotocol.org/ns/templates/sample'>" +
                                       "	<template name='SampleTemplate'>" +
                                       "		<group name='guy'><string name='First Name'/><string name='Last Name'/></group>" +
                                       "	</template>" + "</templates>";

            IMessageTemplateLoader loader = new XmlMessageTemplateLoader();
            MessageTemplate[] templates = loader.Load(new MemoryStream(
                                                          Encoding.ASCII.GetBytes(templateXml)));
            MessageTemplate messageTemplate = templates[0];

            Assert.AreEqual("SampleTemplate", messageTemplate.Name);
            Assert.AreEqual(2, messageTemplate.FieldCount);

            AssertGroup(messageTemplate, 1, "guy");
        }

        [Test]
        public void TestLoadTemplateWithKey()
        {
            const string templateXml = "<templates xmlns='http://www.fixprotocol.org/ns/template-definition'" +
                                       "	ns='http://www.fixprotocol.org/ns/templates/sample'>" +
                                       "	<template name='SampleTemplate'>" +
                                       "		<uInt32 name='value'><copy key='integer'/></uInt32>" +
                                       "	</template>" +
                                       "</templates>";

            IMessageTemplateLoader loader = new XmlMessageTemplateLoader();
            MessageTemplate[] templates = loader.Load(new MemoryStream(
                                                          Encoding.ASCII.GetBytes(templateXml)));
            MessageTemplate messageTemplate = templates[0];

            Scalar scalar = messageTemplate.GetScalar("value");
            Assert.AreEqual(new QName("integer"), scalar.Key);
        }

        [Test]
        public void TestLoadTemplateWithUnicodeString()
        {
            const string templateXml = "<templates xmlns='http://www.fixprotocol.org/ns/template-definition'" +
                                       "	ns='http://www.fixprotocol.org/ns/templates/sample'>" +
                                       "	<template name='SampleTemplate'>" +
                                       "		<string name='name' charset='unicode' presence='mandatory'><copy/></string>" +
                                       "		<string name='id' charset='unicode' presence='optional'><copy/></string>" +
                                       "		<string name='location' charset='ascii' presence='mandatory'><copy/></string>" +
                                       "		<string name='id2' charset='ascii' presence='optional'><copy/></string>" +
                                       "	</template>" + "</templates>";

            IMessageTemplateLoader loader = new XmlMessageTemplateLoader();
            MessageTemplate[] templates = loader.Load(new MemoryStream(
                                                          Encoding.ASCII.GetBytes(templateXml)));
            MessageTemplate messageTemplate = templates[0];

            Scalar name = messageTemplate.GetScalar("name");
            Scalar id = messageTemplate.GetScalar("id");
            Scalar location = messageTemplate.GetScalar("location");
            Scalar id2 = messageTemplate.GetScalar("id2");

            Assert.False(name.IsOptional);
            Assert.True(id.IsOptional);
            Assert.False(location.IsOptional);
            Assert.True(id2.IsOptional);

            Assert.AreEqual(TypeCodec.Unicode, name.TypeCodec);
            Assert.AreEqual(TypeCodec.NullableUnicode, id.TypeCodec);
            Assert.AreEqual(TypeCodec.Ascii, location.TypeCodec);
            Assert.AreEqual(TypeCodec.NullableAscii, id2.TypeCodec);
        }

        [Test]
        public void TestNonExistantTemplateReference()
        {
            const string template2Xml = "<template name='t2'>" +
                                        "  <uInt32 name='quantity'/>" +
                                        "  <templateRef name='t1'/>" +
                                        "  <decimal name='price'/>" +
                                        "</template>";
            try
            {
                new XmlMessageTemplateLoader().Load(new MemoryStream(Encoding.ASCII.GetBytes(template2Xml)));
            }
            catch (DynErrorException e)
            {
                Assert.AreEqual(DynError.TemplateNotExist, e.Error);
            }
        }

        [Test]
        public void TestNullDocument()
        {
            var loader = new XmlMessageTemplateLoader {ErrorHandler = ErrorHandlerFields.Null};
            Assert.AreEqual(0, loader.Load((Stream) null).Length);
        }

        [Test]
        public void TestReferencedTemplateInOtherLoader()
        {
            const string template1Xml = "<template name='t1'>" +
                                        "  <string name='string'/>" +
                                        "</template>";
            const string template2Xml = "<template name='t2'>" +
                                        "  <uInt32 name='quantity'/>" +
                                        "  <templateRef name='t1'/>" +
                                        "  <decimal name='price'/>" +
                                        "</template>";

            IMessageTemplateLoader loader1 = new XmlMessageTemplateLoader();
            IMessageTemplateLoader loader2 = new XmlMessageTemplateLoader {TemplateRegistry = loader1.TemplateRegistry};

            loader1.Load(new MemoryStream(Encoding.ASCII.GetBytes(template1Xml)));
            MessageTemplate[] templates = loader2.Load(new MemoryStream(Encoding.ASCII.GetBytes(template2Xml)));
            Assert.AreEqual(4, templates[0].FieldCount);
            AssertScalarField(templates[0], 1, FastType.U32, "quantity", Operator.None);
            AssertScalarField(templates[0], 2, FastType.Ascii, "string", Operator.None);
            AssertScalarField(templates[0], 3, FastType.Decimal, "price", Operator.None);
        }

        [Test]
        public void TestStaticTemplateReference()
        {
            const string templateXml = "<templates>" +
                                       "  <template name='t1'>" +
                                       "    <string name='string'/>" +
                                       "  </template>" +
                                       "  <template name='t2'>" +
                                       "    <uInt32 name='quantity'/>" +
                                       "    <templateRef name='t1'/>" +
                                       "    <decimal name='price'/>" +
                                       "  </template>" +
                                       "</templates>";
            MessageTemplate[] templates =
                new XmlMessageTemplateLoader().Load(new MemoryStream(Encoding.ASCII.GetBytes(templateXml)));
            AssertEquals(4, templates[1].FieldCount);
            AssertScalarField(templates[1], 1, FastType.U32, "quantity", Operator.None);
            AssertScalarField(templates[1], 2, FastType.Ascii, "string", Operator.None);
            AssertScalarField(templates[1], 3, FastType.Decimal, "price", Operator.None);
        }

        [Test]
        public void TestTemplateReferencedFromPreviousLoad()
        {
            const string template1Xml = "<template name='t1'>" +
                                        "  <string name='string'/>" +
                                        "</template>";
            const string template2Xml = "<template name='t2'>" +
                                        "  <uInt32 name='quantity'/>" +
                                        "  <templateRef name='t1'/>" +
                                        "  <decimal name='price'/>" +
                                        "</template>";

            IMessageTemplateLoader loader = new XmlMessageTemplateLoader();
            loader.Load(new MemoryStream(Encoding.ASCII.GetBytes(template1Xml)));
            MessageTemplate[] templates = loader.Load(new MemoryStream(Encoding.ASCII.GetBytes(template2Xml)));

            Assert.AreEqual(4, templates[0].FieldCount);
            AssertScalarField(templates[0], 1, FastType.U32, "quantity", Operator.None);
            AssertScalarField(templates[0], 2, FastType.Ascii, "string", Operator.None);
            AssertScalarField(templates[0], 3, FastType.Decimal, "price", Operator.None);
        }

        //public void testCustomFieldParser() {
        //    string templateXml = 
        //        "<template name='custom'>" +
        //        "  <array name='intArr' type='int'/>" +
        //        "</template>";
        //    XmlMessageTemplateLoader loader = new XmlMessageTemplateLoader();
        //    try {
        //        loader.Load(new MemoryStream(System.Text.Encoding.ASCII.GetBytes(templateXml)));
        //    } catch (FastException e) {
        //        //assertEquals("No parser registered for array", e.getMessage());
        //        //assertEquals(FastConstants.PARSE_ERROR, e.getCode());
        //    }
        //    loader.AddFieldParser(new FieldParser() {

        //        public boolean canParse(Element element, ParsingContext context) {
        //            return element.getNodeName().equals("array");
        //        }

        //        public Field parse(Element fieldNode, ParsingContext context) {
        //            return new Array(new QName(fieldNode.getAttribute("name"), ""), false);
        //        }});

        //    MessageTemplate template = loader.load(stream(templateXml))[0];
        //    assertEquals(new Array(new QName("intArr", ""), false), template.getField(1));
        //}
    }
}