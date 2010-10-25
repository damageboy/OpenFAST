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
using OpenFAST.UnitTests.Test;
using OpenFAST.Template.Types.Codec;

namespace OpenFAST.UnitTests.Template.Types.Codec
{
    [TestFixture]
    public class NullableSignedIntegerTest : OpenFastTestCase
    {
        [Test]
        public void TestEncodeDecode()
        {
            AssertEncodeDecode(null, "10000000", TypeCodec.NullableInteger);
            AssertEncodeDecode(Int(0), "10000001", TypeCodec.NullableInteger);
            AssertEncodeDecode(Int(638), "00000100 11111111", TypeCodec.NullableInteger);
            AssertEncodeDecode(Int(-2147483648), "01111000 00000000 00000000 00000000 10000000", TypeCodec.NullableInteger);
            AssertEncodeDecode(Int(-17), "11101111", TypeCodec.NullableInteger);
            AssertEncodeDecode(Int(547), "00000100 10100100", TypeCodec.NullableInteger);
            AssertEncodeDecode(Int(-5), "11111011", TypeCodec.NullableInteger);
            AssertEncodeDecode(Int(124322), "00000111 01001011 10100011", TypeCodec.NullableInteger);
        }
    }
}
