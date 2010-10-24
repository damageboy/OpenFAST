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

namespace OpenFAST.UnitTests.Template.Operator
{
    [TestFixture]
    public class OperatorTest
    {
        [Test]
        public void TestDefaultOperator()
        {
            var field = new Scalar("operatorName", FASTType.U32, OpenFAST.Template.Operator.Operator.Default, new IntegerValue(1), false);
            Assert.AreEqual(null, field.OperatorCodec.GetValueToEncode(new IntegerValue(1), null, field));
            //		newly added implementation
            Assert.AreEqual(new IntegerValue(2), field.OperatorCodec.GetValueToEncode(new IntegerValue(2), null, field));
        }
        [Test]
        public void TestCopyOperator()
        {
            var field = new Scalar("", FASTType.U32, OpenFAST.Template.Operator.Operator.Copy, ScalarValue.Undefined, true);
            OperatorCodec copy = OpenFAST.Template.Operator.Operator.Copy.GetCodec(FASTType.U32);
            Assert.AreEqual(new IntegerValue(1), copy.GetValueToEncode(new IntegerValue(1), null, field));
            Assert.AreEqual(new IntegerValue(2), copy.GetValueToEncode(new IntegerValue(2), new IntegerValue(1), field));
            //newly added implementation
            Assert.AreEqual(null, copy.GetValueToEncode(ScalarValue.Null, ScalarValue.Null, field));
        }
        //[Test]
        //public void TestCopyOperatorWithOptionalPresence()
        //{
        //    OperatorCodec copy = OperatorCodec.COPY_ALL;
        //    Scalar field = new Scalar("", FASTType.U32, Operator.COPY, ScalarValue.UNDEFINED, true);
        //    assertEquals(null, copy.getValueToEncode(null, ScalarValue.UNDEFINED, field));
        //    //newly added implementation	
        //    Scalar field1 = new Scalar("", FASTType.U32, Operator.COPY, ScalarValue.UNDEFINED, true);
        //    assertEquals(null, copy.decodeEmptyValue(ScalarValue.UNDEFINED, field1));
        //}
        [Test]
        public void TestIncrementOperatorWithNoDefaultValue()
        {
            var field = new Scalar("", FASTType.U32, OpenFAST.Template.Operator.Operator.Increment, ScalarValue.Undefined, false);
            Assert.AreEqual(new IntegerValue(1), OperatorCodec.GetCodec(OpenFAST.Template.Operator.Operator.Increment, FASTType.I32).GetValueToEncode(new IntegerValue(1), null, field));
            Assert.AreEqual(null, OperatorCodec.GetCodec(OpenFAST.Template.Operator.Operator.Increment, FASTType.I32).GetValueToEncode(new IntegerValue(2), new IntegerValue(1), field));
        }

