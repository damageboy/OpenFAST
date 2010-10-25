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
using OpenFAST.Error;
using OpenFAST.Template;
using OpenFAST.Template.Operator;
using OpenFAST.Template.Types;
using OpenFAST.Template.Types.Codec;
using OpenFAST.UnitTests.Test;

namespace OpenFAST.UnitTests.Template.Operator
{
    [TestFixture]
    public class DeltaDecimalOperatorTest : OpenFastTestCase
    {
        [Test]
        public void TestGetValueToEncodeForMandatory()
        {
            var field = new Scalar("", FASTType.Decimal, OpenFAST.Template.Operator.Operator.Delta, ScalarValue.Undefined, false);
            OperatorCodec operatortemp = field.OperatorCodec;

            var value = (DecimalValue)operatortemp.GetValueToEncode(Decimal(9427.55), ScalarValue.Undefined, field);
            Assert.AreEqual(new decimal(9427.55), value.ToBigDecimal());

            value = (DecimalValue)operatortemp.GetValueToEncode(Decimal(9427.51), Decimal(9427.55), field);
            Assert.AreEqual(-4, value.Mantissa);
            Assert.AreEqual(0, value.Exponent);

            value = (DecimalValue)operatortemp.GetValueToEncode(Decimal(9427.46), Decimal(9427.51), field);
            Assert.AreEqual(-5, value.Mantissa);
            Assert.AreEqual(0, value.Exponent);

            value = (DecimalValue)operatortemp.GetValueToEncode(Decimal(30.6), Decimal(30.6), field);
            Assert.AreEqual(0, value.Exponent);
            Assert.AreEqual(0, value.Mantissa);
        }
        [Test]
        public void TestGetValueToEncodeForOptional()
        {
            var field = new Scalar("", FASTType.Decimal, OpenFAST.Template.Operator.Operator.Delta, ScalarValue.Undefined, true);
            OperatorCodec operatortemp = field.OperatorCodec;

            var value = (DecimalValue)operatortemp.GetValueToEncode(Decimal(9427.55),
                    ScalarValue.Undefined, field);
            Assert.AreEqual(new decimal(9427.55), value.ToBigDecimal());

            value = (DecimalValue)operatortemp.GetValueToEncode(Decimal(9427.51),
                    Decimal(9427.55), field);
            Assert.AreEqual(-4, value.Mantissa);
            Assert.AreEqual(0, value.Exponent);

            value = (DecimalValue)operatortemp.GetValueToEncode(Decimal(9427.46),
                    Decimal(9427.51), field);
            Assert.AreEqual(-5, value.Mantissa);
            Assert.AreEqual(0, value.Exponent);

            value = (DecimalValue)operatortemp.GetValueToEncode(Decimal(30.6), Decimal(30.6), field);
            Assert.AreEqual(0, value.Exponent);
            Assert.AreEqual(0, value.Mantissa);

            Assert.AreEqual(ScalarValue.Null,
                operatortemp.GetValueToEncode(null, Decimal(30.6), field));
        }
        [Test]
        public void TestGetValueToEncodeForMandatoryFieldAndDefaultValue()
        {
            var field = new Scalar("", FASTType.Decimal, OpenFAST.Template.Operator.Operator.Delta, Decimal(12000),
                    false);
            var value = (DecimalValue)field.OperatorCodec
                                                     .GetValueToEncode(Decimal(12000),
                    ScalarValue.Undefined, field);
            Assert.AreEqual(0, value.Mantissa);
            Assert.AreEqual(0, value.Exponent);

            value = (DecimalValue)field.OperatorCodec
                                        .GetValueToEncode(Decimal(12100), Decimal(12000), field);
            Assert.AreEqual(109, value.Mantissa);
            Assert.AreEqual(-1, value.Exponent);

            value = (DecimalValue)field.OperatorCodec
                                        .GetValueToEncode(Decimal(12150), Decimal(12100), field);
            Assert.AreEqual(1094, value.Mantissa);
            Assert.AreEqual(-1, value.Exponent);

            value = (DecimalValue)field.OperatorCodec
                                        .GetValueToEncode(Decimal(12200), Decimal(12150), field);
            Assert.AreEqual(-1093, value.Mantissa);
            Assert.AreEqual(1, value.Exponent);
        }
        [Test]
        public void TestDecodeForMandatoryFieldAndDefaultValue()
        {
            var field = new Scalar("", FASTType.Decimal, OpenFAST.Template.Operator.Operator.Delta, Decimal(12000),
                    false);
            Assert.AreEqual(Decimal(12000),
                OperatorCodec.GetCodec(OpenFAST.Template.Operator.Operator.Delta, FASTType.Decimal).DecodeEmptyValue(ScalarValue.Undefined, field));
            Assert.AreEqual(Decimal(12100),
                OperatorCodec.GetCodec(OpenFAST.Template.Operator.Operator.Delta, FASTType.Decimal).DecodeValue(Decimal(109, -1), Decimal(12000), field));
            Assert.AreEqual(Decimal(12150),
                OperatorCodec.GetCodec(OpenFAST.Template.Operator.Operator.Delta, FASTType.Decimal).DecodeValue(Decimal(1094, -1), Decimal(12100), field));
            Assert.AreEqual(Decimal(12200),
                OperatorCodec.GetCodec(OpenFAST.Template.Operator.Operator.Delta, FASTType.Decimal).DecodeValue(Decimal(-1093, 1), Decimal(12150), field));
        }
        [Test]
        public void TestEncodeDecimalValueWithEmptyPriorValue()
        {
            try
            {
                var field = new Scalar("", FASTType.Decimal, OpenFAST.Template.Operator.Operator.Delta, ScalarValue.Undefined, false);
                field.OperatorCodec
                     .GetValueToEncode(null, ScalarValue.Undefined, field);
                Assert.Fail();
            }
            catch (DynErrorException e)
            {
                Assert.AreEqual(DynError.MandatoryFieldNotPresent, e.Error);
            }
        }

        [Test]
        public void TestEncodeDecimalValueWithOptionalField()
        {
            AssertEncodeDecode(Decimal(-37.0), "10000001 11011011", TypeCodec.NullableSfScaledNumber);
        }
    }
}
