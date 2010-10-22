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
using OpenFAST.Template;
using OpenFAST.Template.Type;
using UnitTest.Test;

namespace UnitTest.Template
{
    [TestFixture]
    public class SequenceTest
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

        [Test]
        public void TestDecode()
        {
            const string actual = "10000010 11100000 10000001 10000010 11100000 10000011 10000100";
            Stream stream = ByteUtil.CreateByteStream(actual);

            var firstNumber = new Scalar("First Number", FASTType.I32, OpenFAST.Template.Operator.Operator.Copy, ScalarValue.Undefined, false);
            var lastNumber = new Scalar("Second Number", FASTType.I32, OpenFAST.Template.Operator.Operator.Copy, ScalarValue.Undefined, false);
            var sequence1 = new Sequence("Contants", new Field[] {firstNumber, lastNumber}, false);

            var sequenceValue = new SequenceValue(sequence1);
            sequenceValue.Add(new IFieldValue[]
                                  {
                                      new IntegerValue(1), new IntegerValue(2)
                                  });
            sequenceValue.Add(new IFieldValue[]
                                  {
                                      new IntegerValue(3), new IntegerValue(4)
                                  });

            IFieldValue result = sequence1.Decode(stream, _template, _context, BitVectorReader.InfiniteTrue);
            Assert.AreEqual(sequenceValue, result);
        }

        [Test]
        public void TestEncode()
        {
            var firstName = new Scalar("First Name", FASTType.I32, OpenFAST.Template.Operator.Operator.Copy, ScalarValue.Undefined, false);
            var lastName = new Scalar("Last Name", FASTType.I32, OpenFAST.Template.Operator.Operator.Copy, ScalarValue.Undefined, false);
            var sequence1 = new Sequence("Contacts", new Field[] {firstName, lastName}, false);

            var sequenceValue = new SequenceValue(sequence1);
            sequenceValue.Add(new IFieldValue[]
                                  {
                                      new IntegerValue(1), new IntegerValue(2)
                                  });
            sequenceValue.Add(new IFieldValue[]
                                  {
                                      new IntegerValue(3), new IntegerValue(4)
                                  });

            byte[] actual = sequence1.Encode(sequenceValue, _template, _context, new BitVectorBuilder(1));
            
            const string expected = "10000010 11100000 10000001 10000010 11100000 10000011 10000100";
            TestUtil.AssertBitVectorEquals(expected, actual);
        }
    }
}