using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using OpenFAST;
using UnitTest.Test;

namespace UnitTest
{
    [TestFixture]
    public class BitVectorTest
    {
        [Test]
        public void TestGetTruncatedBytes()
        {
            BitVector vector = new BitVector(new byte[] { 0x00, 0x00 });
            Assert.IsTrue(vector.Overlong);
            TestUtil.AssertByteArrayEquals(new byte[] { 0x80 } ,
                vector.TruncatedBytes);

            vector = new BitVector(new byte[] { 0x00 });
            Assert.IsFalse(vector.Overlong);
            TestUtil.AssertByteArrayEquals(new byte[] { 0x80 },
                vector.TruncatedBytes);

            vector = new BitVector(new byte[] { 0x60, 0x00, 0x04, 0x00 });
            Assert.IsTrue(vector.Overlong);
            TestUtil.AssertByteArrayEquals(new byte[] { 0x60, 0x00, 0x84 },
                vector.TruncatedBytes);
        }

        [Test]
        public void TestGetBytes()
        {
            BitVector vector = new BitVector(7);
            Assert.AreEqual(1, vector.Bytes.Length);
            vector = new BitVector(8);
            Assert.AreEqual(2, vector.Bytes.Length);
        }
        [Test]
        public void TestSetWithOneByte()
        {
            BitVector vector = new BitVector(7);
            vector.set_Renamed(0);
            TestUtil.AssertBitVectorEquals("11000000", vector.Bytes);
            vector.set_Renamed(3);
            TestUtil.AssertBitVectorEquals("11001000", vector.Bytes);
            vector.set_Renamed(6);
            TestUtil.AssertBitVectorEquals("11001001", vector.Bytes);
        }
        [Test]
        public void TestIsSet()
        {
            BitVector vector = new BitVector(7);
            Assert.IsFalse(vector.IsSet(1));
            vector.set_Renamed(1);
            Assert.IsTrue(vector.IsSet(1));
            Assert.IsFalse(vector.IsSet(6));
            vector.set_Renamed(6);
            Assert.IsTrue(vector.IsSet(6));
            Assert.IsFalse(vector.IsSet(7));
            Assert.IsFalse(vector.IsSet(8));
        }
        [Test]
        public void TestSetWithMultipleBytes()
        {
            BitVector vector = new BitVector(15);
            vector.set_Renamed(0);
            TestUtil.AssertBitVectorEquals("01000000 00000000 10000000",
                vector.Bytes);
            vector.set_Renamed(4);
            TestUtil.AssertBitVectorEquals("01000100 00000000 10000000",
                vector.Bytes);
            vector.set_Renamed(9);
            TestUtil.AssertBitVectorEquals("01000100 00010000 10000000",
                vector.Bytes);
            vector.set_Renamed(14);
            TestUtil.AssertBitVectorEquals("01000100 00010000 11000000",
                vector.Bytes);
        }
        [Test]
        public void TestEquals()
        {
            BitVector expected = new BitVector(new byte[] { 0xf0 });
            BitVector actual = new BitVector(7);
            actual.set_Renamed(0);
            actual.set_Renamed(1);
            actual.set_Renamed(2);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void TestIndexLastSet()
        {
            BitVector bv = new BitVector(new byte[] { 0x70, 0x00, 0x04 });
            Assert.AreEqual(18, bv.IndexOfLastSet());
        }
    }
}
