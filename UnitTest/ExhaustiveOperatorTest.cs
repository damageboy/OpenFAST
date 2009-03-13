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
using System;
using OpenFAST;
using OpenFAST.Codec;
using OpenFAST.Template;
using NUnit.Framework;
using OpenFAST.Template.Operator;
using OpenFAST.Template.Type;
using UnitTest.Test;
using System.IO;

namespace UnitTest
{
    [TestFixture]
    public class ExhaustiveOperatorTest
    {
        private Context decodingContext;
        private Context encodingContext;
        private FastEncoder encoder;
        private PipedStream input;
        private PipedStream output;
        private FastDecoder decoder;
        [SetUp]
        public void SetUp()
        {
            output = new PipedStream();
            input = new PipedStream(output);
            encodingContext = new Context();
            decodingContext = new Context();
            encoder = new FastEncoder(encodingContext);
            decoder = new FastDecoder(decodingContext, input);
        }
        [Test]
        public void TestEmptyOperatorWithOptionalField()
        {
            var field = new Scalar("", FASTType.U32, Operator.NONE,
                    ScalarValue.UNDEFINED, true);
            MessageTemplate template = registerTemplate(field);

            var message = new Message(template);
            message.SetInteger(1, 126);

            //                 --PMAP-- --TID--- ---#1---
            const string encoding = "11000000 11110001 11111111";

            encodeAndAssertEquals(encoding, message);

            GroupValue readMessage = decoder.ReadMessage();

            Assert.AreEqual(message, readMessage);
        }

        [Test]
        public void TestEmptyOperatorWithOptionalFieldOnNullValue()
        {
            var field = new Scalar("", FASTType.U32, Operator.NONE,
                    ScalarValue.UNDEFINED, true);
            MessageTemplate template = registerTemplate(field);

            var message = new Message(template);

            //                 --PMAP-- --TID--- ---#1---
            const string encoding = "11000000 11110001 10000000";

            encodeAndAssertEquals(encoding, message);

            GroupValue readMessage = decoder.ReadMessage();

            Assert.AreEqual(message, readMessage);
        }

        [Test]
        public void TestEmptyOperatorWithSequenceOfMessages()
        {
            var field = new Scalar("", FASTType.U32, Operator.NONE,
                    ScalarValue.UNDEFINED, true);
            MessageTemplate template = registerTemplate(field);

            var msg1 = new Message(template);

            var msg2 = new Message(template);
            msg2.SetInteger(1, 15);

            //                 --PMAP-- --TID--- ---#1---
            String encoding = "11000000 11110001 10000000";
            encodeAndAssertEquals(encoding, msg1);

            //          --PMAP-- ---#1---
            encoding = "10000000 10010000";
            byte[] encodedMessage = encoder.Encode(msg2);
            TestUtil.AssertBitVectorEquals(encoding, encodedMessage);
            output.Write(encodedMessage);

            GroupValue readMessage = decoder.ReadMessage();
            Assert.AreEqual(msg1, readMessage);
            readMessage = decoder.ReadMessage();
            Assert.AreEqual(msg2, readMessage);
        }
        [Test]
        public void TestEmptyOperatorWithMandatoryField()
        {
            var field = new Scalar("", FASTType.U32, Operator.NONE,
                    ScalarValue.UNDEFINED, false);
            MessageTemplate template = registerTemplate(field);

            var msg1 = new Message(template);
            msg1.SetInteger(1, 0);

            var msg2 = new Message(template);
            msg2.SetInteger(1, 16);

            //                 --PMAP-- --TID--- ---#1---
            String encoding = "11000000 11110001 10000000";
            encodeAndAssertEquals(encoding, msg1);

            //          --PMAP-- ---#1---
            encoding = "10000000 10010000";
            byte[] encodedMessage = encoder.Encode(msg2);
            TestUtil.AssertBitVectorEquals(encoding, encodedMessage);
            output.Write(encodedMessage);

            GroupValue readMessage = decoder.ReadMessage();
            Assert.AreEqual(msg1, readMessage);
            readMessage = decoder.ReadMessage();
            Assert.AreEqual(msg2, readMessage);
        }

