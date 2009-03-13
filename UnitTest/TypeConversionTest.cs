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
using UnitTest.Test;
using NUnit.Framework;
using OpenFAST.Template;
using OpenFAST;
using OpenFAST.Codec;

namespace UnitTest
{
    [TestFixture]
    public class TypeConversionTest : OpenFastTestCase
    {
        [Test]
        public void TestConversions()
        {
            MessageTemplate template = Template(
                    "<template>" +
                    "  <string name=\"string\"/>" +
                    "  <uInt32 name=\"uint\"/>" +
                    "  <int8 name=\"byte\"/>" +
                    "  <int16 name=\"short\"/>" +
                    "  <int64 name=\"long\"/>" +
                    "  <byteVector name=\"bytevector\"/>" +
                    "  <decimal name=\"decimal\"/>" +
                    "</template>");

            var message = new Message(template);
            message.SetByteVector("string", byt("7f001a"));
            message.SetDecimal("uint", 150.0);
            message.SetString("byte", "4");
            message.SetString("short", "-5");
            message.SetString("long", "1000000000000000000");
            message.SetString("bytevector", "abcd");
            message.SetString("decimal", "2.3");

            FastEncoder encoder = Encoder(template);

            byte[] encoding = encoder.Encode(message);

            FastDecoder decoder = Decoder(template, encoding);
            Message decodedMessage = decoder.ReadMessage();

            Assert.AreEqual("7f001a", decodedMessage.GetString("string"));
            Assert.AreEqual(150, decodedMessage.GetInt("uint"));
            Assert.AreEqual(150, decodedMessage.GetShort("uint"));
            Assert.AreEqual(4, decodedMessage.GetByte("byte"));
            Assert.AreEqual(-5, decodedMessage.GetShort("short"));
            Assert.AreEqual(1000000000000000000L, decodedMessage.GetLong("long"));
            Assert.AreEqual("61626364", decodedMessage.GetString("bytevector"));
        }
    }

}
