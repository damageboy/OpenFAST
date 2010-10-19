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
    public class DynamicTemplateReferenceTest : OpenFastTestCase
    {
        private MessageTemplate nameTemplate;
        private MessageTemplate template;
        private Message message;
        private Message name;
        private Context context;
        [SetUp]
        protected void SetUp()
        {
            nameTemplate = Template("<template>" +
                            "  <string name=\"name\"/>" +
                            "</template>");
            template = Template("<template>" +
                            "  <uInt32 name=\"quantity\"/>" +
                            "  <templateRef />" +
                            "  <decimal name=\"price\"/>" +
                            "</template>");
            message = new Message(template);
            message.SetInteger(1, 15);
            message.SetDecimal(3, 102.0);

            name = new Message(nameTemplate);
            name.SetString(1, "IBM");
            message.SetFieldValue(2, name);

            context = new Context();
            context.RegisterTemplate(1, template);
            context.RegisterTemplate(2, nameTemplate);
        }
        [Test]
        public void TestEncode()
        {
            FastEncoder encoder = new FastEncoder(context);
            //            --PMAP-- --TID--- ---#1--- --PMAP-- --TID--- ------------#1------------ ------------#3------------
            TestUtil.AssertBitVectorEquals("11000000 10000001 10001111 11000000 10000010 01001001 01000010 11001101 10000000 00000000 11100110", encoder.Encode(message));
        }
        [Test]
        public void TestDecode()
        {
            //                 --PMAP-- --TID--- ---#1--- --PMAP-- --TID--- ------------#1------------ ------------#3------------
            string encoding = "11000000 10000001 10001111 11000000 10000010 01001001 01000010 11001101 10000000 00000000 11100110";
            FastDecoder decoder = new FastDecoder(context, BitStream(encoding));
            Message readMessage = decoder.ReadMessage();
            Assert.AreEqual(message, readMessage);
        }
    }
}
