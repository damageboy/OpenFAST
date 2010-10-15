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
using System.IO;
using NUnit.Framework;
using OpenFAST;

namespace UnitTest.Test
{
    public class TestUtil
    {
        public static byte[] ToByte(StreamWriter stream)
        {
            if (stream.BaseStream is MemoryStream)
            {
                return ToByte((MemoryStream) stream.BaseStream);
            }
            throw new Exception("Invalid base stream");
        }

        public static byte[] ToByte(MemoryStream stream)
        {
            var ret = new byte[stream.Length];
            byte[] buff = stream.GetBuffer();
            for (int i = 0; i < ret.Length; i++)
            {
                ret[i] = buff[i];
            }
            return ret;
        }

        public static void AssertBitVectorEquals(String bitString, byte[] encoding)
        {
            Assert.AreEqual(ByteUtil.ConvertBitStringToFastByteArray(
                bitString), encoding);
        }
    }
}