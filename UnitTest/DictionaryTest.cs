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
using OpenFAST.Session;
using OpenFAST.Template;
using OpenFAST.Template.Operator;
using OpenFAST.Template.Type;
using UnitTest.Test;

namespace UnitTest
{
    [TestFixture]
    public class DictionaryTest
    {
        #region Setup/Teardown

        [SetUp]
        protected void SetUp()
        {
            _output = new StreamWriter(new MemoryStream());
            _connection = new MyConnection(_output.BaseStream);
            _session = new OpenFAST.Session.Session(_connection,
                                   SessionConstants.SCP_1_0,
                                   TemplateRegistryFields.Null, TemplateRegistryFields.Null);
        }

        [TearDown]
        protected void TearDown()
        {
            // TODO: test proper session closing
            _session.Close();
        }

        #endregion

        private OpenFAST.Session.Session _session;
        private StreamWriter _output;
        private MyConnection _connection;

        private class MyConnection : IConnection
        {
            private readonly StreamReader _inStream;
            private readonly StreamWriter _outStream;

            public MyConnection(Stream outStream)
            {
                _outStream = new StreamWriter(outStream);
                _inStream = new StreamReader(outStream);
            }

            #region IConnection Members

            public StreamReader InputStream
            {
                get { return _inStream; }
            }

            public StreamWriter OutputStream
            {
                get { return _outStream; }
            }

            public void Close()
            {
                _inStream.Close();
                _outStream.Close();
            }

            #endregion
        }

        [Test]
        public void TestMultipleDictionaryTypes()
        {
            var bid = new Scalar("bid", FASTType.DECIMAL, Operator.COPY, ScalarValue.Undefined, false)
                          {Dictionary = DictionaryFields.Template};

            var quote = new MessageTemplate("quote", new Field[] {bid});

            var bidR = new Scalar("bid", FASTType.DECIMAL, Operator.COPY, ScalarValue.Undefined, false);
            var request = new MessageTemplate("request",
                                              new Field[] {bidR});

            var quote1 = new Message(quote);
            quote1.SetFieldValue(1, new DecimalValue(10.2));

            var request1 = new Message(request);
            request1.SetFieldValue(1, new DecimalValue(10.3));

            var quote2 = new Message(quote);
            quote2.SetFieldValue(1, new DecimalValue(10.2));

            var request2 = new Message(request);
            request2.SetFieldValue(1, new DecimalValue(10.2));

            _session.MessageOutputStream.RegisterTemplate(1, request);
            _session.MessageOutputStream.RegisterTemplate(2, quote);
            _session.MessageOutputStream.WriteMessage(quote1);
            _session.MessageOutputStream.WriteMessage(request1);
            _session.MessageOutputStream.WriteMessage(quote2);
            _session.MessageOutputStream.WriteMessage(request2);

            const string expected = "11100000 10000010 11111111 00000000 11100110 " +
                                    "11100000 10000001 11111111 00000000 11100111 " +
                                    "11000000 10000010 " +
                                    "11100000 10000001 11111111 00000000 11100110";
            TestUtil.AssertBitVectorEquals(expected, TestUtil.ToByte(_output));
        }
    }
}