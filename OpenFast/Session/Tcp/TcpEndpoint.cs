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
using System.Net;
using System.Net.Sockets;

namespace OpenFAST.Session.Tcp
{
    public sealed class TcpEndpoint : IEndpoint
    {
        private readonly string _host;
        private readonly int _port;
        private bool _closed = true;
        private ConnectionListener _connectionListener = ConnectionListener.NULL;
        private TcpListener _serverSocket;

        public TcpEndpoint(int port)
        {
            _port = port;
        }

        public TcpEndpoint(string host, int port)
            : this(port)
        {
            _host = host;
        }

        #region Endpoint Members

        public ConnectionListener ConnectionListener
        {
            set { _connectionListener = value; }
        }

        public IConnection Connect()
        {
            try
            {
                var socket = new TcpClient(_host, _port);
                IConnection connection = new TcpConnection(socket);
                return connection;
            }
            catch (IOException e)
            {
                throw new FastConnectionException(e);
            }
            catch (Exception e)
            {
                throw new FastConnectionException(e);
            }
        }

        public void Accept()
        {
            _closed = false;
            try
            {
                if (_serverSocket == null)
                {
                    //commented by shariq
                    //tmp = new System.Net.Sockets.TcpListener(System.Net.Dns.GetHostByName(System.Net.Dns.GetHostName()).AddressList[0], port);
                    //listen on all adapters
                    var tmp = new TcpListener(IPAddress.Parse("0.0.0.0"), _port);
                    tmp.Start();
                    _serverSocket = tmp;
                }
                while (true)
                {
                    TcpClient socket = _serverSocket.AcceptTcpClient();
                    _connectionListener.OnConnect(new TcpConnection(socket));
                }
            }
            catch (IOException e)
            {
                if (!_closed)
                    throw new FastConnectionException(e);
            }
        }

        public void Close()
        {
            _closed = true;
            if (_serverSocket != null)
                try
                {
                    _serverSocket.Stop();
                }
                catch (IOException)
                {
                }
            _serverSocket = null;
        }

        #endregion
    }
}