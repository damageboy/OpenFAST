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
using OpenFAST.Utility;

namespace OpenFAST.Session
{
    public class RecordingEndpoint : IEndpoint
    {
        private readonly IEndpoint _underlyingEndpoint;

        public RecordingEndpoint(IEndpoint endpoint)
        {
            _underlyingEndpoint = endpoint;
        }

        #region IEndpoint Members

        public virtual ConnectionListener ConnectionListener
        {
            set { _underlyingEndpoint.ConnectionListener = value; }
        }

        public virtual void Accept()
        {
            _underlyingEndpoint.Accept();
        }

        public virtual IConnection Connect()
        {
            IConnection connection = _underlyingEndpoint.Connect();
            IConnection connectionWrapper = new RecordingConnection(connection);
            return connectionWrapper;
        }

        public virtual void Close()
        {
            _underlyingEndpoint.Close();
        }

        #endregion

        #region Nested type: RecordingConnection

        private sealed class RecordingConnection : IConnection
        {
            private readonly StreamReader _inStream;
            private readonly StreamWriter _outStream;

            private readonly RecordingInputStream _recordingInputStream;
            private readonly RecordingOutputStream _recordingOutputStream;

            internal RecordingConnection(IConnection connection)
            {
                try
                {
                    _recordingInputStream = new RecordingInputStream(connection.InputStream.BaseStream);
                    _recordingOutputStream = new RecordingOutputStream(connection.OutputStream.BaseStream);
                    _inStream = new StreamReader(_recordingInputStream);
                    _outStream = new StreamWriter(_recordingOutputStream);
                }
                catch (IOException e)
                {
                    throw new RuntimeException(e);
                }
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
            }

            #endregion
        }

        #endregion
    }
}