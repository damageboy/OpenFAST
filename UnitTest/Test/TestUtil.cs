using System;
using System.Collections.Generic;
using System.Text;
using OpenFAST;
using NUnit.Framework;
using System.IO;

namespace UnitTest.Test
{
    public class TestUtil
    {
        public static byte[] ToSByte(StreamWriter stream)
        {
            if (stream.BaseStream is MemoryStream)
            {
                return ToSByte((MemoryStream)stream.BaseStream);

            }
            throw new Exception("Invalid base stream");
        }
        public static byte[] ToSByte(MemoryStream stream)
        {
            byte[] ret = new byte[stream.Length];
            byte[] buff = stream.GetBuffer();
            for (int i = 0; i < ret.Length; i++)
            {
                ret[i] = ToSByte(buff[i]);
            }
            return ret;
        }
        public static byte ToSByte(byte b)
        {
            byte bb =(byte) b;
            return bb;
        }
        public static void AssertBitVectorEquals(String bitString, byte[] encoding)
        {
            AssertByteArrayEquals(ByteUtil.ConvertBitStringToFastByteArray(
                        bitString), encoding);
        }

        public static void AssertByteArrayEquals(byte[] expected, byte[] actual)
        {
            String error = "expected:<" +
            ByteUtil.ConvertByteArrayToBitString(expected) +
            "> but was:<" + ByteUtil.ConvertByteArrayToBitString(actual) +
            ">";
            if (expected.Length != actual.Length)
                Assert.Fail(error);

            for (int i = 0; i < expected.Length; i++)
            {
                TestCase.AssertEquals(error, expected[i], actual[i]);
            }
        }
    }
}
