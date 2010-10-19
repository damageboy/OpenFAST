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
using OpenFAST.Error;
using UnitTest.Test;

namespace UnitTest
{
    [TestFixture]
    public class IntegerValueTest : OpenFastTestCase
    {
        [Test]
        public void TestToBigDecimal()
        {
            Assert.AreEqual(new Decimal(125), Int(125).ToBigDecimal());
        }

        [Test]
        public void TestToByte()
        {
            AssertEquals(0x7f, Int(127).ToByte());
        }

        [Test]
        public void TestToByteWithLargeInt()
        {
            try
            {
                Int(128).ToByte();
                Assert.Fail();
            }
            catch (FastException e)
            {
                Assert.AreEqual(FastConstants.R4_NUMERIC_VALUE_TOO_LARGE, e.Code);
            }
        }

        [Test]
        public void TestToDouble()
        {
            Assert.AreEqual(125.0, Int(125).ToDouble(), 0.1);
        }

        [Test]
        public void TestToInt()
        {
            AssertEquals(125, Int(125).ToInt());
        }

        [Test]
        public void TestToLong()
        {
            AssertEquals(125L, Int(125).ToLong());
        }

        [Test]
        public void TestToShort()
        {
            AssertEquals((short) 32767, Int(32767).ToShort());
        }

        [Test]
        public void TestToShortWithLargeInt()
        {
            try
            {
                Int(32768).ToByte();
                Assert.Fail();
            }
            catch (FastException e)
            {
                Assert.AreEqual(FastConstants.R4_NUMERIC_VALUE_TOO_LARGE, e.Code);
            }
        }

        [Test]
        public void TestToString()
        {
            Assert.AreEqual("105", Int(105).ToString());
        }
    }
}