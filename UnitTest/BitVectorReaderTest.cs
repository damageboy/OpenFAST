using System;
using System.Collections.Generic;
using System.Text;
using OpenFAST;
using NUnit.Framework;

namespace UnitTest
{
    [TestFixture]
    public class BitVectorReaderTest
    {

        [Test]
        public void TestRead()
        {
            BitVectorReader reader = new BitVectorReader(new BitVector(ByteUtil.ConvertBitStringToFastByteArray("11000000")));
            Assert.IsTrue(reader.Read());
            Assert.IsFalse(reader.Read());
            Assert.IsFalse(reader.HasMoreBitsSet());
        }

    }
}
