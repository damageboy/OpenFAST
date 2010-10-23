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
using NUnit.Framework;
using OpenFAST;
using OpenFAST.Error;
using OpenFAST.Template;
using OpenFAST.Template.Type;
using UnitTest.Test;

namespace UnitTest.Template.Operator
{
    [TestFixture]
    public class DeltaStringOperatorTest : OpenFastTestCase
    {
        private Scalar _field;

        [Test]
        public void TestDecodeSubtractionLengthError()
        {
            _field = new Scalar("", FASTType.Ascii, OpenFAST.Template.Operator.Operator.Delta, ScalarValue.Undefined, false);
            try
            {
                Decode(Twin(Int(5), new StringValue("abc")), new StringValue("def"));
                Assert.Fail();
            }
            catch (DynErrorException e)
            {
                Assert.AreEqual(DynError.D7SubtrctnLenLong, e.Error);
                Assert.AreEqual(
                        "The string diff <5, abc> cannot be applied to the base value 'def' because the subtraction length is too long.",
                        e.Message);
            }
        }

        [Test]
        public void TestGetValueToEncodeMandatory()
        {
            _field = new Scalar("", FASTType.Ascii, OpenFAST.Template.Operator.Operator.Delta, ScalarValue.Undefined, false);
            Assert.AreEqual(Tv(0, "ABCD"), Encode("ABCD", ScalarValue.Undefined));
            Assert.AreEqual(Tv(1, "E"), Encode("ABCE", new StringValue("ABCD")));
            Assert.AreEqual(Tv(-2, "Z"), Encode("ZBCE", new StringValue("ABCE")));
            Assert.AreEqual(Tv(-1, "Y"), Encode("YZBCE", new StringValue("ZBCE")));
            Assert.AreEqual(Tv(0, "F"), Encode("YZBCEF", new StringValue("YZBCE")));
        }

        [Test]
        public void TestDecodeValueMandatory()
        {
            _field = new Scalar("", FASTType.Ascii, OpenFAST.Template.Operator.Operator.Delta, ScalarValue.Undefined, false);
            Assert.AreEqual(new StringValue("ABCD"), Decode(Tv(0, "ABCD"), ScalarValue.Undefined));
            Assert.AreEqual(new StringValue("ABCE"), Decode(Tv(1, "E"), new StringValue("ABCD")));
            Assert.AreEqual(new StringValue("ZBCE"), Decode(Tv(-2, "Z"), new StringValue("ABCE")));
            Assert.AreEqual(new StringValue("YZBCE"), Decode(Tv(-1, "Y"), new StringValue("ZBCE")));
            Assert.AreEqual(new StringValue("YZBCEF"), Decode(Tv(0, "F"), new StringValue("YZBCE")));
        }

        [Test]
        public void TestGetValueToEncodeOptional()
        {
            _field = new Scalar("", FASTType.Ascii, OpenFAST.Template.Operator.Operator.Delta, ScalarValue.Undefined, true);
            Assert.AreEqual(Tv(0, "ABCD"), Encode("ABCD", ScalarValue.Undefined));
            Assert.AreEqual(Tv(1, "E"), Encode("ABCE", new StringValue("ABCD")));
            Assert.AreEqual(Tv(-2, "Z"), Encode("ZBCE", new StringValue("ABCE")));
            Assert.AreEqual(Tv(-1, "Y"), Encode("YZBCE", new StringValue("ZBCE")));
            Assert.AreEqual(Tv(0, "F"), Encode("YZBCEF", new StringValue("YZBCE")));
            Assert.AreEqual(ScalarValue.Null, Encode(null, new StringValue("YZBCEF")));
        }

        [Test]
        public void TestDecodeValueOptional()
        {
            _field = new Scalar("", FASTType.Ascii, OpenFAST.Template.Operator.Operator.Delta, ScalarValue.Undefined, true);
            Assert.AreEqual(new StringValue("ABCD"), Decode(Tv(0, "ABCD"), ScalarValue.Undefined));
            Assert.AreEqual(new StringValue("ABCE"), Decode(Tv(1, "E"), new StringValue("ABCD")));
            Assert.AreEqual(new StringValue("ZBCE"), Decode(Tv(-2, "Z"), new StringValue("ABCE")));
            Assert.AreEqual(new StringValue("YZBCE"), Decode(Tv(-1, "Y"), new StringValue("ZBCE")));
            Assert.AreEqual(new StringValue("YZBCEF"), Decode(Tv(0, "F"), new StringValue("YZBCE")));
            Assert.AreEqual(null, Decode(ScalarValue.Null, new StringValue("YZBCEF")));
        }

        private ScalarValue Encode(string value, ScalarValue priorValue)
        {
            if (value == null)
            {
                return OpenFAST.Template.Operator.OperatorCodec.GetCodec(OpenFAST.Template.Operator.Operator.Delta, FASTType.String).GetValueToEncode(null, priorValue, _field);
            }
            return OpenFAST.Template.Operator.OperatorCodec.GetCodec(OpenFAST.Template.Operator.Operator.Delta, FASTType.String).GetValueToEncode(new StringValue(value), priorValue, _field);
        }

        private ScalarValue Decode(ScalarValue diff, ScalarValue priorValue)
        {
            return OpenFAST.Template.Operator.OperatorCodec.GetCodec(OpenFAST.Template.Operator.Operator.Delta, FASTType.String).DecodeValue(diff, priorValue, _field);
        }

        private static TwinValue Tv(int subtraction, string diff)
        {
            return new TwinValue(new IntegerValue(subtraction), Str2Bv(diff));
        }

        private static ByteVectorValue Str2Bv(string str)
        {
            return new ByteVectorValue(System.Text.Encoding.ASCII.GetBytes(str));
        }

    }
}
