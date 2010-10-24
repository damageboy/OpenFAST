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
using OpenFAST.Error;
using OpenFAST.Template.Loader;
using OpenFAST.Template.Types;
using OpenFAST.UnitTests.Test;
using NUnit.Framework;
using System.IO;
using OpenFAST.Template;
using OpenFAST.Template.Types.Codec;

namespace OpenFAST.UnitTests.Template.Loader
{
    [TestFixture]
    public class XmlMessageTemplateLoaderTest : OpenFastTestCase
    {
        [Test]
        public void TestLoadTemplateThatContainsDecimalWithTwinOperators()
        {
            const string templateXml = "<templates xmlns=\"http://www.fixprotocol.org/ns/template-definition\"" +
                                       "	ns=\"http://www.fixprotocol.org/ns/templates/sample\">" +
                                       "	<template name=\"SampleTemplate\">" +
                                       "		<decimal name=\"bid\"><mantissa><delta /></mantissa><exponent><copy value=\"-2\" /></exponent></decimal>" +
                                       "		<decimal name=\"ask\"><mantissa><delta /></mantissa></decimal>" +
                                       "		<decimal name=\"high\"><exponent><copy/></exponent></decimal>" +
                                       "		<decimal name=\"low\"><mantissa><delta value=\"10\"/></mantissa><exponent><copy value=\"-2\" /></exponent></decimal>" +
                                       "		<decimal name=\"open\"><copy /></decimal>" +
                                       "		<decimal name=\"close\"><copy /></decimal>" + "	</template>" +
                                       "</templates>";
            IMessageTemplateLoader loader = new XmlMessageTemplateLoader();
            MessageTemplate[] templates = loader.Load(new MemoryStream(System.Text.Encoding.ASCII.GetBytes(templateXml)));
            MessageTemplate messageTemplate = templates[0];
            Assert.AreEqual("SampleTemplate", messageTemplate.Name);
            Assert.AreEqual(7, messageTemplate.FieldCount);
            AssertComposedScalarField(messageTemplate, 1, FASTType.Decimal, "bid", OpenFAST.Template.Operator.Operator.Copy, Int(-2), OpenFAST.Template.Operator.Operator.Delta, ScalarValue.Undefined);
            AssertComposedScalarField(messageTemplate, 2, FASTType.Decimal, "ask", OpenFAST.Template.Operator.Operator.None, ScalarValue.Undefined, OpenFAST.Template.Operator.Operator.Delta, ScalarValue.Undefined);
            AssertComposedScalarField(messageTemplate, 3, FASTType.Decimal, "high", OpenFAST.Template.Operator.Operator.Copy, ScalarValue.Undefined, OpenFAST.Template.Operator.Operator.None, ScalarValue.Undefined);
            AssertComposedScalarField(messageTemplate, 4, FASTType.Decimal, "low", OpenFAST.Template.Operator.Operator.Copy, Int(-2), OpenFAST.Template.Operator.Operator.Delta, Int(10));
            AssertScalarField(messageTemplate, 5, FASTType.Decimal, "open", OpenFAST.Template.Operator.Operator.Copy);
            AssertScalarField(messageTemplate, 6, FASTType.Decimal, "close", OpenFAST.Template.Operator.Operator.Copy);
        }
        [Test]
        public void TestLoadTemplateThatContainsGroups()
        {
            const string templateXml = "<templates xmlns=\"http://www.fixprotocol.org/ns/template-definition\"" +
                                       "	ns=\"http://www.fixprotocol.org/ns/templates/sample\">" +
                                       "	<template name=\"SampleTemplate\">" +
                                       "		<group name=\"guy\"><string name=\"First Name\"></string><string name=\"Last Name\"></string></group>" +
                                       "	</template>" + "</templates>";

            IMessageTemplateLoader loader = new XmlMessageTemplateLoader();
            MessageTemplate[] templates = loader.Load(new MemoryStream(
                        System.Text.Encoding.ASCII.GetBytes(templateXml)));
            MessageTemplate messageTemplate = templates[0];

            Assert.AreEqual("SampleTemplate", messageTemplate.Name);
            Assert.AreEqual(2, messageTemplate.FieldCount);

            AssertGroup(messageTemplate, 1, "guy");
        }
        [Test]
        public void TestLoadTemplateWithKey()
        {
            const string templateXml = "<templates xmlns=\"http://www.fixprotocol.org/ns/template-definition\"" +
                                       "	ns=\"http://www.fixprotocol.org/ns/templates/sample\">" +
                                       "	<template name=\"SampleTemplate\">" +
                                       "		<uInt32 name=\"value\"><copy key=\"integer\" /></uInt32>" +
                                       "	</template>" + "</templates>";

            IMessageTemplateLoader loader = new XmlMessageTemplateLoader();
            MessageTemplate[] templates = loader.Load(new MemoryStream(
                        System.Text.Encoding.ASCII.GetBytes(templateXml)));
            MessageTemplate messageTemplate = templates[0];

            Scalar scalar = messageTemplate.GetScalar("value");
            Assert.AreEqual(new QName("integer"), scalar.Key);
        }
        [Test]
        public void TestLoadTemplateWithUnicodeString()
        {
            const string templateXml = "<templates xmlns=\"http://www.fixprotocol.org/ns/template-definition\"" +
                                       "	ns=\"http://www.fixprotocol.org/ns/templates/sample\">" +
                                       "	<template name=\"SampleTemplate\">" +
                                       "		<string name=\"name\" charset=\"unicode\" presence=\"mandatory\"><copy /></string>" +
                                       "		<string name=\"id\" charset=\"unicode\" presence=\"optional\"><copy /></string>" +
                                       "		<string name=\"location\" charset=\"ascii\" presence=\"mandatory\"><copy /></string>" +
                                       "		<string name=\"id2\" charset=\"ascii\" presence=\"optional\"><copy /></string>" +
                                       "	</template>" + "</templates>";

            IMessageTemplateLoader loader = new XmlMessageTemplateLoader();
            MessageTemplate[] templates = loader.Load(new MemoryStream(
                        System.Text.Encoding.ASCII.GetBytes(templateXml)));
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
        public void TestLoadMdIncrementalRefreshTemplate()
        {
            var templateStream = new StreamReader("FPL/mdIncrementalRefreshTemplate.xml");
            IMessageTemplateLoader loader = new XmlMessageTemplateLoader();
            MessageTemplate messageTemplate = loader.Load(templateStream.BaseStream)[0];

            Assert.AreEqual("MDIncrementalRefresh", messageTemplate.TypeReference.Name);
            Assert.AreEqual("MDRefreshSample", messageTemplate.Name);
            Assert.AreEqual(10, messageTemplate.FieldCount);

            /********************************** TEMPLATE FIELDS **********************************/
            int index = 0;
            AssertScalarField(messageTemplate, index++, FASTType.U32, "templateId", OpenFAST.Template.Operator.Operator.Copy);
            AssertScalarField(messageTemplate, index++, FASTType.Ascii, "8", OpenFAST.Template.Operator.Operator.Constant);
            AssertScalarField(messageTemplate, index++, FASTType.U32, "9", OpenFAST.Template.Operator.Operator.Constant);
            AssertScalarField(messageTemplate, index++, FASTType.Ascii, "35", OpenFAST.Template.Operator.Operator.Constant);
            AssertScalarField(messageTemplate, index++, FASTType.Ascii, "49", OpenFAST.Template.Operator.Operator.Constant);
            AssertScalarField(messageTemplate, index++, FASTType.U32, "34", OpenFAST.Template.Operator.Operator.Increment);
            AssertScalarField(messageTemplate, index++, FASTType.Ascii, "52", OpenFAST.Template.Operator.Operator.Delta);
            AssertScalarField(messageTemplate, index++, FASTType.U32, "75", OpenFAST.Template.Operator.Operator.Copy);

            /************************************* SEQUENCE **************************************/
            AssertSequence(messageTemplate, index, 17);

            var sequence = (Sequence)messageTemplate.GetField(index++);
            Assert.AreEqual("MDEntries", sequence.TypeReference.Name);
            AssertSequenceLengthField(sequence, "268", FASTType.U32, OpenFAST.Template.Operator.Operator.None);

            /********************************** SEQUENCE FIELDS **********************************/
            int seqIndex = 0;
            AssertScalarField(sequence, seqIndex++, FASTType.Decimal, "270",
                OpenFAST.Template.Operator.Operator.Delta);
            AssertScalarField(sequence, seqIndex++, FASTType.I32, "271",
                OpenFAST.Template.Operator.Operator.Delta);
            AssertScalarField(sequence, seqIndex++, FASTType.U32, "273",
                OpenFAST.Template.Operator.Operator.Delta);
            AssertOptionalScalarField(sequence, seqIndex++, FASTType.U32,
                "346", OpenFAST.Template.Operator.Operator.None);
            AssertScalarField(sequence, seqIndex++, FASTType.U32, "1023",
                OpenFAST.Template.Operator.Operator.Increment);
            AssertScalarField(sequence, seqIndex++, FASTType.Ascii, "279",
                OpenFAST.Template.Operator.Operator.Copy);
            AssertScalarField(sequence, seqIndex++, FASTType.Ascii, "269",
                OpenFAST.Template.Operator.Operator.Copy);
            AssertScalarField(sequence, seqIndex++, FASTType.Ascii, "107",
                OpenFAST.Template.Operator.Operator.Copy);
            AssertScalarField(sequence, seqIndex++, FASTType.Ascii, "48",
                OpenFAST.Template.Operator.Operator.Delta);
            AssertScalarField(sequence, seqIndex++, FASTType.Ascii, "276",
                OpenFAST.Template.Operator.Operator.Copy);
            AssertScalarField(sequence, seqIndex++, FASTType.Ascii, "274",
                OpenFAST.Template.Operator.Operator.Copy);
            AssertScalarField(sequence, seqIndex++, FASTType.Decimal, "451",
                OpenFAST.Template.Operator.Operator.Copy);
            AssertScalarField(sequence, seqIndex++, FASTType.Ascii, "277",
                OpenFAST.Template.Operator.Operator.Default);
            AssertOptionalScalarField(sequence, seqIndex++, FASTType.U32,
                "1020", OpenFAST.Template.Operator.Operator.None);
            AssertScalarField(sequence, seqIndex++, FASTType.I32, "537",
                OpenFAST.Template.Operator.Operator.Default);
            AssertScalarField(sequence, seqIndex++, FASTType.Ascii, "1024",
                OpenFAST.Template.Operator.Operator.Default);
            AssertScalarField(sequence, seqIndex, FASTType.Ascii, "336",
                OpenFAST.Template.Operator.Operator.Default);

            AssertScalarField(messageTemplate, index, FASTType.Ascii, "10",
                OpenFAST.Template.Operator.Operator.None);
        }
        [Test]
        public void TestStaticTemplateReference()
        {
            const string templateXml = "<templates>" +
                                       "  <template name=\"t1\">" +
                                       "    <string name=\"string\"/>" +
                                       "  </template>" +
                                       "  <template name=\"t2\">" +
                                       "    <uInt32 name=\"quantity\"/>" +
                                       "    <templateRef name=\"t1\"/>" +
                                       "    <decimal name=\"price\"/>" +
                                       "  </template>" +
                                       "</templates>";
            MessageTemplate[] templates = new XmlMessageTemplateLoader().Load(new MemoryStream(System.Text.Encoding.ASCII.GetBytes(templateXml)));
            AssertEquals(4, templates[1].FieldCount);
            AssertScalarField(templates[1], 1, FASTType.U32, "quantity", OpenFAST.Template.Operator.Operator.None);
            AssertScalarField(templates[1], 2, FASTType.Ascii, "string", OpenFAST.Template.Operator.Operator.None);
            AssertScalarField(templates[1], 3, FASTType.Decimal, "price", OpenFAST.Template.Operator.Operator.None);
        }
        [Test]
        public void TestNonExistantTemplateReference()
        {
            const string template2Xml = "<template name=\"t2\">" +
                                        "  <uInt32 name=\"quantity\"/>" +
                                        "  <templateRef name=\"t1\"/>" +
                                        "  <decimal name=\"price\"/>" +
                                        "</template>";
            try
            {
                new XmlMessageTemplateLoader().Load(new MemoryStream(System.Text.Encoding.ASCII.GetBytes(template2Xml)));
            }
            catch (DynErrorException e)
            {
                Assert.AreEqual(DynError.TemplateNotExist, e.Error);
            }
        }

        [Test]
        public void TestReferencedTemplateInOtherLoader()
        {
            const string template1Xml = "<template name=\"t1\">" +
                                        "  <string name=\"string\"/>" +
                                        "</template>";
            const string template2Xml = "<template name=\"t2\">" +
                                        "  <uInt32 name=\"quantity\"/>" +
                                        "  <templateRef name=\"t1\"/>" +
                                        "  <decimal name=\"price\"/>" +
                                        "</template>";

            IMessageTemplateLoader loader1 = new XmlMessageTemplateLoader();
            IMessageTemplateLoader loader2 = new XmlMessageTemplateLoader {TemplateRegistry = loader1.TemplateRegistry};

            loader1.Load(new MemoryStream(System.Text.Encoding.ASCII.GetBytes(template1Xml)));
            MessageTemplate[] templates = loader2.Load(new MemoryStream(System.Text.Encoding.ASCII.GetBytes(template2Xml)));
            Assert.AreEqual(4, templates[0].FieldCount);
            AssertScalarField(templates[0], 1, FASTType.U32, "quantity", OpenFAST.Template.Operator.Operator.None);
            AssertScalarField(templates[0], 2, FASTType.Ascii, "string", OpenFAST.Template.Operator.Operator.None);
            AssertScalarField(templates[0], 3, FASTType.Decimal, "price", OpenFAST.Template.Operator.Operator.None);
        }
        [Test]
        public void TestTemplateReferencedFromPreviousLoad()
        {
            const string template1Xml = "<template name=\"t1\">" +
                                        "  <string name=\"string\"/>" +
                                        "</template>";
            const string template2Xml = "<template name=\"t2\">" +
                                        "  <uInt32 name=\"quantity\"/>" +
                                        "  <templateRef name=\"t1\"/>" +
                                        "  <decimal name=\"price\"/>" +
                                        "</template>";

            IMessageTemplateLoader loader = new XmlMessageTemplateLoader();
            loader.Load(new MemoryStream(System.Text.Encoding.ASCII.GetBytes(template1Xml)));
            MessageTemplate[] templates = loader.Load(new MemoryStream(System.Text.Encoding.ASCII.GetBytes(template2Xml)));

            Assert.AreEqual(4, templates[0].FieldCount);
            AssertScalarField(templates[0], 1, FASTType.U32, "quantity", OpenFAST.Template.Operator.Operator.None);
            AssertScalarField(templates[0], 2, FASTType.Ascii, "string", OpenFAST.Template.Operator.Operator.None);
            AssertScalarField(templates[0], 3, FASTType.Decimal, "price", OpenFAST.Template.Operator.Operator.None);
        }
        [Test]
        public void TestDynamicTemplateReference()
        {
            const string template1Xml = "<template name=\"t1\">" +
                                        "  <string name=\"string\"/>" +
                                        "</template>";
            const string template2Xml = "<template name=\"t2\">" +
                                        "  <uInt32 name=\"quantity\"/>" +
                                        "  <templateRef/>" +
                                        "  <decimal name=\"price\"/>" +
                                        "</template>";

            IMessageTemplateLoader loader = new XmlMessageTemplateLoader();
            loader.Load(new MemoryStream(System.Text.Encoding.ASCII.GetBytes(template1Xml)));
            MessageTemplate[] templates = loader.Load(new MemoryStream(System.Text.Encoding.ASCII.GetBytes(template2Xml)));

            Assert.AreEqual(4, templates[0].FieldCount);
            AssertScalarField(templates[0], 1, FASTType.U32, "quantity", OpenFAST.Template.Operator.Operator.None);
            AssertScalarField(templates[0], 3, FASTType.Decimal, "price", OpenFAST.Template.Operator.Operator.None);
            Assert.True(templates[0].GetField(2) is DynamicTemplateReference);
        }
        [Test]
        public void TestByteVector()
        {
            const string templateXml = "<template name=\"bvt\">" +
                                       "  <byteVector name=\"data\">" +
                                       "    <length name=\"dataLength\"/>" +
                                       "    <tail/>" +
                                       "  </byteVector>" +
                                       "</template>";
            IMessageTemplateLoader loader = new XmlMessageTemplateLoader();
            MessageTemplate bvt = loader.Load(new MemoryStream(System.Text.Encoding.ASCII.GetBytes(templateXml)))[0];

            AssertScalarField(bvt, 1, FASTType.ByteVector, "data", OpenFAST.Template.Operator.Operator.Tail);
        }
        [Test]
        public void TestNullDocument()
        {
            var loader = new XmlMessageTemplateLoader();
            loader.SetErrorHandler(ErrorHandlerFields.Null);
            Assert.AreEqual(0, loader.Load(null).Length);
        }

        //public void testCustomFieldParser() {
        //    string templateXml = 
        //        "<template name=\"custom\">" +
        //        "  <array name=\"intArr\" type=\"int\"></array>" +
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
