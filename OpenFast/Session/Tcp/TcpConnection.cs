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
using System.IO;
using System.Net.Sockets;

namespace OpenFAST.Session.Tcp
{
    internal sealed class TcpConnection : Connection
    {
        private readonly StreamReader in_stream;
        private readonly StreamWriter out_stream;

        private readonly TcpClient socket;

        public TcpConnection(TcpClient socket)
        {
            if (socket == null)
                throw new NullReferenceException();
            this.socket = socket;
            in_stream = new StreamReader(socket.GetStream());
            out_stream = new StreamWriter(socket.GetStream());
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
            try
            {
                socket.Close();
            }
            catch (IOException)
            {
            }
        }

        #endregion
    }
}