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
using OpenFAST.Template.Types.Codec;
using OpenFAST.UnitTests.Test;

namespace OpenFAST.UnitTests.Template.Types.Codec
{
    [TestFixture]
    public class NullableAsciiStringTest : OpenFastTestCase
    {
        [Test]
        public void TestEncodeDecode()
        {
            AssertEncodeDecode(null, "10000000", TypeCodec.NullableAscii);
            AssertEncodeDecode(new StringValue(""), "00000000 10000000", TypeCodec.NullableAscii);
        }
        [Test]
        public void TestOverlong()
        {
            try
            {
                TypeCodec.NullableAscii.Decode(BitStream("00000000 11000001"));
                Assert.Fail();
            }
            catch (RepErrorException e)
            {
                Assert.AreEqual(RepError.StringOverlong, e.Error);
            }
        }
    }
}
