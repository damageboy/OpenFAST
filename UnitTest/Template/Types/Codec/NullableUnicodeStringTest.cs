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
using OpenFAST.Template.Types.Codec;
using OpenFAST.UnitTests.Test;

namespace OpenFAST.UnitTests.Template.Types.Codec
{
    [TestFixture]
    public class NullableUnicodeStringTest : OpenFastTestCase
    {
        [Test]
        public void TestEncodeDecode()
        {
            AssertEncodeDecode(new StringValue("Yo"), "10000011 01011001 01101111", TypeCodec.NullableUnicode);
            AssertEncodeDecode(new StringValue("\u00f1"), "10000011 11000011 10110001", TypeCodec.NullableUnicode);
            AssertEncodeDecode(new StringValue("A\u00ea\u00f1\u00fcC"), "10001001 01000001 11000011 10101010 11000011 10110001 11000011 10111100 01000011", TypeCodec.NullableUnicode);
            AssertEncodeDecode(null, "10000000", TypeCodec.NullableUnicode);
        }
    }
}
