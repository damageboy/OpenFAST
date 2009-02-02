using System;
using System.Collections.Generic;
using System.Text;
using UnitTest.Test;
using NUnit.Framework;
using OpenFAST;

namespace UnitTest
{
    [TestFixture]
    public class ByteUtilTest : OpenFastTestCase
    {
        [Test]
        public void TestCombine()
        {
            AssertEquals("00000000 01111111", ByteUtil.Combine(new byte[] { 0x00 }, new byte[] { 0x7f }));
            AssertEquals("00000000 01000000 01111111 00111111", ByteUtil.Combine(new byte[] { 0x00, 0x40 }, new byte[] { 0x7f, 0x3f }));
            AssertEquals("00000000", ByteUtil.Combine(new byte[] { 0x00 }, new byte[] { }));
            AssertEquals("01111111", ByteUtil.Combine(new byte[] { }, new byte[] { 0x7f }));
        }

    }
}
