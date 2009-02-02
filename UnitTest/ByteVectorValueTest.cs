using System;
using System.Collections.Generic;
using System.Text;
using UnitTest.Test;
using OpenFAST;
using NUnit.Framework;

namespace UnitTest
{
    [TestFixture]
    public class ByteVectorValueTest : OpenFastTestCase
    {
        [Test]
        public void TestEquals()
        {
            ByteVectorValue expected = new ByteVectorValue(new byte[] { TestUtil.ToSByte(0xff) });
            ByteVectorValue actual = new ByteVectorValue(new byte[] { TestUtil.ToSByte(0xff) });
            Assert.AreEqual(expected, actual);
        }
    }
}
