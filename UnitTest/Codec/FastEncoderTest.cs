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
using System.Collections.Generic;
using System.Text;
using UnitTest.Test;
using OpenFAST.Template;
using OpenFAST;
using OpenFAST.Codec;
using NUnit.Framework;
using OpenFAST.Template.Type;
using OpenFAST.Template.operator_Renamed;

namespace UnitTest.Codec
{
    [TestFixture]
    public class FastEncoderTest : OpenFastTestCase
    {
        [Test]
        public void TestEncodeEmptyMessage()
        {
            MessageTemplate messageTemplate = new MessageTemplate("", new Field[] { });
            Message message = new Message(messageTemplate);
            Context context = new Context();
            context.RegisterTemplate(113, messageTemplate);

            byte[] encoding = new FastEncoder(context).Encode(message);
            AssertEquals("11000000 11110001", encoding);
        }
        [Test]
        public void TestEncodeSequentialEmptyMessages()
        {
            MessageTemplate messageTemplate = new MessageTemplate("", new Field[] { });
            Message message = new Message(messageTemplate);
            Message nextMsg = new Message(messageTemplate);
            Context context = new Context();
            context.RegisterTemplate(113, messageTemplate);

            FastEncoder encoder = new FastEncoder(context);

            // Presence map should show that the only field present is the template id.
            AssertEquals("11000000 11110001",
                encoder.Encode(message));
            // Presence map should be empty (except for leading stop bit)
            AssertEquals("10000000", encoder.Encode(nextMsg));
        }
        [Test]
        public void TestEncodeSimpleMessage()
        {
            MessageTemplate template = new MessageTemplate("",
                    new Field[] {
                    new Scalar("1", FASTType.U32, Operator.COPY, ScalarValue.UNDEFINED, false)
                });
            Context context = new Context();
            context.RegisterTemplate(113, template);

            Message message = new Message(template);
            message.SetInteger(1, 1);

            FastEncoder encoder = new FastEncoder(context);
            AssertEquals("11100000 11110001 10000001", encoder.Encode(message));
        }
        [Test]
        public void TestEncodeMessageWithAllFieldTypes()
        {
            MessageTemplate template = new MessageTemplate("",
                    new Field[] {
                    new Scalar("1", FASTType.STRING, Operator.COPY, ScalarValue.UNDEFINED, false),
                    new Scalar("2", FASTType.BYTE_VECTOR, Operator.COPY, ScalarValue.UNDEFINED, false),
                    new Scalar("3", FASTType.DECIMAL, Operator.COPY, ScalarValue.UNDEFINED, false),
                    new Scalar("4", FASTType.I32, Operator.COPY, ScalarValue.UNDEFINED, false),
                    new Scalar("5", FASTType.STRING, Operator.COPY, ScalarValue.UNDEFINED, false),
                    new Scalar("6", FASTType.U32, Operator.COPY, ScalarValue.UNDEFINED, false),
                });
            Context context = new Context();
            context.RegisterTemplate(113, template);

            Message message = new Message(template);
            message.SetString(1, "H");
            message.SetByteVector(2, new byte[] { (byte)0xFF });
            message.SetDecimal(3, 1.201);
            message.SetInteger(4, -1);
            message.SetString(5, "abc");
            message.SetInteger(6, 2);

            //               --PMAP-- --TID--- ---#1--- -------#2-------- ------------#3------------ ---#4--- ------------#5------------ ---#6---
            String msgstr = "11111111 11110001 11001000 10000001 11111111 11111101 00001001 10110001 11111111 01100001 01100010 11100011 10000010";
            AssertEquals(msgstr, new FastEncoder(context).Encode(message));
        }
        [Test]
        public void TestEncodeMessageWithOverlongPmap()
        {
            MessageTemplate template = new MessageTemplate("",
                    new Field[] {
                    new Scalar("1", FASTType.U32, Operator.COPY, new IntegerValue(1), false),
                    new Scalar("1", FASTType.U32, Operator.COPY, new IntegerValue(1), false),
                    new Scalar("1", FASTType.U32, Operator.COPY, new IntegerValue(1), false),
                    new Scalar("1", FASTType.U32, Operator.COPY, new IntegerValue(1), false),
                    new Scalar("1", FASTType.U32, Operator.COPY, new IntegerValue(1), false),
                    new Scalar("1", FASTType.U32, Operator.COPY, new IntegerValue(1), false),
                    new Scalar("1", FASTType.U32, Operator.COPY, new IntegerValue(1), false)
                });

            Context context = new Context();
            context.RegisterTemplate(113, template);

            Message message = new Message(template);
            message.SetInteger(1, 1);
            message.SetInteger(2, 1);
            message.SetInteger(3, 1);
            message.SetInteger(4, 1);
            message.SetInteger(5, 1);
            message.SetInteger(6, 1);
            message.SetInteger(7, 1);

            //               --PMAP-- --PMAP-- --PMAP-- --TID---
            //WHAT IT THINKS 01000000 00000000 10000000 11110001
            String msgstr = "11000000 11110001";

            AssertEquals(msgstr, new FastEncoder(context).Encode(message));
        }
        [Test]
        public void TestEncodeMessageWithSignedIntegerFieldTypesAndAllOperators()
        {
            MessageTemplate template = new MessageTemplate("",
                    new Field[] {
                    new Scalar("1", FASTType.I32, Operator.COPY, ScalarValue.UNDEFINED, false),
                    new Scalar("2", FASTType.I32, Operator.DELTA, ScalarValue.UNDEFINED, false),
                    new Scalar("3", FASTType.I32, Operator.INCREMENT, new IntegerValue(10), false),
                    new Scalar("4", FASTType.I32, Operator.INCREMENT, ScalarValue.UNDEFINED, false),
                    new Scalar("5", FASTType.I32, Operator.CONSTANT, new IntegerValue(1), false), /* NON-TRANSFERRABLE */
                    new Scalar("6", FASTType.I32, Operator.DEFAULT, new IntegerValue(2), false)
                });
            Context context = new Context();
            context.RegisterTemplate(113, template);

            FastEncoder encoder = new FastEncoder(context);

            Message message = new Message(template);
            message.SetInteger(1, 109);
            message.SetInteger(2, 29470);
            message.SetInteger(3, 10);
            message.SetInteger(4, 3);
            message.SetInteger(5, 1);
            message.SetInteger(6, 2);

            //             --PMAP-- --TID--- --------#1------- ------------#2------------ ---#4---
            String msg1 = "11101000 11110001 00000000 11101101 00000001 01100110 10011110 10000011";
            TestUtil.AssertBitVectorEquals(msg1, encoder.Encode(message));

            message.SetInteger(2, 29469);
            message.SetInteger(3, 11);
            message.SetInteger(4, 4);
            message.SetInteger(6, 3);

            //             --PMAP-- ---#2--- ---#6---
            String msg2 = "10000100 11111111 10000011";
            TestUtil.AssertBitVectorEquals(msg2, encoder.Encode(message));

            message.SetInteger(1, 96);
            message.SetInteger(2, 30500);
            message.SetInteger(3, 12);
            message.SetInteger(4, 1);

            //             --PMAP-- --------#1------- --------#2------- ---#4--- ---#6---
            String msg3 = "10101100 00000000 11100000 00001000 10000111 10000001 10000011";
            AssertEquals(msg3, encoder.Encode(message));
        }
        [Test]
        public void TestEncodeMessageWithUnsignedIntegerFieldTypesAndAllOperators()
        {
            MessageTemplate template = new MessageTemplate("",
                    new Field[] {
                    new Scalar("1", FASTType.U32, Operator.COPY, ScalarValue.UNDEFINED, false),
                    new Scalar("2", FASTType.U32, Operator.DELTA, ScalarValue.UNDEFINED, false),
                    new Scalar("3", FASTType.I32, Operator.INCREMENT, new IntegerValue(10), false),
                    new Scalar("4", FASTType.I32, Operator.INCREMENT, ScalarValue.UNDEFINED, false),
                    new Scalar("5", FASTType.I32, Operator.CONSTANT, new IntegerValue(1), false), /* NON-TRANSFERRABLE */
                    new Scalar("6", FASTType.I32, Operator.DEFAULT, new IntegerValue(2), false)
                });
            Context context = new Context();
            context.RegisterTemplate(113, template);

            FastEncoder encoder = new FastEncoder(context);

            Message message = new Message(template);
            message.SetInteger(1, 109);
            message.SetInteger(2, 29470);
            message.SetInteger(3, 10);
            message.SetInteger(4, 3);
            message.SetInteger(5, 1);
            message.SetInteger(6, 2);

            //             --PMAP-- --TID--- ---#1--- ------------#2------------ ---#4---
            String msg1 = "11101000 11110001 11101101 00000001 01100110 10011110 10000011";
            AssertEquals(msg1, encoder.Encode(message));

            message.SetInteger(2, 29471);
            message.SetInteger(3, 11);
            message.SetInteger(4, 4);
            message.SetInteger(6, 3);

            //             --PMAP-- ---#2--- ---#6---
            String msg2 = "10000100 10000001 10000011";
            AssertEquals(msg2, encoder.Encode(message));

            message.SetInteger(1, 96);
            message.SetInteger(2, 30500);
            message.SetInteger(3, 12);
            message.SetInteger(4, 1);

            //             --PMAP-- ---#1--- --------#2------- ---#4--- ---#6---
            String msg3 = "10101100 11100000 00001000 10000101 10000001 10000011";
            AssertEquals(msg3, encoder.Encode(message));
        }
        [Test]
        public void TestEncodeMessageWithStringFieldTypesAndAllOperators()
        {
            MessageTemplate template = new MessageTemplate("",
                    new Field[] {
                    new Scalar("1", FASTType.STRING, Operator.COPY, ScalarValue.UNDEFINED, false),
                    new Scalar("2", FASTType.STRING, Operator.DELTA, ScalarValue.UNDEFINED, false),
                    new Scalar("3", FASTType.STRING, Operator.CONSTANT, new StringValue("e"), false), /* NON-TRANSFERRABLE */
                    new Scalar("4", FASTType.STRING, Operator.DEFAULT, new StringValue("long"), false)
                });
            Context context = new Context();
            context.RegisterTemplate(113, template);

            Message message = new Message(template);
            message.SetString(1, "on");
            message.SetString(2, "DCB32");
            message.SetString(3, "e");
            message.SetString(4, "long");

            //             --PMAP-- --TID--- --------#1------- ---------------------#2------------------------------
            String msg1 = "11100000 11110001 01101111 11101110 10000000 01000100 01000011 01000010 00110011 10110010";

            //             --PMAP-- --------#2---------------- ---------------------#4---------------------
            String msg2 = "10010000 10000010 00110001 10110110 01110011 01101000 01101111 01110010 11110100";

            FastEncoder encoder = new FastEncoder(context);

            AssertEquals(msg1, encoder.Encode(message));

            message.SetString(2, "DCB16");
            message.SetString(4, "short");

            AssertEquals(msg2, encoder.Encode(message));
        }
    }

}