        [Test]
        public void TestConstantOperatorWithOptionalField()
        {
            var field = new Scalar("", FASTType.U32, Operator.CONSTANT, new IntegerValue(16), true);
            MessageTemplate template = registerTemplate(field);

            var msg1 = new Message(template);

            var msg2 = new Message(template);
            msg2.SetInteger(1, 16);

            //                     --PMAP-- --TID---
            encodeAndAssertEquals("11000000 11110001", msg1);

            //                     --PMAP--
            encodeAndAssertEquals("10100000", msg2);

            readMessageAndAssertEquals(msg1);
            readMessageAndAssertEquals(msg2);
        }

        [Test]
        public void TestConstantOperatorWithMandatoryField()
        {
            var field = new Scalar("", FASTType.U32, Operator.CONSTANT, new IntegerValue(16), false);
            MessageTemplate template = registerTemplate(field);

            var msg1 = new Message(template);
            //        msg1.setInteger(1, 16);

            var msg2 = new Message(template);
            msg2.SetInteger(1, 16);

            //                 --PMAP-- --TID---
            encodeAndAssertEquals("11000000 11110001", msg1);

            //                     --PMAP--
            encodeAndAssertEquals("10000000", msg2);

            GroupValue readMessage = decoder.ReadMessage();
            Assert.AreEqual(msg1, readMessage);
            readMessage = decoder.ReadMessage();
            Assert.AreEqual(msg2, readMessage);
        }
        [Test]
        public void TestDefaultOperatorWithOptionalField()
        {
            var field = new Scalar("", FASTType.U32, Operator.DEFAULT,
                    new IntegerValue(16), true);
            MessageTemplate template = registerTemplate(field);

            var msg1 = new Message(template);

            var msg2 = new Message(template);
            msg2.SetInteger(1, 16);

            var msg3 = new Message(template);
            msg3.SetInteger(1, 20);

            //                     --PMAP-- --TID--- ---#1---
            encodeAndAssertEquals("11100000 11110001 10000000", msg1);

            //                     --PMAP--
            encodeAndAssertEquals("10000000", msg2);

            //                     --PMAP-- ---#1---
            encodeAndAssertEquals("10100000 10010101", msg3);

            readMessageAndAssertEquals(msg1);
            readMessageAndAssertEquals(msg2);
            readMessageAndAssertEquals(msg3);
        }
        [Test]
        public void TestDefaultOperatorWithMandatoryField()
        {
            var field = new Scalar("", FASTType.U32, Operator.DEFAULT,
                    new IntegerValue(16), false);
            var template = registerTemplate(field);

            var msg1 = new Message(template);
            msg1.SetInteger(1, 16);

            var msg2 = new Message(template);
            msg2.SetInteger(1, 20);

            //                     --PMAP-- --TID---
            encodeAndAssertEquals("11000000 11110001", msg1);

            //                     --PMAP-- ---#1---
            encodeAndAssertEquals("10100000 10010100", msg2);

            readMessageAndAssertEquals(msg1);
            readMessageAndAssertEquals(msg2);
        }
        [Test]
        public void TestCopyOperatorWithOptionalField()
        {
            var field = new Scalar("", FASTType.U32, Operator.COPY,
                    new IntegerValue(16), true);
            MessageTemplate template = registerTemplate(field);

            var msg1 = new Message(template);
            msg1.SetInteger(1, 16);

            var msg2 = new Message(template);

            var msg3 = new Message(template);
            msg3.SetInteger(1, 20);

            var msg4 = new Message(template);
            msg4.SetInteger(1, 20);

            //                     --PMAP-- --TID---
            encodeAndAssertEquals("11000000 11110001", msg1);

            //                     --PMAP-- ---#1---
            encodeAndAssertEquals("10100000 10000000", msg2);

            //                     --PMAP-- ---#1---
            encodeAndAssertEquals("10100000 10010101", msg3);

            //                     --PMAP--
            encodeAndAssertEquals("10000000", msg4);

            readMessageAndAssertEquals(msg1);
            readMessageAndAssertEquals(msg2);
            readMessageAndAssertEquals(msg3);
            readMessageAndAssertEquals(msg4);
        }
        [Test]
        public void TestCopyOperatorWithMandatoryField()
        {
            var field = new Scalar("", FASTType.U32, Operator.COPY,
                    new IntegerValue(16), false);
            MessageTemplate template = registerTemplate(field);

            var msg1 = new Message(template);
            msg1.SetInteger(1, 16);

            var msg2 = new Message(template);
            msg2.SetInteger(1, 20);

            var msg3 = new Message(template);
            msg3.SetInteger(1, 20);

            //                     --PMAP-- --TID---
            encodeAndAssertEquals("11000000 11110001", msg1);

            //                     --PMAP-- ---#1---
            encodeAndAssertEquals("10100000 10010100", msg2);

            //                     --PMAP--
            encodeAndAssertEquals("10000000", msg3);

            readMessageAndAssertEquals(msg1);
            readMessageAndAssertEquals(msg2);
            readMessageAndAssertEquals(msg3);
        }
        [Test]
        public void TestIncrementOperatorWithOptionalField()
        {
            var field = new Scalar("", FASTType.U32,
                    Operator.INCREMENT, new IntegerValue(16), true);
            MessageTemplate template = registerTemplate(field);

            var msg1 = new Message(template);
            msg1.SetInteger(1, 16);

            var msg2 = new Message(template);
            msg2.SetInteger(1, 17);

            var msg3 = new Message(template);

            var msg4 = new Message(template);
            msg4.SetInteger(1, 20);

            //                     --PMAP-- --TID---
            encodeAndAssertEquals("11000000 11110001", msg1);

            //                     --PMAP--
            encodeAndAssertEquals("10000000", msg2);

            //                     --PMAP-- ---#1---
            encodeAndAssertEquals("10100000 10000000", msg3);

            //                     --PMAP-- ---#1---
            encodeAndAssertEquals("10100000 10010101", msg4);

            readMessageAndAssertEquals(msg1);
            readMessageAndAssertEquals(msg2);
            readMessageAndAssertEquals(msg3);
            readMessageAndAssertEquals(msg4);
        }
        [Test]
        public void TestIncrementOperatorWithMandatoryField()
        {
            var field = new Scalar("", FASTType.U32,
                    Operator.INCREMENT, new IntegerValue(16), false);
            MessageTemplate template = registerTemplate(field);

            var msg1 = new Message(template);
            msg1.SetInteger(1, 16);

            var msg2 = new Message(template);
            msg2.SetInteger(1, 17);

            var msg3 = new Message(template);
            msg3.SetInteger(1, 20);

            //                     --PMAP-- --TID---
            encodeAndAssertEquals("11000000 11110001", msg1);

            //                     --PMAP-- ---#1---
            encodeAndAssertEquals("10000000", msg2);

            //                     --PMAP--
            encodeAndAssertEquals("10100000 10010100", msg3);

            readMessageAndAssertEquals(msg1);
            readMessageAndAssertEquals(msg2);
            readMessageAndAssertEquals(msg3);
        }
        [Test]
        public void TestDeltaOperatorWithOptionalField()
        {
            var field = new Scalar("", FASTType.U32, Operator.DELTA,
                    new IntegerValue(16), true);
            MessageTemplate template = registerTemplate(field);

            var msg1 = new Message(template);
            msg1.SetInteger(1, 16);

            var msg2 = new Message(template);
            msg2.SetInteger(1, 17);

            var msg3 = new Message(template);

            var msg4 = new Message(template);
            msg4.SetInteger(1, 20);

            //                     --PMAP-- --TID--- ---#1---
            encodeAndAssertEquals("11000000 11110001 10000001", msg1);

            //                     --PMAP-- ---#1---
            encodeAndAssertEquals("10000000 10000010", msg2);

            //                     --PMAP-- ---#1---
            encodeAndAssertEquals("10000000 10000000", msg3);

            //                     --PMAP-- ---#1---
            encodeAndAssertEquals("10000000 10000100", msg4);

            readMessageAndAssertEquals(msg1);
            readMessageAndAssertEquals(msg2);
            readMessageAndAssertEquals(msg3);
            readMessageAndAssertEquals(msg4);
        }
        [Test]
        public void TestDeltaOperatorWithMandatoryField()
        {
            var field = new Scalar("", FASTType.U32,
                    Operator.INCREMENT, new IntegerValue(16), false);
            MessageTemplate template = registerTemplate(field);

            var msg1 = new Message(template);
            msg1.SetInteger(1, 16);

            var msg2 = new Message(template);
            msg2.SetInteger(1, 17);

            var msg3 = new Message(template);
            msg3.SetInteger(1, 20);

            //                     --PMAP-- --TID---
            encodeAndAssertEquals("11000000 11110001", msg1);

            //                     --PMAP-- ---#1---
            encodeAndAssertEquals("10000000", msg2);

            //                     --PMAP--
            encodeAndAssertEquals("10100000 10010100", msg3);

            readMessageAndAssertEquals(msg1);
            readMessageAndAssertEquals(msg2);
            readMessageAndAssertEquals(msg3);
        }
        [Test]
        public void TestTailOperatorWithOptionalField()
        {
            var field = new Scalar("", FASTType.STRING, Operator.TAIL,
                    new StringValue("abc"), true);
            MessageTemplate template = registerTemplate(field);

            var msg1 = new Message(template);
            msg1.SetString(1, "abc");

            var msg2 = new Message(template);
            msg2.SetString(1, "abd");

            var msg3 = new Message(template);

            var msg4 = new Message(template);
            msg4.SetString(1, "dbef");

            //                     --PMAP-- --TID---
            encodeAndAssertEquals("11000000 11110001", msg1);

            //                     --PMAP-- ---#1---
            encodeAndAssertEquals("10100000 11100100", msg2);

            //                     --PMAP-- ---#1---
            encodeAndAssertEquals("10100000 10000000", msg3);

            //                     --PMAP-- -----------------#1----------------
            encodeAndAssertEquals("10100000 01100100 01100010 01100101 11100110",
                msg4);

            readMessageAndAssertEquals(msg1);
            readMessageAndAssertEquals(msg2);
            readMessageAndAssertEquals(msg3);
            readMessageAndAssertEquals(msg4);
        }
        [Test]
        public void TestTailOperatorWithMandatoryField()
        {
            var field = new Scalar("", FASTType.STRING, Operator.TAIL,
                    new StringValue("abc"), false);
            MessageTemplate template = registerTemplate(field);

            var msg1 = new Message(template);
            msg1.SetString(1, "abc");

            var msg2 = new Message(template);
            msg2.SetString(1, "abd");

            var msg3 = new Message(template);
            msg3.SetString(1, "abc");

            var msg4 = new Message(template);
            msg4.SetString(1, "dbef");

            //                     --PMAP-- --TID---
            encodeAndAssertEquals("11000000 11110001", msg1);

            //                     --PMAP-- ---#1---
            encodeAndAssertEquals("10100000 11100100", msg2);

            //                     --PMAP-- ---#1---
            encodeAndAssertEquals("10100000 11100011", msg3);

            //                     --PMAP-- -----------------#1----------------
            encodeAndAssertEquals("10100000 01100100 01100010 01100101 11100110",
                msg4);

            readMessageAndAssertEquals(msg1);
            readMessageAndAssertEquals(msg2);
            readMessageAndAssertEquals(msg3);
            readMessageAndAssertEquals(msg4);
        }

        private MessageTemplate registerTemplate(Field field)
        {
            var messageTemplate = new MessageTemplate("", new[] { field });
            encodingContext.RegisterTemplate(113, messageTemplate);
            decodingContext.RegisterTemplate(113, messageTemplate);

            return messageTemplate;
        }

        private void readMessageAndAssertEquals(GroupValue msg1)
        {
            GroupValue readMessage = decoder.ReadMessage();
            Assert.AreEqual(msg1, readMessage);
        }

        private void encodeAndAssertEquals(String encoding, Message msg1)
        {
            byte[] encodedMessage = encoder.Encode(msg1);
            TestUtil.AssertBitVectorEquals(encoding, encodedMessage);

            try
            {
                output.Write(encodedMessage);
            }
            catch (IOException e)
            {
                throw new RuntimeException(e);
            }
        }
    }
}
