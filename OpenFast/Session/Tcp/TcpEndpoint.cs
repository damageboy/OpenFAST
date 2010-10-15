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
    public sealed class TcpEndpoint : Endpoint
    {
        private readonly string host;
        private readonly int port;
        private bool closed = true;
        private ConnectionListener connectionListener = ConnectionListener.NULL;
        private TcpListener serverSocket;

        public TcpEndpoint(int port)
        {
            this.port = port;
        }

        public TcpEndpoint(string host, int port)
            : this(port)
        {
            this.host = host;
        }

        #region Endpoint Members

        public ConnectionListener ConnectionListener
        {
            set { connectionListener = value; }
        }

        public Connection Connect()
        {
            try
            {
                var socket = new TcpClient(host, port);
                Connection connection = new TcpConnection(socket);
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
            closed = false;
            try
            {
                if (serverSocket == null)
                {
                    //commented by shariq
                    //temp_tcpListener = new System.Net.Sockets.TcpListener(System.Net.Dns.GetHostByName(System.Net.Dns.GetHostName()).AddressList[0], port);
                    //listen on all adapters
                    var temp_tcpListener = new TcpListener(IPAddress.Parse("0.0.0.0"),
                                                           port);

                    temp_tcpListener.Start();
                    serverSocket = temp_tcpListener;
                }
                while (true)
                {
                    TcpClient socket = serverSocket.AcceptTcpClient();
                    connectionListener.OnConnect(new TcpConnection(socket));
                }
            }
            catch (IOException e)
            {
                if (!closed)
                    throw new FastConnectionException(e);
            }
        }

        public void Close()
        {
            closed = true;
            if (serverSocket != null)
                try
                {
                    serverSocket.Stop();
                }
                catch (IOException)
                {
                }
            serverSocket = null;
        }

        #endregion
    }
}