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

*/
using System;
using OpenFAST;
using OpenFAST.Template;
using System.Xml;
using System.IO;
using NUnit.Framework;
using OpenFAST.Template.Type.Codec;
using OpenFAST.Codec;
using OpenFAST.Template.Loader;
using OpenFAST.Template.Operator;

namespace UnitTest.Test
{
    public abstract class OpenFastTestCase
    {
        protected static readonly ScalarValue NULL = ScalarValue.NULL;

        protected static readonly ScalarValue UNDEF = ScalarValue.UNDEFINED;

        public static DecimalValue d(double value)
        {
            return new DecimalValue(value);
        }

        protected static IntegerValue i(int value)
        {
            return new IntegerValue(value);
        }

        protected static LongValue l(long value)
        {
            return new LongValue(value);
        }

        protected static TwinValue twin(ScalarValue first, ScalarValue second)
        {
            return new TwinValue(first, second);
        }

        protected static void AssertEquals(String bitString, byte[] encoding)
        {
            TestUtil.AssertBitVectorEquals(bitString, encoding);
        }

        protected static void AssertEncodeDecode(ScalarValue value, String bitString, TypeCodec type)
        {
            Assert.AreEqual(bitString, type.Encode(value ?? ScalarValue.NULL));
            Assert.AreEqual(value, type.Decode(ByteUtil.CreateByteStream(bitString)));
        }

        protected static Stream BitStream(String bitString)
        {
            return ByteUtil.CreateByteStream(bitString);
        }

        protected static Stream Stream(String source)
        {
            return new MemoryStream(System.Text.Encoding.ASCII.GetBytes(source));
        }

        protected static Stream ByteStream(byte[] bytes)
        {
            return new MemoryStream(bytes);
        }

        protected static ByteVectorValue byt(byte[] value)
        {
            return new ByteVectorValue(value);
        }

        protected static FastDecoder Decoder(String bitString, MessageTemplate template)
        {
            var context = new Context();
            context.RegisterTemplate(1, template);
            return new FastDecoder(context, BitStream(bitString));
        }

        protected static FastDecoder Decoder(MessageTemplate template, byte[] encoding)
        {
            var context = new Context();
            context.RegisterTemplate(1, template);
            return new FastDecoder(context, new MemoryStream(encoding));
        }

        protected static FastEncoder Encoder(MessageTemplate template)
        {
            var context = new Context();
            context.RegisterTemplate(1, template);
            return new FastEncoder(context);
        }

        protected static DateTime Date(int year, int month, int day)
        {
            return new DateTime(year, month, day);
        }

        protected static DateTime Time(int hour, int min, int sec, int ms)
        {
            return new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, hour, min, sec, ms);


        }

        protected static byte[] byt(String hexString)
        {
            return ByteUtil.ConvertHexStringToByteArray(hexString);
        }

        protected static byte[] bytes(String binaryString)
        {
            return ByteUtil.ConvertBitStringToFastByteArray(binaryString);
        }

        protected static DecimalValue d(int mantissa, int exponent)
        {
            return new DecimalValue(mantissa, exponent);
        }

        protected static ScalarValue String(String value)
        {
            return new StringValue(value);
        }

        protected static MessageTemplate Template(String templateXml)
        {
            MessageTemplate[] templates = new XMLMessageTemplateLoader().Load
            (new MemoryStream(System.Text.Encoding.ASCII.GetBytes(templateXml)));
            return templates[0];
        }

        protected static MessageTemplate Template(Field field)
        {
            return new MessageTemplate("Doesn't matter", new[] { field });
        }

        protected static void AssertScalarField(FieldSet fieldSet, int fieldIndex, Type type, String name, OperatorCodec operator_ren,
                ScalarValue defaultValue)
        {
            var field = (Scalar)fieldSet.GetField(fieldIndex);
            AssertScalarField(field, type, name);
            Assert.AreEqual(operator_ren, field.OperatorCodec);
            Assert.AreEqual(defaultValue, field.DefaultValue);
        }

