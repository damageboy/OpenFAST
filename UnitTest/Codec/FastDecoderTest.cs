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
using NUnit.Framework;
using OpenFAST.Template;
using OpenFAST;
using System.IO;
using OpenFAST.Codec;
using OpenFAST.Template.Type;
using OpenFAST.Template.operator_Renamed;

namespace UnitTest.Codec
{
    [TestFixture]
    public class FastDecoderTest
    {
        [Test]
        public void TestDecodeEmptyMessage()
        {
            MessageTemplate messageTemplate = new MessageTemplate("", new Field[] { });
            Stream input = ByteUtil.CreateByteStream("11000000 11110001");
            Context context = new Context();
            context.RegisterTemplate(113, messageTemplate);

            Message message = new FastDecoder(context, input).ReadMessage();
            Assert.AreEqual(113, message.GetInt(0));
        }
        [Test]
        public void TestDecodeSequentialEmptyMessages()
        {
            MessageTemplate messageTemplate = new MessageTemplate("", new Field[] { });
            Stream input = ByteUtil.CreateByteStream("11000000 11110001 10000000");
            Context context = new Context();
            context.RegisterTemplate(113, messageTemplate);

            FastDecoder decoder = new FastDecoder(context, input);
            GroupValue message = decoder.ReadMessage();
            GroupValue message2 = decoder.ReadMessage();
            Assert.AreEqual(113, message.GetInt(0));
            Assert.AreEqual(113, message2.GetInt(0));
        }

        public void testDecodeSimpleMessage()
        {
            MessageTemplate template = new MessageTemplate("",
                    new Field[] {
                    new Scalar("1", FASTType.U32, Operator.COPY, ScalarValue.UNDEFINED, false)
                });
            Stream input = ByteUtil.CreateByteStream("11100000 11110001 10000001");
            Context context = new Context();
            context.RegisterTemplate(113, template);

            Message message = new Message(template);
            message.SetInteger(1, 1);

            FastDecoder decoder = new FastDecoder(context, input);
            GroupValue readMessage = decoder.ReadMessage();
            Assert.AreEqual(message, readMessage);
            Assert.AreEqual(readMessage, message);
        }
        [Test]
        public void TestDecodeMessageWithAllFieldTypes()
        {
            //               --PMAP-- --TID--- ---#1--- -------#2-------- ------------#3------------ ---#4--- ------------#5------------ ---#6---
            String msgstr = "11111111 11110001 11001000 10000001 11111111 11111101 00001001 10110001 11111111 01100001 01100010 11100011 10000010";
            Stream input = ByteUtil.CreateByteStream(msgstr);

            MessageTemplate template = new MessageTemplate("",
                    new Field[] {
                    new Scalar("1", FASTType.ASCII, Operator.COPY, ScalarValue.UNDEFINED, false),
                    new Scalar("2", FASTType.BYTE_VECTOR, Operator.COPY, ScalarValue.UNDEFINED, false),
                    new Scalar("3", FASTType.DECIMAL, Operator.COPY, ScalarValue.UNDEFINED, false),
                    new Scalar("4", FASTType.I32, Operator.COPY, ScalarValue.UNDEFINED, false),
                    new Scalar("5", FASTType.ASCII, Operator.COPY, ScalarValue.UNDEFINED, false),
                    new Scalar("6", FASTType.U32, Operator.COPY, ScalarValue.UNDEFINED, false),
                });
            Context context = new Context();
            context.RegisterTemplate(113, template);

            GroupValue message = new Message(template);
            message.SetString(1, "H");
            message.SetByteVector(2, new byte[] { (byte)0xFF });
            message.SetDecimal(3, 1.201);
            message.SetInteger(4, -1);
            message.SetString(5, "abc");
            message.SetInteger(6, 2);
            Assert.AreEqual(message, new FastDecoder(context, input).ReadMessage());
        }
        [Test]
        public void TestDecodeMessageWithSignedIntegerFieldTypesAndAllOperators()
        {
            MessageTemplate template = new MessageTemplate("",
                    new Field[] {
                    new Scalar("1", FASTType.I32, Operator.COPY, ScalarValue.UNDEFINED, false),
                    new Scalar("2", FASTType.I32, Operator.DELTA, ScalarValue.UNDEFINED, false),
                    new Scalar("3", FASTType.I32, Operator.INCREMENT,
                        new IntegerValue(10), false),
                    new Scalar("4", FASTType.I32, Operator.INCREMENT,
                        ScalarValue.UNDEFINED, false),
                    new Scalar("5", FASTType.I32, Operator.CONSTANT,
                        new IntegerValue(1), false), /* NON-TRANSFERRABLE */
                new Scalar("6", FASTType.I32, Operator.DEFAULT,
                        new IntegerValue(2), false)
                });

            GroupValue message = new Message(template);
            message.SetInteger(1, 109);
            message.SetInteger(2, 29470);
            message.SetInteger(3, 10);
            message.SetInteger(4, 3);
            message.SetInteger(5, 1);
            message.SetInteger(6, 2);

            //             --PMAP-- --TID--- --------#1------- ------------#2------------ ---#4---
            String msg1 = "11101000 11110001 00000000 11101101 00000001 01100110 10011110 10000011";

            //             --PMAP-- ---#2--- ---#6---
            String msg2 = "10000100 11111111 10000011";

            //             --PMAP-- --------#1------- --------#2------- ---#4--- ---#6---
            String msg3 = "10101100 00000000 11100000 00001000 10000111 10000001 10000011";

            Stream input = ByteUtil.CreateByteStream(msg1 + ' ' + msg2 + ' ' +
                    msg3);
            Context context = new Context();
            context.RegisterTemplate(113, template);

            FastDecoder decoder = new FastDecoder(context, input);

            Message readMessage = decoder.ReadMessage();
            Assert.AreEqual(message, readMessage);

            message.SetInteger(2, 29469);
            message.SetInteger(3, 11);
            message.SetInteger(4, 4);
            message.SetInteger(6, 3);

            readMessage = decoder.ReadMessage();
            Assert.AreEqual(message, readMessage);

            message.SetInteger(1, 96);
            message.SetInteger(2, 30500);
            message.SetInteger(3, 12);
            message.SetInteger(4, 1);

            readMessage = decoder.ReadMessage();
            Assert.AreEqual(message, readMessage);
        }
        [Test]
        public void TestDecodeMessageWithUnsignedIntegerFieldTypesAndAllOperators()
        {
            MessageTemplate template = new MessageTemplate("",
                    new Field[] {
                    new Scalar("1", FASTType.U32, Operator.COPY, ScalarValue.UNDEFINED, false),
                    new Scalar("2", FASTType.U32, Operator.DELTA, ScalarValue.UNDEFINED, false),
                    new Scalar("3", FASTType.U32, Operator.INCREMENT,
                        new IntegerValue(10), false),
                    new Scalar("4", FASTType.U32, Operator.INCREMENT,
                        ScalarValue.UNDEFINED, false),
                    new Scalar("5", FASTType.U32, Operator.CONSTANT,
                        new IntegerValue(1), false), /* NON-TRANSFERRABLE */
                new Scalar("6", FASTType.U32, Operator.DEFAULT,
                        new IntegerValue(2), false)
                });

            GroupValue message = new Message(template);
            message.SetInteger(1, 109);
            message.SetInteger(2, 29470);
            message.SetInteger(3, 10);
            message.SetInteger(4, 3);
            message.SetInteger(5, 1);
            message.SetInteger(6, 2);

            //             --PMAP-- --TID--- ---#1--- ------------#2------------ ---#4---
            String msg1 = "11101000 11110001 11101101 00000001 01100110 10011110 10000011";

            //             --PMAP-- ---#2--- ---#6---
            String msg2 = "10000100 11111111 10000011";

            //             --PMAP-- ---#1--- --------#2------- ---#4--- ---#6---
            String msg3 = "10101100 11100000 00001000 10000111 10000001 10000011";

            Stream input = ByteUtil.CreateByteStream(msg1 + ' ' + msg2 + ' ' +
                    msg3);
            Context context = new Context();
            context.RegisterTemplate(113, template);

            FastDecoder decoder = new FastDecoder(context, input);

            Message readMessage = decoder.ReadMessage();
            Assert.AreEqual(message, readMessage);

            message.SetInteger(2, 29469);
            message.SetInteger(3, 11);
            message.SetInteger(4, 4);
            message.SetInteger(6, 3);

            readMessage = decoder.ReadMessage();
            Assert.AreEqual(message, readMessage);

            message.SetInteger(1, 96);
            message.SetInteger(2, 30500);
            message.SetInteger(3, 12);
            message.SetInteger(4, 1);

            readMessage = decoder.ReadMessage();
            Assert.AreEqual(message, readMessage);
        }

