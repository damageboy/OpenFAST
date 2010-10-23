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
using NUnit.Framework;
using OpenFAST;
using OpenFAST.Error;
using OpenFAST.Template;
using OpenFAST.Template.Type;
using UnitTest.Test;

namespace UnitTest.Template.Operator
{
    [TestFixture]
    public class DeltaByteVectorOperatorTest : OpenFastTestCase
    {
        private Scalar _field;
        [Test]
        public void TestDecodeSubtractionLengthError()
        {
            _field = new Scalar("", FASTType.ByteVector, OpenFAST.Template.Operator.Operator.Delta, ScalarValue.Undefined, false);
            try
            {
                Decode(Twin(Int(5), Byte(Byte("c0afcd"))), Byte(Byte("123456")));
                Assert.Fail();
            }
            catch (DynErrorException e)
            {
                Assert.AreEqual(DynError.D7SubtrctnLenLong, e.Error);
            }
        }
        [Test]
        public void TestGetValueToEncodeMandatory()
        {
            _field = new Scalar("", FASTType.ByteVector, OpenFAST.Template.Operator.Operator.Delta, ScalarValue.Undefined, false);
            Assert.AreEqual(Tv(0, Byte("aabbccdd")), Encode("aabbccdd", ScalarValue.Undefined));
            Assert.AreEqual(Tv(1, Byte("ee")), Encode("aabbccee", ByteVector("aabbccdd")));
            Assert.AreEqual(Tv(-2, Byte("ff")), Encode("ffbbccee", ByteVector("aabbccee")));
            Assert.AreEqual(Tv(-1, Byte("11")), Encode("11ffbbccee", ByteVector("ffbbccee")));
            Assert.AreEqual(Tv(0, Byte("ff")), Encode("11ffbbcceeff", ByteVector("11ffbbccee")));
        }
        [Test]
        public void TestDecodeValueMandatory()
        {
            _field = new Scalar("", FASTType.ByteVector, OpenFAST.Template.Operator.Operator.Delta, ScalarValue.Undefined, false);
            Assert.AreEqual(ByteVector("aabbccdd"), Decode(Tv(0, Byte("aabbccdd")), ScalarValue.Undefined));
            Assert.AreEqual(ByteVector("aabbccee"), Decode(Tv(1, Byte("ee")), ByteVector("aabbccdd")));
            Assert.AreEqual(ByteVector("ffbbccee"), Decode(Tv(-2, Byte("ff")), ByteVector("aabbccee")));
            Assert.AreEqual(ByteVector("11ffbbccee"), Decode(Tv(-1, Byte("11")), ByteVector("ffbbccee")));
            Assert.AreEqual(ByteVector("11ffbbcceeff"), Decode(Tv(0, Byte("ff")), ByteVector("11ffbbccee")));
        }
        [Test]
        public void TestGetValueToEncodeOptional()
        {
            _field = new Scalar("", FASTType.ByteVector, OpenFAST.Template.Operator.Operator.Delta, ScalarValue.Undefined, true);
            Assert.AreEqual(Tv(0, Byte("aabbccdd")), Encode("aabbccdd", ScalarValue.Undefined));
            Assert.AreEqual(Tv(1, Byte("ee")), Encode("aabbccee", ByteVector("aabbccdd")));
            Assert.AreEqual(Tv(-2, Byte("ff")), Encode("ffbbccee", ByteVector("aabbccee")));
            Assert.AreEqual(Tv(-1, Byte("11")), Encode("11ffbbccee", ByteVector("ffbbccee")));
            Assert.AreEqual(Tv(0, Byte("ff")), Encode("11ffbbcceeff", ByteVector("11ffbbccee")));
            Assert.AreEqual(ScalarValue.Null, Encode(null, ByteVector("11ffbbcceeff")));
        }
        [Test]
        public void TestDecodeValueOptional()
        {
            _field = new Scalar("", FASTType.ByteVector, OpenFAST.Template.Operator.Operator.Delta, ScalarValue.Undefined, true);
            Assert.AreEqual(ByteVector("aabbccdd"), Decode(Tv(0, Byte("aabbccdd")), ScalarValue.Undefined));
            Assert.AreEqual(ByteVector("aabbccee"), Decode(Tv(1, Byte("ee")), ByteVector("aabbccdd")));
            Assert.AreEqual(ByteVector("ffbbccee"), Decode(Tv(-2, Byte("ff")), ByteVector("aabbccee")));
            Assert.AreEqual(ByteVector("11ffbbccee"), Decode(Tv(-1, Byte("11")), ByteVector("ffbbccee")));
            Assert.AreEqual(ByteVector("11ffbbcceeff"), Decode(Tv(0, Byte("ff")), ByteVector("11ffbbccee")));
            Assert.AreEqual(null, Decode(ScalarValue.Null, String("11ffbbccee")));
        }

        private ScalarValue Encode(String value, ScalarValue priorValue)
        {
            if (value == null)
            {
                return OpenFAST.Template.Operator.OperatorCodec.GetCodec(OpenFAST.Template.Operator.Operator.Delta, FASTType.String).GetValueToEncode(null, priorValue, _field);
            }
            return OpenFAST.Template.Operator.OperatorCodec.GetCodec(OpenFAST.Template.Operator.Operator.Delta, FASTType.String).GetValueToEncode(ByteVector(value), priorValue, _field);
        }

        private ScalarValue Decode(ScalarValue diff, ScalarValue priorValue)
        {
            return OpenFAST.Template.Operator.OperatorCodec.GetCodec(OpenFAST.Template.Operator.Operator.Delta, FASTType.String).DecodeValue(diff, priorValue, _field);
        }

        private static TwinValue Tv(int subtraction, byte[] bytes)
        {
            return new TwinValue(new IntegerValue(subtraction), new ByteVectorValue(bytes));
        }
    }
}