        [Test]
        public void TestIncrementOperatorWithDefaultValue()
        {
            var field = new Scalar("", FASTType.U32, OpenFAST.Template.Operator.Operator.Increment, new IntegerValue(1), false);
            Assert.AreEqual(null, OperatorCodec.GetCodec(OpenFAST.Template.Operator.Operator.Increment, FASTType.I32).GetValueToEncode(new IntegerValue(1), ScalarValue.Undefined, field));
            Assert.AreEqual(null, OperatorCodec.GetCodec(OpenFAST.Template.Operator.Operator.Increment, FASTType.I32).GetValueToEncode(new IntegerValue(2), new IntegerValue(1), field));
            Assert.AreEqual(new IntegerValue(3), OperatorCodec.GetCodec(OpenFAST.Template.Operator.Operator.Increment, FASTType.I32).GetValueToEncode(new IntegerValue(3), new IntegerValue(1), field));
            Assert.AreEqual(new IntegerValue(3), OperatorCodec.GetCodec(OpenFAST.Template.Operator.Operator.Increment, FASTType.I32).GetValueToEncode(new IntegerValue(3), null, field));
        }
        //[Test]
        //public void TestConstantValueOperator()
        //{
        //    Scalar field = new Scalar("", FASTType.ASCII, OpenFAST.Template.Operator.Operator.CONSTANT, new StringValue("5"), false);
        //    Assert.AreEqual(null, OperatorCodec.CONSTANT_ALL.getValueToEncode(null, null, field, new BitVectorBuilder(1)));
        //    Scalar field1 = new Scalar("", FASTType.ASCII, OpenFAST.Template.Operator.Operator.CONSTANT, new StringValue("99"), false);
        //    Assert.AreEqual(null, OperatorCodec.CONSTANT_ALL.getValueToEncode(null, null, field1, new BitVectorBuilder(1)));
        //    //newly added implementation
        //    Scalar field2 = new Scalar("", FASTType.ASCII, OpenFAST.Template.Operator.Operator.CONSTANT, new StringValue("4"), true);
        //    Assert.AreEqual(null, OperatorCodec.CONSTANT_ALL.decodeEmptyValue(new StringValue("4"), field2));
        //}
        [Test]
        public void TestDeltaValueOperatorForEncodingIntegerValue()
        {
            var field = new Scalar("", FASTType.I32, OpenFAST.Template.Operator.Operator.Delta, ScalarValue.Undefined, false);
            Assert.AreEqual(new IntegerValue(15), field.GetOperatorCodec().GetValueToEncode(new IntegerValue(45), new IntegerValue(30), field));
            Assert.AreEqual(new IntegerValue(-15), field.GetOperatorCodec().GetValueToEncode(new IntegerValue(30), new IntegerValue(45), field));
            field = new Scalar("", FASTType.I32, OpenFAST.Template.Operator.Operator.Delta, new IntegerValue(25), false);
            Assert.AreEqual(new IntegerValue(5), field.GetOperatorCodec().GetValueToEncode(new IntegerValue(30), ScalarValue.Undefined, field));
        }
        [Test]
        public void TestDeltaValueOperatorForDecodingIntegerValue()
        {
            Assert.AreEqual(new IntegerValue(45), OperatorCodec.GetCodec(OpenFAST.Template.Operator.Operator.Delta, FASTType.I32).DecodeValue(new IntegerValue(15), new IntegerValue(30), null));
            Assert.AreEqual(new IntegerValue(30), OperatorCodec.GetCodec(OpenFAST.Template.Operator.Operator.Delta, FASTType.I32).DecodeValue(new IntegerValue(-15), new IntegerValue(45), null));
            var field = new Scalar("", FASTType.I32, OpenFAST.Template.Operator.Operator.Delta, new IntegerValue(25), false);
            Assert.AreEqual(new IntegerValue(30), OperatorCodec.GetCodec(OpenFAST.Template.Operator.Operator.Delta, FASTType.I32).DecodeValue(new IntegerValue(5), ScalarValue.Undefined, field));
            var field2 = new Scalar("", FASTType.I32, OpenFAST.Template.Operator.Operator.Delta, new IntegerValue(25), false);
            Assert.AreEqual(new IntegerValue(25), OperatorCodec.GetCodec(OpenFAST.Template.Operator.Operator.Delta, FASTType.I32).DecodeEmptyValue(ScalarValue.Undefined, field2));
            Assert.AreEqual(new IntegerValue(5), OperatorCodec.GetCodec(OpenFAST.Template.Operator.Operator.Delta, FASTType.I32).DecodeEmptyValue(new IntegerValue(5), field));
            var field1 = new Scalar("", FASTType.I32, OpenFAST.Template.Operator.Operator.Delta, ScalarValue.Undefined, true);
            Assert.AreEqual(ScalarValue.Undefined, OperatorCodec.GetCodec(OpenFAST.Template.Operator.Operator.Delta, FASTType.I32).DecodeEmptyValue(ScalarValue.Undefined, field1));
        }
        [Test]
        public void TestDeltaValueOperatorForEncodingIntegerValueWithEmptyPriorValue()
        {
            try
            {
                var field = new Scalar("", FASTType.I32, OpenFAST.Template.Operator.Operator.Delta, new IntegerValue(25), false);
                field.GetOperatorCodec().GetValueToEncode(new IntegerValue(30), null, field);
                Assert.Fail();
            }
            catch (DynErrorException e)
            {
                Assert.AreEqual(DynError.MandatoryFieldNotPresent, e.Error);
            }
        }
        [Test]
        public void TestDeltaValueOperatorForDecodingIntegerValueWithEmptyPriorValue()
        {
            try
            {
                var field = new Scalar("", FASTType.U32, OpenFAST.Template.Operator.Operator.Delta, new IntegerValue(25), false);
                OperatorCodec.GetCodec(OpenFAST.Template.Operator.Operator.Delta, FASTType.I32).DecodeValue(new IntegerValue(30), null, field);
                //newly added implementation
                var field1 = new Scalar("", FASTType.U32, OpenFAST.Template.Operator.Operator.Delta, ScalarValue.Undefined, false);
                Assert.AreEqual(ScalarValue.Undefined, OperatorCodec.GetCodec(OpenFAST.Template.Operator.Operator.Delta, FASTType.I32).DecodeEmptyValue(ScalarValue.Undefined, field1));
                Assert.Fail();
            }
            catch (DynErrorException e)
            {
                Assert.AreEqual(DynError.MandatoryFieldNotPresent, e.Error);
            }
        }
        [Test]
        public void TestDeltaOperatorForOptionalUnsignedInteger()
        {
            var field = new Scalar("", FASTType.U32, OpenFAST.Template.Operator.Operator.Delta, ScalarValue.Undefined, true);
            OperatorCodec delta = field.GetOperatorCodec();
            Assert.AreEqual(ScalarValue.Null, delta.GetValueToEncode(null, ScalarValue.Undefined, field));
        }
        [Test]
        public void TestIncompatibleOperatorAndTypeError()
        {
            try
            {
                OpenFAST.Template.Operator.Operator.Increment.GetCodec(FASTType.String);
                Assert.Fail();
            }
            catch (StatErrorException e)
            {
                Assert.AreEqual(StaticError.OperatorTypeIncomp, e.Error);
                Assert.AreEqual("The operator 'increment' is not compatible with type 'string'", e.Message);
            }
        }
    }
}
