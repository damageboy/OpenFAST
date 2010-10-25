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
using OpenFAST.UnitTests.Test;

namespace OpenFAST.UnitTests.Template
{
    [TestFixture]
    public class DynamicTemplateReferenceTest : OpenFastTestCase
    {
        #region Setup/Teardown

        [SetUp]
        protected void SetUp()
        {
            _nameTemplate = Template("<template>" +
                                     "  <string name='name'/>" +
                                     "</template>");
            _template = Template("<template>" +
                                 "  <uInt32 name='quantity'/>" +
                                 "  <templateRef/>" +
                                 "  <decimal name='price'/>" +
                                 "</template>");
            _message = new Message(_template);
            _message.SetInteger(1, 15);
            _message.SetDecimal(3, 102.0);

            _name = new Message(_nameTemplate);
            _name.SetString(1, "IBM");
            _message.SetFieldValue(2, _name);

            _context = new Context();
            _context.RegisterTemplate(1, _template);
            _context.RegisterTemplate(2, _nameTemplate);
        }

        #endregion

        private MessageTemplate _nameTemplate;
        private MessageTemplate _template;
        private Message _message;
        private Message _name;
        private Context _context;

        [Test]
        public void TestDecode()
        {
            //   --PMAP-- --TID--- ---#1--- --PMAP-- --TID--- ------------#1------------ ------------#3------------
            const string encoding =
                "11000000 10000001 10001111 11000000 10000010 01001001 01000010 11001101 10000000 00000000 11100110";
            
            var decoder = new FastDecoder(_context, BitStream(encoding));
            Message readMessage = decoder.ReadMessage();
            Assert.AreEqual(_message, readMessage);
        }

        [Test]
        public void TestEncode()
        {
            var encoder = new FastEncoder(_context);
            
            //   --PMAP-- --TID--- ---#1--- --PMAP-- --TID--- ------------#1------------ ------------#3------------
            TestUtil.AssertBitVectorEquals(
                "11000000 10000001 10001111 11000000 10000010 01001001 01000010 11001101 10000000 00000000 11100110",
                encoder.Encode(_message));
        }
    }
}