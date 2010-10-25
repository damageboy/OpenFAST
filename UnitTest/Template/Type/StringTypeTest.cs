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

namespace OpenFAST.UnitTests.Template.Type
{
    [TestFixture]
    public class StringTypeTest : OpenFastTestCase
    {
        [Test]
        public void TestStringWithLength()
        {
            MessageTemplate template = Template("<template name=\"template\">"
                    + "  <string name=\"message\" charset=\"unicode\"><length name=\"messageLength\"/><copy/></string>" + "</template>");
            FastDecoder decoder = Decoder("11100000 10000001 10000010 01010101 10101010", template);
            Message message = decoder.ReadMessage();
            Assert.AreEqual(2, message.GetInt("messageLength"));
        }
    }
}
