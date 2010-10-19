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
using OpenFAST;
using OpenFAST.Template;
using OpenFAST.Template.Operator;
using System.IO;
using OpenFAST.Template.Type;
using UnitTest.Test;

namespace UnitTest.Template
{
    [TestFixture]
    public class SequenceTest
    {
        private Group template;
        private Context context;
        [SetUp]
        protected void SetUp()
        {
            template = new MessageTemplate("", new Field[] { });
            context = new Context();
        }
        [Test]
        public void TestEncode()
        {
            Scalar firstName = new Scalar("First Name", FASTType.I32,
                    Operator.COPY, ScalarValue.Undefined, false);
            Scalar lastName = new Scalar("Last Name", FASTType.I32,
                    Operator.COPY, ScalarValue.Undefined, false);
            Sequence sequence1 = new Sequence("Contacts",
                    new Field[] { firstName, lastName }, false);

            SequenceValue sequenceValue = new SequenceValue(sequence1);
            sequenceValue.Add(new IFieldValue[] {
                new IntegerValue(1), new IntegerValue(2)
            });
            sequenceValue.Add(new IFieldValue[] {
                new IntegerValue(3), new IntegerValue(4)
            });

            byte[] actual = sequence1.Encode(sequenceValue, template, context, new BitVectorBuilder(1));
            string expected = "10000010 11100000 10000001 10000010 11100000 10000011 10000100";
            TestUtil.AssertBitVectorEquals(expected, actual);
        }
        [Test]
        public void TestDecode()
        {
            string actual = "10000010 11100000 10000001 10000010 11100000 10000011 10000100";
            Stream stream = ByteUtil.CreateByteStream(actual);

            Scalar firstNumber = new Scalar("First Number", FASTType.I32,
                    Operator.COPY, ScalarValue.Undefined, false);
            Scalar lastNumber = new Scalar("Second Number", FASTType.I32,
                    Operator.COPY, ScalarValue.Undefined, false);
            Sequence sequence1 = new Sequence("Contants",
                    new Field[] { firstNumber, lastNumber }, false);

            SequenceValue sequenceValue = new SequenceValue(sequence1);
            sequenceValue.Add(new IFieldValue[] {
                new IntegerValue(1), new IntegerValue(2)
            });
            sequenceValue.Add(new IFieldValue[] {
                new IntegerValue(3), new IntegerValue(4)
            });

            IFieldValue result = sequence1.Decode(stream, template, context, BitVectorReader.InfiniteTrue);
            Assert.AreEqual(sequenceValue, result);
        }

    }
}
