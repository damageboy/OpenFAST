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
using OpenFAST.Error;
using OpenFAST.Template;
using OpenFAST.Template.Types.Codec;
using OpenFAST.UnitTests.Test;

namespace OpenFAST.UnitTests.Template.Types
{
    [TestFixture]
    public class IntegerTypeTest : OpenFastTestCase
    {
        [Test]
        public void TestGetSignedIntegerSize()
        {
            Assert.AreEqual(1, IntegerCodec.GetSignedIntegerSize(63));
            Assert.AreEqual(1, IntegerCodec.GetSignedIntegerSize(-64));
            Assert.AreEqual(2, IntegerCodec.GetSignedIntegerSize(64));
            Assert.AreEqual(2, IntegerCodec.GetSignedIntegerSize(8191));
            Assert.AreEqual(2, IntegerCodec.GetSignedIntegerSize(-8192));
            Assert.AreEqual(2, IntegerCodec.GetSignedIntegerSize(-65));
            Assert.AreEqual(4, IntegerCodec.GetSignedIntegerSize(134217727));
            Assert.AreEqual(4, IntegerCodec.GetSignedIntegerSize(-134217728));
        }

        [Test]
        public void TestIntegerSizeTooLarge()
        {
            MessageTemplate template = Template(
                "<template>" +
                "  <uInt32 name='price'/>" +
                "</template>");
            FastDecoder decoder = Decoder("11000000 10000001 00111111 01111111 01111111 01111111 11111111", template);
            try
            {
                decoder.ReadMessage();
                Assert.Fail();
            }
            catch (DynErrorException e)
            {
                Assert.AreEqual(DynError.IntOutOfRange, e.Error);
                //Assert.AreEqual("The value 17179869183 is out of range for type uInt32", e.getCause().getCause().getMessage());
                return;
            }
        }
    }
}