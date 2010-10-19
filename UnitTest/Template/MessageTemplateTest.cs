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
using OpenFAST;
using OpenFAST.Codec;
using OpenFAST.Template;
using UnitTest.Test;

namespace UnitTest.Template
{
    [TestFixture]
    public class MessageTemplateTest : OpenFastTestCase
    {
        [Test]
        public void TestMessageTemplateWithNoFieldsThatUsePresenceMapStillEncodesPresenceMap()
        {
            MessageTemplate template = Template(
                "<template>" +
                "  <string name='string'/>" +
                "  <decimal name='decimal'><delta/></decimal>" +
                "</template>");

            const string encoding = "11000000 10000001 11100001 10000000 10000001";

            FastDecoder decoder = Decoder(encoding, template);
            FastEncoder encoder = Encoder(template);

            Message message = decoder.ReadMessage();
            Assert.AreEqual("a", message.GetString("string"));
            Assert.AreEqual(1.0, message.GetDouble("decimal"), 0.1);

            TestUtil.AssertBitVectorEquals(encoding, encoder.Encode(message));
        }
    }
}