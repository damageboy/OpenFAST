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

namespace UnitTest
{
    [TestFixture]
    public class IntegerValueTest : OpenFastTestCase
    {
        [Test]
        public void TestToInt()
        {
            AssertEquals(125, i(125).ToInt());
        }
        [Test]
        public void TestToLong()
        {
            AssertEquals(125L, i(125).ToLong());
        }
        [Test]
        public void TestToString()
        {
            Assert.AreEqual("105", i(105).ToString());
        }
        [Test]
        public void TestToByte()
        {
            AssertEquals(0x7f, i(127).ToByte());
        }
        [Test]
        public void TestToByteWithLargeInt()
        {
            try
            {
                i(128).ToByte();
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
            AssertEquals((short)32767, i(32767).ToShort());
        }
        [Test]
        public void TestToShortWithLargeInt()
        {
            try
            {
                i(32768).ToByte();
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
            Assert.AreEqual(125.0, i(125).ToDouble(), 0.1);
        }
        [Test]
        public void TestToBigDecimal()
        {
            Assert.AreEqual(new Decimal(125), i(125).ToBigDecimal());
        }

    }

}