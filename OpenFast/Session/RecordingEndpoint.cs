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
using System.IO;
using OpenFAST.util;

namespace OpenFAST.Session
{
    public class RecordingEndpoint : IEndpoint
    {
        private readonly IEndpoint underlyingEndpoint;

        public RecordingEndpoint(IEndpoint endpoint)
        {
            underlyingEndpoint = endpoint;
        }

        #region Endpoint Members

        public virtual ConnectionListener ConnectionListener
        {
            set { underlyingEndpoint.ConnectionListener = value; }
        }

        public virtual void Accept()
        {
            underlyingEndpoint.Accept();
        }

        public virtual IConnection Connect()
        {
            IConnection connection = underlyingEndpoint.Connect();
            IConnection connectionWrapper = new RecordingConnection(connection);
            return connectionWrapper;
        }

        public virtual void Close()
        {
            underlyingEndpoint.Close();
        }

        #endregion

        #region Nested type: RecordingConnection

        private sealed class RecordingConnection : IConnection
        {
            private readonly StreamReader in_stream;
            private readonly StreamWriter out_stream;

            private readonly RecordingInputStream recordingInputStream;
            private readonly RecordingOutputStream recordingOutputStream;

            internal RecordingConnection(IConnection connection)
            {
                try
                {
                    recordingInputStream = new RecordingInputStream(connection.InputStream.BaseStream);
                    recordingOutputStream = new RecordingOutputStream(connection.OutputStream.BaseStream);
                    in_stream = new StreamReader(recordingInputStream);
                    out_stream = new StreamWriter(recordingOutputStream);
                }
                catch (IOException e)
                {
                    throw new RuntimeException(e);
                }
            }

            #region Connection Members

            public StreamReader InputStream
            {
                get { return in_stream; }
            }

            public StreamWriter OutputStream
            {
                get { return out_stream; }
            }

            public void Close()
            {
            }

            #endregion
        }

        #endregion
    }
}