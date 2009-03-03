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
using System.Collections.Generic;
using System.Text;
using UnitTest.Test;
using NUnit.Framework;
using OpenFAST.Error;
using OpenFAST;

namespace UnitTest
{
    [TestFixture]
    public class DecimalValueTest : OpenFastTestCase
    {
        [Test]
        public void TestToBigDecimal()
        {
            AssertEquals(new Decimal(241e5), new DecimalValue(241, 5).ToBigDecimal());
            AssertEquals(new Decimal(15e-4), new DecimalValue(15, -4).ToBigDecimal());
        }
        [Test]
        public void TestToDouble()
        {
            Assert.AreEqual(3.3, new DecimalValue(33, -1).ToDouble(), 0.000000000001);
        }
        [Test]
        public void TestMaxValue()
        {
            DecimalValue max = new DecimalValue(int.MaxValue, 10);
            AssertEquals(new Decimal(2147483647e10), max.ToBigDecimal());
        }
        [Test]
        public void TestMantissaAndExponent()
        {
            DecimalValue value = new DecimalValue(9427.55);
            AssertEquals(942755, value.mantissa);
            AssertEquals(-2, value.exponent);

            value = new DecimalValue(942755, -2);
            AssertEquals(((decimal)9427.55), value.ToBigDecimal());
        }

        [Test]
        public void TestToByte()
        {
            AssertEquals(100, d(100.0).ToByte());
        }

        [Test]
        public void TestToByteWithDecimalPart()
        {
            try
            {
                d(100.1).ToByte();
                Assert.Fail();

            }
            catch (FastException e)
            {
                Assert.AreEqual(FastConstants.R5_DECIMAL_CANT_CONVERT_TO_INT, e.Code);
            }
        }
        [Test]
        public void TestToLong()
        {
            AssertEquals(10000000000000L, d(10000000000000.0).ToLong());
        }
        [Test]
        public void TestToShort()
        {
            AssertEquals(128, d(128.0).ToShort());
        }
        [Test]
        public void TestToShortWithDecimalPart()
        {
            try
            {
                d(100.1).ToShort();
                Assert.Fail();

            }
            catch (FastException e)
            {
                Assert.AreEqual(FastConstants.R5_DECIMAL_CANT_CONVERT_TO_INT, e.Code);
            }
        }
        [Test]
        public void TestToInt()
        {
            AssertEquals(100, d(100.0).ToInt());
        }
        [Test]
        public void TestToIntWithDecimalPart()
        {
            try
            {
                d(100.1).ToInt();
                Assert.Fail();

            }
            catch (FastException e)
            {
                Assert.AreEqual(FastConstants.R5_DECIMAL_CANT_CONVERT_TO_INT, e.Code);
            }
        }
        [Test]
        public void TestToLongWithDecimalPart()
        {
            try
            {
                d(100.1).ToLong();
                Assert.Fail();
            }
            catch (FastException e)
            {
                Assert.AreEqual(FastConstants.R5_DECIMAL_CANT_CONVERT_TO_INT, e.Code);
            }
        }

    }
}
