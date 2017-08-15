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
using NUnit.Framework;
using OpenFAST.Error;
using OpenFAST.UnitTests.Test;
using OpenFAST.Template.Types.Codec;

namespace OpenFAST.UnitTests.Template.Types.Codec
{
    [TestFixture]
    public class SingleFieldDecimalTest:OpenFastTestCase
    {
        [Test]
        public void TestEncodeDecodeBoundary()
        {
            AssertEncodeDecode(new DecimalValue(long.MaxValue, 63), "10111111 00000000 01111111 01111111 01111111 01111111 01111111 01111111 01111111 01111111 11111111", TypeCodec.SfScaledNumber);
            AssertEncodeDecode(new DecimalValue(long.MinValue, -63), "11000001 01111111 00000000 00000000 00000000 00000000 00000000 00000000 00000000 00000000 10000000", TypeCodec.SfScaledNumber);
        }
        [Test]
        public void TestEncodeDecode()
        {
            AssertEncodeDecode(Decimal(94275500), "10000010 00111001 01000101 10100011", TypeCodec.SfScaledNumber);
            AssertEncodeDecode(Decimal(9427.55), "11111110 00111001 01000101 10100011", TypeCodec.SfScaledNumber);
            AssertEncodeDecode(Decimal(4), "10000000 10000100", TypeCodec.SfScaledNumber);
            AssertEncodeDecode(Decimal(400), "10000010 10000100", TypeCodec.SfScaledNumber);
            AssertEncodeDecode(Decimal(0.4), "11111111 10000100", TypeCodec.SfScaledNumber);
            AssertEncodeDecode(Decimal(1000), "10000011 10000001", TypeCodec.SfScaledNumber);
            AssertEncodeDecode(Decimal(9427550, 1),"10000001 00000100 00111111 00110100 11011110",TypeCodec.SfScaledNumber);
        }
        [Test]
        public void TestEncodeLargeDecimalReportsError()
        {
            try
            {
                TypeCodec.SfScaledNumber.Encode(Decimal(150, 64));
                Assert.Fail();
            }
            catch (RepErrorException e)
            {
                Assert.AreEqual(RepError.LargeDecimal, e.Error);
                //assertEquals("Encountered exponent of size 64", e.getMessage());
            }
        }
        [Test]
        public void TestDecodeLargeDecimalReportsError()
        {
            try
            {
                TypeCodec.SfScaledNumber.Decode(BitStream("00000001 11111111 10000001"));
                Assert.Fail();
            }
            catch (RepErrorException e)
            {
                Assert.AreEqual(RepError.LargeDecimal, e.Error);
                //assertEquals("Encountered exponent of size 255", e.getMessage());
            }
        }
    }
}
