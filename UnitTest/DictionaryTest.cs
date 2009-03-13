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
using OpenFAST.Session;
using System.IO;
using NUnit.Framework;
using OpenFAST.Template.Type;
using OpenFAST.Template;
using OpenFAST;
using UnitTest.Test;
using OpenFAST.Template.Operator;

namespace UnitTest
{
    [TestFixture]
    public class DictionaryTest
    {
        private Session session;
        private StreamWriter output;
        public class MyConnection : Connection
        {
            readonly StreamWriter outStream;
            readonly StreamReader inStream;

            public MyConnection(Stream outStream)
            {
                this.outStream = new StreamWriter(outStream);
                inStream = new StreamReader(outStream);

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
        var bid = new Scalar("bid", FASTType.DECIMAL, Operator.COPY, ScalarValue.UNDEFINED, false)
                      {Dictionary = Dictionary_Fields.TEMPLATE};

            var quote = new MessageTemplate("quote", new Field[] { bid });

        var bidR = new Scalar("bid", FASTType.DECIMAL, Operator.COPY, ScalarValue.UNDEFINED, false);
        var request = new MessageTemplate("request",
                new Field[] { bidR });

        var quote1 = new Message(quote);
        quote1.SetFieldValue(1, new DecimalValue(10.2));

        var request1 = new Message(request);
        request1.SetFieldValue(1, new DecimalValue(10.3));

        var quote2 = new Message(quote);
        quote2.SetFieldValue(1, new DecimalValue(10.2));

        var request2 = new Message(request);
        request2.SetFieldValue(1, new DecimalValue(10.2));

        session.MessageOutputStream.RegisterTemplate(1, request);
        session.MessageOutputStream.RegisterTemplate(2, quote);
        session.MessageOutputStream.WriteMessage(quote1);
        session.MessageOutputStream.WriteMessage(request1);
        session.MessageOutputStream.WriteMessage(quote2);
        session.MessageOutputStream.WriteMessage(request2);

        const string expected = "11100000 10000010 11111111 00000000 11100110 " +
                                "11100000 10000001 11111111 00000000 11100111 " +
                                "11000000 10000010 " +
                                "11100000 10000001 11111111 00000000 11100110";
        TestUtil.AssertBitVectorEquals(expected,TestUtil.ToByte( output));
    }
    }
}
