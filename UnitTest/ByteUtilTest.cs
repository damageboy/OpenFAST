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

*/
using UnitTest.Test;
using NUnit.Framework;
using OpenFAST;

namespace UnitTest
{
    [TestFixture]
    public class ByteUtilTest : OpenFastTestCase
    {
        [Test]
        public void TestCombine()
        {
            AssertEquals("00000000 01111111", ByteUtil.Combine(new byte[] { 0x00 }, new byte[] { 0x7f }));
            AssertEquals("00000000 01000000 01111111 00111111", ByteUtil.Combine(new byte[] { 0x00, 0x40 }, new byte[] { 0x7f, 0x3f }));
            AssertEquals("00000000", ByteUtil.Combine(new byte[] { 0x00 }, new byte[] { }));
            AssertEquals("01111111", ByteUtil.Combine(new byte[] { }, new byte[] { 0x7f }));
        }

    }
}
