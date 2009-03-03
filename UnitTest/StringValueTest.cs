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
using OpenFAST;
using NUnit.Framework;
using OpenFAST.Error;

namespace UnitTest
{
    [TestFixture]
    public class StringValueTest : OpenFastTestCase
    {
        [Test]
        public void TestToByte()
        {
            ScalarValue value = String("5");
            Assert.AreEqual(0x05, value.ToByte());
        }
        [Test]
        public void TestToByteWithLargeValue()
        {
            try
            {
                ScalarValue value = String("128");
                value.ToByte();
                Assert.Fail();
            }
            catch (FastException e)
            {
                Assert.AreEqual(FastConstants.R4_NUMERIC_VALUE_TOO_LARGE, e.Code);
            }
        }
        [Test]
        public void TestToShort()
        {
            ScalarValue value = String("128");
            Assert.AreEqual((short)0x80, value.ToShort());
        }
        [Test]
        public void TestToShortWithLargeValue()
        {
            try
            {
                ScalarValue value = String("32768");
                value.ToShort();
                Assert.Fail();
            }
            catch (FastException e)
            {
                Assert.AreEqual(FastConstants.R4_NUMERIC_VALUE_TOO_LARGE, e.Code);
            }
        }
        [Test]
        public void TestToInt()
        {
            ScalarValue value = String("32768");
            Assert.AreEqual(32768, value.ToInt());
        }
        [Test]
        public void TestToIntWithLargeValue()
        {
            try
            {
                ScalarValue value = String("2147483648");
                value.ToInt();
                Assert.Fail();
            }
            catch (FastException e)
            {
                Assert.AreEqual(FastConstants.R4_NUMERIC_VALUE_TOO_LARGE, e.Code);
            }
        }
        [Test]
        public void TestToLong()
        {
            ScalarValue value = String("2147483648");
            Assert.AreEqual(2147483648L, value.ToLong());
        }

        public void TestToLongWithLargeValue()
        {
            try
            {
                ScalarValue value = String(" 9223372036854775808");
                value.ToLong();
                Assert.Fail();
            }
            catch (FastException e)
            {
                Assert.AreEqual(FastConstants.R4_NUMERIC_VALUE_TOO_LARGE, e.Code);
            }
        }

        public void TestGetBytes()
        {
            AssertEquals("01100001 01100010 01100011 01100100", String("abcd").Bytes);
            AssertEquals("01000001 01000010 01000011 01000100", String("ABCD").Bytes);
        }

        public void TestToDouble()
        {
            ScalarValue value = String("  -1.234 ");
            Assert.AreEqual(-1.234, value.ToDouble(), .001);
        }

        public void TestToBigDecimal()
        {
            Assert.AreEqual(new Decimal(-1.234), String("-1.234").ToBigDecimal());
        }

        public void TestToString()
        {
            ScalarValue value = String("1234abcd");
            Assert.AreEqual("1234abcd", value.ToString());
        }

    }

}
