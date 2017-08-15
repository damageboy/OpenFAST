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
using OpenFAST.Template;
using OpenFAST.Template.Operators;
using OpenFAST.Template.Types;
using OpenFAST.Template.Types.Codec;
using OpenFAST.UnitTests.Test;

namespace OpenFAST.UnitTests.Template.Types.Codec
{
    [TestFixture]
    public class StringDeltaTest : OpenFastTestCase
    {
        [Test]
        public void TestEncodeValue()
        {
            AssertEncodeDecode(Twin(Int(1), new StringValue("A")), "10000001 11000001", TypeCodec.StringDelta);
        }

        [Test]
        public void TestNullValue()
        {
            var scalar = new Scalar("deltaString", FastType.String, Operator.Delta, ScalarValue.Undefined, true);
            var template = new MessageTemplate("template", new Field[] {scalar});
            var bvBuilder = new BitVectorBuilder(7);
            TestUtil.AssertBitVectorEquals("10000000", scalar.Encode(null, template, new Context(), bvBuilder));

            //		assertEquals(null, scalar.decode(bitStream("10000000"), ScalarValue.UNDEFINED));
        }
    }
}