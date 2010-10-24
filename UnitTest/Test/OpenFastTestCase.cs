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
using System;
using System.IO;
using System.Text;
using System.Xml;
using NUnit.Framework;
using OpenFAST.Codec;
using OpenFAST.Template;
using OpenFAST.Template.Loader;
using OpenFAST.Template.Operator;
using OpenFAST.Template.Types;
using OpenFAST.Template.Types.Codec;

namespace OpenFAST.UnitTests.Test
{
    public abstract class OpenFastTestCase
    {
        protected static DecimalValue Decimal(double value)
        {
            return new DecimalValue(value);
        }

        protected static IntegerValue Int(int value)
        {
            return new IntegerValue(value);
        }

        protected static LongValue Long(long value)
        {
            return new LongValue(value);
        }

        protected static TwinValue Twin(ScalarValue first, ScalarValue second)
        {
            return new TwinValue(first, second);
        }

        protected static void AssertEquals(String bitString, byte[] encoding)
        {
            TestUtil.AssertBitVectorEquals(bitString, encoding);
        }

        protected static void AssertEncodeDecode(ScalarValue value, String bitString, TypeCodec type)
        {
            Assert.AreEqual(ByteUtil.ConvertBitStringToFastByteArray(bitString), type.Encode(value ?? ScalarValue.Null));
            Assert.AreEqual(value, type.Decode(ByteUtil.CreateByteStream(bitString)));
        }

        protected static Stream BitStream(String bitString)
        {
            return ByteUtil.CreateByteStream(bitString);
        }

        protected static Stream Stream(String source)
        {
            return new MemoryStream(Encoding.ASCII.GetBytes(source));
        }

        protected static Stream ByteStream(byte[] bytes)
        {
            return new MemoryStream(bytes);
        }

        protected static ByteVectorValue Byte(byte[] value)
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

        protected static byte[] Byte(String hexString)
        {
            return ByteUtil.ConvertHexStringToByteArray(hexString);
        }
        protected static ByteVectorValue ByteVector(String hexString)
        {
            return Byte(Byte(hexString));
        }

        protected static byte[] Bytes(String binaryString)
        {
            return ByteUtil.ConvertBitStringToFastByteArray(binaryString);
        }

        protected static DecimalValue Decimal(int mantissa, int exponent)
        {
            return new DecimalValue(mantissa, exponent);
        }

        protected static ScalarValue String(String value)
        {
            return new StringValue(value);
        }

        protected static MessageTemplate Template(String templateXml)
        {
            MessageTemplate[] templates = new XmlMessageTemplateLoader().Load
                (new MemoryStream(Encoding.ASCII.GetBytes(templateXml)));
            return templates[0];
        }

        protected static MessageTemplate Template(Field field)
        {
            return new MessageTemplate("Doesn't matter", new[] {field});
        }

        protected static void AssertScalarField(IFieldSet fieldSet, int fieldIndex, FASTType type, String name,
                                                OperatorCodec operatorCodec,
                                                ScalarValue defaultValue)
        {
            var field = (Scalar) fieldSet.GetField(fieldIndex);
            AssertScalarField(field, type, name);
            Assert.AreEqual(operatorCodec, field.OperatorCodec);
            Assert.AreEqual(defaultValue, field.DefaultValue);
        }

        protected static void AssertComposedScalarField(IFieldSet fieldSet, int fieldIndex, FASTType type, String name,
                                                        Operator exponentOp,
                                                        ScalarValue exponentValue, Operator mantissaOp,
                                                        ScalarValue mantissaValue)
        {
            var field = (ComposedScalar) fieldSet.GetField(fieldIndex);
            Assert.AreEqual(type, field.FASTType);
            Assert.AreEqual(name, field.Name);

            Scalar[] fields = field.Fields;
            Assert.AreEqual(exponentOp, fields[0].Operator);
            Assert.AreEqual(exponentValue, fields[0].DefaultValue);

            Assert.AreEqual(mantissaOp, fields[1].Operator);
            Assert.AreEqual(mantissaValue, fields[1].DefaultValue);
        }