        protected static void AssertComposedScalarField(FieldSet fieldSet, int fieldIndex, Type type, String name, Operator exponentOp,
                ScalarValue exponentValue, Operator mantissaOp, ScalarValue mantissaValue)
        {
            var field = (ComposedScalar)fieldSet.GetField(fieldIndex);
            Assert.AreEqual(type, field.Type);
            Assert.AreEqual(name, field.Name);
            Scalar[] fields = field.Fields;
            Assert.AreEqual(exponentOp, fields[0].Operator);
            Assert.AreEqual(exponentValue, fields[0].DefaultValue);

            Assert.AreEqual(mantissaOp, fields[1].Operator);
            Assert.AreEqual(mantissaValue, fields[1].DefaultValue);
        }

        protected static void AssertComposedScalarField(ComposedScalar field, Type type, String name, Operator exponentOp,
                ScalarValue exponentValue, Operator mantissaOp, ScalarValue mantissaValue)
        {

            Assert.AreEqual(type, field.Type);
            Assert.AreEqual(name, field.Name);
            Scalar[] fields = field.Fields;
            Assert.AreEqual(exponentOp, fields[0].Operator);
            Assert.AreEqual(exponentValue, fields[0].DefaultValue);

            Assert.AreEqual(mantissaOp, fields[1].Operator);
            Assert.AreEqual(mantissaValue, fields[1].DefaultValue);
        }

        protected static void AssertScalarField(FieldSet fieldSet, int fieldIndex, Type type, String name, Operator operator_ren)
        {
            var field = (Scalar)fieldSet.GetField(fieldIndex);
            AssertScalarField(field, type, name);
            Assert.AreEqual(operator_ren, field.Operator);
        }

        protected static void AssertSequenceLengthField(Sequence sequence, String name, Type type, Operator operator_ren)
        {
            Assert.AreEqual(type, sequence.Length.Type);
            Assert.AreEqual(name, sequence.Length.Name);
            Assert.AreEqual(operator_ren, sequence.Length.Operator);
        }

        protected static void AssertSequence(MessageTemplate messageTemplate, int fieldIndex, int fieldCount)
        {
            var sequence = (Sequence)messageTemplate.GetField(fieldIndex);
            AssertEquals(fieldCount, sequence.FieldCount);
        }

        protected static void AssertGroup(MessageTemplate messageTemplate, int fieldIndex, String name)
        {
            var currentGroup = (Group)messageTemplate.GetField(fieldIndex);
            Assert.AreEqual(name, currentGroup.Name);
        }

        protected static void AssertOptionalScalarField(FieldSet fieldSet, int fieldIndex, Type type, String name, Operator operator_ren)
        {
            var field = (Scalar)fieldSet.GetField(fieldIndex);
            AssertScalarField(field, type, name);
            Assert.AreEqual(operator_ren, field.Operator);
            Assert.IsTrue(field.Optional);
        }

        private static void AssertScalarField(Scalar field, Type type, String name)
        {
            Assert.AreEqual(name, field.Name);
            Assert.AreEqual(type, field.Type);
        }

        protected static XmlDocument Document(String xml)
        {
            var doc = new XmlDocument();
            doc.LoadXml(xml);
            return doc;
        }

        protected static void AssertScalarField(Scalar scalar, Type type, String name, String id, String namespace_ren, String dictionary,
                String key, Operator op, ScalarValue defaultVal, bool optional)
        {
            AssertScalarField(scalar, type, name, id, namespace_ren, dictionary, key, namespace_ren, op, defaultVal, optional);
        }

        protected static void AssertScalarField(Scalar scalar, Type type, String name, String id, String namespace_ren, String dictionary,
                String key, String keyNamespace, Operator op, ScalarValue defaultVal, bool optional)
        {
            var qname = new QName(name, namespace_ren);
            Assert.AreEqual(type, scalar.Type);
            Assert.AreEqual(op, scalar.Operator);
            Assert.AreEqual(qname, scalar.QName);
            var keyName = new QName(key, keyNamespace);
            Assert.AreEqual(keyName, scalar.Key);
            Assert.AreEqual(id, scalar.Id);
            Assert.AreEqual(dictionary, scalar.Dictionary);
            Assert.AreEqual(defaultVal, scalar.DefaultValue);
            Assert.AreEqual(optional, scalar.Optional);
        }

        protected static void AssertEquals(decimal expected, decimal actual)
        {
            if (expected.CompareTo(actual) != 0)
                Assert.Fail("expected:<" + expected + "> bug was:<" + actual + ">");
        }
    }
}
