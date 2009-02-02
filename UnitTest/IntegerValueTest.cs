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