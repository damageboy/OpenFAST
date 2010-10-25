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
using OpenFAST.Codec;
using OpenFAST.Template;
using OpenFAST.Template.Types.Codec;
using OpenFAST.UnitTests.Test;

namespace OpenFAST.UnitTests.Template.Type.Codec
{
    [TestFixture]
    public class ByteVectorTest : OpenFastTestCase
    {
        [Test]
        public void TestEncode()
        {
            AssertEncodeDecode(Byte(new byte[] { 0x00 }), "10000001 00000000", TypeCodec.ByteVector);
            AssertEncodeDecode(Byte(new byte[] { 0x00, 0x7f }), "10000010 00000000 01111111", TypeCodec.ByteVector);
            AssertEncodeDecode(Byte(new byte[] { 0x01, 0x02, 0x04, 0x08, 0x10 }), "10000101 00000001 00000010 00000100 00001000 00010000",
                    TypeCodec.ByteVector);
            AssertEncodeDecode(Byte(new byte[] { 0x16, 0x32, 0x64, 0x0f }), "10000100 00010110 00110010 01100100 00001111",
                    TypeCodec.ByteVector);
            AssertEncodeDecode(Byte(new byte[] { 0x57, 0x4e }), "10000010 01010111 01001110", TypeCodec.ByteVector);
        }
        [Test]
        public void TestByteVectorWithLength()
        {
            MessageTemplate template = Template("<template name=\"template\">"
                    + "  <byteVector name=\"data\"><length name=\"dataLength\"/><copy/></byteVector>" + "</template>");
            FastDecoder decoder = Decoder("11100000 10000001 10000001 01010101", template);
            Message message = decoder.ReadMessage();
            Assert.AreEqual(1, message.GetInt("dataLength"));
        }
    }
}
