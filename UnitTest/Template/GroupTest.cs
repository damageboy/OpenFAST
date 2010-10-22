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
using System.IO;
using NUnit.Framework;
using OpenFAST;
using OpenFAST.Codec;
using OpenFAST.Error;
using OpenFAST.Template;
using OpenFAST.Template.Type;
using UnitTest.Test;

namespace UnitTest.Template
{
    [TestFixture]
    public class GroupTest : OpenFastTestCase
    {
        #region Setup/Teardown

        [SetUp]
        protected void SetUp()
        {
            _template = new MessageTemplate("", new Field[] {});
            _context = new Context();
        }

        #endregion

        private Group _template;
        private Context _context;

        [TestCase]
        public void TestEncode()
        {
            var firstName = new Scalar("First Name", FASTType.U32, OpenFAST.Template.Operator.Operator.Copy, ScalarValue.Undefined, true);
            var lastName = new Scalar("Last Name", FASTType.U32, OpenFAST.Template.Operator.Operator.None, ScalarValue.Undefined, false);
            var theGroup = new Group("guy", new Field[] {firstName, lastName}, false);

            byte[] actual = theGroup.Encode(
                new GroupValue(
                    new Group("", new Field[] {}, false),
                    new IFieldValue[] {new IntegerValue(1), new IntegerValue(2)}),
                _template, _context);

            const string expected = "11000000 10000010 10000010";

            TestUtil.AssertBitVectorEquals(expected, actual);
        }

        [TestCase]
        public void TestDecode()
        {
            const string message = "11000000 10000010 10000010";
            Stream inp = new MemoryStream(ByteUtil.ConvertBitStringToFastByteArray(message));
            var firstname = new Scalar("firstName", FASTType.U32, OpenFAST.Template.Operator.Operator.Copy, ScalarValue.Undefined, true);
            var lastName = new Scalar("lastName", FASTType.U32, OpenFAST.Template.Operator.Operator.None, ScalarValue.Undefined, false);

            // MessageInputStream in = new MessageInputStream(new
            // ByteArrayInputStream(message.getBytes()));
            var group = new Group("person", new Field[] {firstname, lastName}, false);
            var groupValue = (GroupValue) group.Decode(inp, _template, _context, BitVectorReader.InfiniteTrue);
            Assert.AreEqual(1, ((IntegerValue) groupValue.GetValue(0)).Value);
            Assert.AreEqual(2, ((IntegerValue) groupValue.GetValue(1)).Value);
        }

        [TestCase]
        public void TestGroupWithoutPresenceMap()
        {
            MessageTemplate template = Template(
                "<template><group name='priceGroup' presence='optional'><decimal name='price'><delta/></decimal></group></template>");
            var encodingContext = new Context();
            var decodingContext = new Context();
            encodingContext.RegisterTemplate(1, template);
            decodingContext.RegisterTemplate(1, template);

            const string encodedBits = "11100000 10000001 11111110 10111111";

            var decoder = new FastDecoder(decodingContext,
                                          new MemoryStream(ByteUtil.ConvertBitStringToFastByteArray(encodedBits)));
            Message message = decoder.ReadMessage();
            Assert.AreEqual(0.63, message.GetGroup("priceGroup").GetDouble("price"), 0.01);

            byte[] encoding = template.Encode(message, encodingContext);
            TestUtil.AssertBitVectorEquals(encodedBits, encoding);
        }

        [TestCase]
        public void TestDecodeGroupWithOverlongPresenceMap()
        {
            try
            {
                ObjectMother.QuoteTemplate().Decode(
                    new MemoryStream(ByteUtil.ConvertBitStringToFastByteArray("00000000 10000000")),
                    ObjectMother.QuoteTemplate(), new Context(),
                    BitVectorReader.InfiniteTrue);
                Assert.Fail();
            }
            catch (FastException e)
            {
                Assert.AreEqual(FastConstants.R7PmapOverlong, e.Code);
            }
        }

        [TestCase]
        public void TestDecodeGroupWithPresenceMapWithTooManyBits()
        {
            MessageTemplate g = ObjectMother.QuoteTemplate();
            var c = new Context();
            c.RegisterTemplate(1, g);
            try
            {
                g.Decode(
                    new MemoryStream(
                        ByteUtil.ConvertBitStringToFastByteArray("11111000 10000001 10000000 10000110 10000000 10000110")),
                    g, c, BitVectorReader.InfiniteTrue);
            }
            catch (FastException e)
            {
                Assert.AreEqual(FastConstants.R8PmapTooManyBits, e.Code);
            }
        }
    }
}