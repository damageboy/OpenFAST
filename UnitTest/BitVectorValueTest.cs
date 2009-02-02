using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using OpenFAST;
using UnitTest.Test;

namespace UnitTest
{
    [TestFixture]
    public class BitVectorValueTest
    {
        [Test]
        public void TestEquals()
        {
            BitVectorValue expected = new BitVectorValue(new BitVector(
                        new byte[] { TestUtil.ToSByte(0xf0) }));
            BitVectorValue actual = new BitVectorValue(new BitVector(7));
            actual.value_Renamed.set_Renamed(0);
            actual.value_Renamed.set_Renamed(1);
            actual.value_Renamed.set_Renamed(2);
            Assert.AreEqual(expected, actual);
        }
    }
}
