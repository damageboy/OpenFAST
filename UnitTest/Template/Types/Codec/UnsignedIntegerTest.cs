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
    public class UnsignedIntegerTest : OpenFastTestCase
    {
        [Test]
        public void TestEncodeDecode()
        {
            AssertEncodeDecode(Int(127), "11111111", TypeCodec.Uint);
            AssertEncodeDecode(Int(16383), "01111111 11111111", TypeCodec.Uint);
            AssertEncodeDecode(Int(5), "10000101", TypeCodec.Uint);
            AssertEncodeDecode(Int(0), "10000000", TypeCodec.Uint);
            AssertEncodeDecode(Int(942755), "00111001 01000101 10100011", TypeCodec.Uint);
            AssertEncodeDecode(Int(268435452), "01111111 01111111 01111111 11111100", TypeCodec.Uint);
            AssertEncodeDecode(Int(269435452), "00000001 00000000 00111101 00000100 10111100", TypeCodec.Uint);
            AssertEncodeDecode(Long(274877906943L), "00000111 01111111 01111111 01111111 01111111 11111111", TypeCodec.Uint);
            AssertEncodeDecode(Long(1181048340000L), "00100010 00101111 01011111 01011101 01111100 10100000", TypeCodec.Uint);
            AssertEncodeDecode(Long(4294967295L), "00001111 01111111 01111111 01111111 11111111", TypeCodec.Uint);
        }
    }
}
