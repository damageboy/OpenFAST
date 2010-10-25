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
    public class SignedIntegerTest : OpenFastTestCase
    {
        [Test]
        public void TestEncodeDecode()
        {
            AssertEncodeDecode(Int(63), "10111111", TypeCodec.Integer);
            AssertEncodeDecode(Int(64), "00000000 11000000", TypeCodec.Integer);
            AssertEncodeDecode(Int(-1), "11111111", TypeCodec.Integer);
            AssertEncodeDecode(Int(-2), "11111110", TypeCodec.Integer);
            AssertEncodeDecode(Int(-64), "11000000", TypeCodec.Integer);
            AssertEncodeDecode(Int(-65), "01111111 10111111", TypeCodec.Integer);
            AssertEncodeDecode(Int(639), "00000100 11111111", TypeCodec.Integer);
            AssertEncodeDecode(Int(942755), "00111001 01000101 10100011", TypeCodec.Integer);
            AssertEncodeDecode(Int(-942755), "01000110 00111010 11011101", TypeCodec.Integer);
            AssertEncodeDecode(Int(8193), "00000000 01000000 10000001", TypeCodec.Integer);
            AssertEncodeDecode(Int(-8193), "01111111 00111111 11111111", TypeCodec.Integer);
        }
        [Test]
        public void TestEncodeDecodeBoundary()
        {
            AssertEncodeDecode(Long(long.MaxValue), "00000000 01111111 01111111 01111111 01111111 01111111 01111111 01111111 01111111 11111111", TypeCodec.Integer);
            AssertEncodeDecode(Long(long.MinValue), "01111111 00000000 00000000 00000000 00000000 00000000 00000000 00000000 00000000 10000000", TypeCodec.Integer);
        }
    }
}