        public void testDecodeMessageWithStringFieldTypesAndAllOperators()
        {
            MessageTemplate template = new MessageTemplate("",
                    new Field[] {
                    new Scalar("1", FASTType.ASCII, Operator.COPY, ScalarValue.UNDEFINED, false),
                    new Scalar("2", FASTType.ASCII, Operator.DELTA, ScalarValue.UNDEFINED, false),
                    new Scalar("3", FASTType.ASCII, Operator.CONSTANT,
                        new StringValue("e"), false), /* NON-TRANSFERRABLE */
                new Scalar("4", FASTType.ASCII, Operator.DEFAULT,
                        new StringValue("long"), false)
                });

            Message message = new Message(template);
            message.SetString(1, "on");
            message.SetString(2, "DCB32");
            message.SetString(3, "e");
            message.SetString(4, "long");

            //             --PMAP-- --TID--- --------#1------- ---------------------#2---------------------
            String msg1 = "11100000 11110001 01101111 11101110 10000000 01000100 01000011 01000010 00110011 10110010";

            //             --PMAP-- ------------#2------------ ---------------------#4---------------------
            String msg2 = "10010000 10000010 00110001 10110110 01110011 01101000 01101111 01110010 11110100";

            Stream input = ByteUtil.CreateByteStream(msg1 + ' ' + msg2);
            Context context = new Context();
            context.RegisterTemplate(113, template);

            FastDecoder decoder = new FastDecoder(context, input);

            Message readMessage = decoder.ReadMessage();
            Assert.AreEqual(message, readMessage);

            message.SetString(2, "DCB16");
            message.SetString(4, "short");

            readMessage = decoder.ReadMessage();
            Assert.AreEqual(message, readMessage);
        }
        //[Test]
        //public void testDecodeEndOfStream() {
        //    FastDecoder decoder = new FastDecoder(new Context(), new InputStream() {
        //        public int read() throws IOException {
        //            return -1;
        //        }});

        //    Message message = decoder.readMessage();
        //    assertNull(message);
        //}
    }

}