        protected static void AssertComposedScalarField(ComposedScalar field, FASTType type, String name,
                                                        Operator exponentOp,
                                                        ScalarValue exponentValue, Operator mantissaOp,
                                                        ScalarValue mantissaValue)
        {
            Assert.AreEqual(type, field.FASTType);
            Assert.AreEqual(name, field.Name);
            Scalar[] fields = field.Fields;
            Assert.AreEqual(exponentOp, fields[0].Operator);
            Assert.AreEqual(exponentValue, fields[0].DefaultValue);

            Assert.AreEqual(mantissaOp, fields[1].Operator);
            Assert.AreEqual(mantissaValue, fields[1].DefaultValue);
        }

        protected static void AssertScalarField(IFieldSet fieldSet, int fieldIndex, FASTType type, String name, Operator op)
        {
            var field = (Scalar) fieldSet.GetField(fieldIndex);
            AssertScalarField(field, type, name);
            Assert.AreEqual(op, field.Operator);
        }

        protected static void AssertSequenceLengthField(Sequence sequence, String name, FASTType type, Operator op)
        {
            Assert.AreEqual(type, sequence.Length.FASTType);
            Assert.AreEqual(name, sequence.Length.Name);
            Assert.AreEqual(op, sequence.Length.Operator);
        }

        protected static void AssertSequence(MessageTemplate messageTemplate, int fieldIndex, int fieldCount)
        {
            var sequence = (Sequence) messageTemplate.Fields[fieldIndex];
            AssertEquals(fieldCount, sequence.FieldCount);
        }

        protected static void AssertGroup(MessageTemplate messageTemplate, int fieldIndex, String name)
        {
            var currentGroup = (Group) messageTemplate.Fields[fieldIndex];
            Assert.AreEqual(name, currentGroup.Name);
        }

        protected static void AssertOptionalScalarField(IFieldSet fieldSet, int fieldIndex, FASTType type, String name,
                                                        Operator op)
        {
            var field = (Scalar) fieldSet.GetField(fieldIndex);
            AssertScalarField(field, type, name);
            Assert.AreEqual(op, field.Operator);
            Assert.IsTrue(field.IsOptional);
        }

        private static void AssertScalarField(Scalar field, FASTType type, string name)
        {
            Assert.AreEqual(name, field.Name);
            Assert.AreEqual(type, field.FASTType);
        }

        protected static XmlDocument Document(String xml)
        {
            var doc = new XmlDocument();
            doc.LoadXml(xml);
            return doc;
        }

        protected static void AssertScalarField(Scalar scalar, FASTType type, String name, String id, String ns,
                                                String dictionary, String key, Operator op,
                                                ScalarValue defaultVal, bool optional)
        {
            AssertScalarField(scalar, type, name, id, ns, dictionary, key, ns, op, defaultVal, optional);
        }

        protected static void AssertScalarField(Scalar scalar, FASTType type, String name, String id, String ns,
                                                String dictionary, String key, String keyNamespace, Operator op,
                                                ScalarValue defaultVal, bool optional)
        {
            var qname = new QName(name, ns);
            Assert.AreEqual(type, scalar.FASTType);
            Assert.AreEqual(op, scalar.Operator);
            Assert.AreEqual(qname, scalar.QName);
            var keyName = new QName(key, keyNamespace);
            Assert.AreEqual(keyName, scalar.Key);
            if (id == null)
            {
                Assert.True(scalar.IsIdNull());
            }
            else
            {
                Assert.AreEqual(id, scalar.Id);
            }
            Assert.AreEqual(dictionary, scalar.Dictionary);
            Assert.AreEqual(defaultVal, scalar.DefaultValue);
            Assert.AreEqual(optional, scalar.IsOptional);
        }

        protected static void AssertEquals(decimal expected, decimal actual)
        {
            if (expected.CompareTo(actual) != 0)
                Assert.Fail("expected:<" + expected + "> bug was:<" + actual + ">");
        }
    }
}