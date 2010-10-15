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
    internal sealed class TcpConnection : IConnection
    {
        private readonly StreamReader _inputStream;
        private readonly StreamWriter _outputStream;
        private readonly TcpClient _socket;

        public TcpConnection(TcpClient socket)
        {
            if (socket == null)
                throw new NullReferenceException();
            _socket = socket;
            _inputStream = new StreamReader(socket.GetStream());
            _outputStream = new StreamWriter(socket.GetStream());
        }

        #region Connection Members

        public StreamReader InputStream
        {
            get { return _inputStream; }
        }

        public StreamWriter OutputStream
        {
            get { return _outputStream; }
        }

        public void Close()
        {
            try
            {
                _socket.Close();
            }
            catch (IOException)
            {
            }
        }

        #endregion
    }
}