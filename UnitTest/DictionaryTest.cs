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
using OpenFAST.Session;
using System.IO;
using NUnit.Framework;
using openfast.Template.Operator;
using OpenFAST.Template.Type;
using OpenFAST.Template;
using OpenFAST;
using UnitTest.Test;

namespace UnitTest
{
    [TestFixture]
    public class DictionaryTest
    {
        private Session session;
        private StreamWriter output;
        public class MyConnection : Connection
        {
            StreamWriter outStream;
            StreamReader inStream;

            public MyConnection(Stream outStream)
            {
                this.outStream = new StreamWriter(outStream);
                this.inStream = new StreamReader(outStream);

            }
            #region Connection Members

            public StreamReader InputStream
            {
                get { return inStream; }
            }

            public StreamWriter OutputStream
            {
                get { return outStream; }
            }

            public void Close()
            {
                try
                {
                    outStream.Close();
                }
                catch (IOException)
                {
                }
            }

            #endregion
        }
        [SetUp]
        protected void setUp()
        {
            output = new StreamWriter(new MemoryStream());
            session = new Session(new MyConnection(output.BaseStream), SessionConstants.SCP_1_0, TemplateRegistry_Fields.NULL, TemplateRegistry_Fields.NULL);
        }
        [Test]
        public void TestMultipleDictionaryTypes() {
        Scalar bid = new Scalar("bid", FASTType.DECIMAL, Operator.COPY, ScalarValue.UNDEFINED, false);
        bid.Dictionary = Dictionary_Fields.TEMPLATE;

        MessageTemplate quote = new MessageTemplate("quote", new Field[] { bid });

        Scalar bidR = new Scalar("bid", FASTType.DECIMAL, Operator.COPY, ScalarValue.UNDEFINED, false);
        MessageTemplate request = new MessageTemplate("request",
                new Field[] { bidR });

        Message quote1 = new Message(quote);
        quote1.SetFieldValue(1, new DecimalValue(10.2));

        Message request1 = new Message(request);
        request1.SetFieldValue(1, new DecimalValue(10.3));

        Message quote2 = new Message(quote);
        quote2.SetFieldValue(1, new DecimalValue(10.2));

        Message request2 = new Message(request);
        request2.SetFieldValue(1, new DecimalValue(10.2));

        session.MessageOutputStream.RegisterTemplate(1, request);
        session.MessageOutputStream.RegisterTemplate(2, quote);
        session.MessageOutputStream.WriteMessage(quote1);
        session.MessageOutputStream.WriteMessage(request1);
        session.MessageOutputStream.WriteMessage(quote2);
        session.MessageOutputStream.WriteMessage(request2);

        String expected = "11100000 10000010 11111111 00000000 11100110 " +
            "11100000 10000001 11111111 00000000 11100111 " +
            "11000000 10000010 " +
            "11100000 10000001 11111111 00000000 11100110";
        TestUtil.AssertBitVectorEquals(expected,TestUtil.ToByte( output));
    }
    }
}
