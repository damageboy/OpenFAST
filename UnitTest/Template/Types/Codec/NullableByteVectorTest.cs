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
    public class NullableByteVectorTest : OpenFastTestCase
    {
        [Test]
        public void TestEncoding()
        {
            AssertEncodeDecode(null, "10000000", TypeCodec.NullableByteVectorType);
            AssertEncodeDecode(Byte(new byte[] { 0x00 }), "10000010 00000000", TypeCodec.NullableByteVectorType);
            AssertEncodeDecode(Byte(new byte[] { 0x00, 0x7F }), "10000011 00000000 01111111", TypeCodec.NullableByteVectorType);
        }
    }
}
