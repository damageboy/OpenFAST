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
namespace OpenFAST.Session.Tcp
{
    public sealed class TcpEndpoint : Endpoint
    {
        public ConnectionListener ConnectionListener
        {
            set
            {
                connectionListener = value;
            }

        }

        private readonly int port;
        private readonly string host;
        private ConnectionListener connectionListener = ConnectionListener.NULL;
        private System.Net.Sockets.TcpListener serverSocket;
        private bool closed = true;

        public TcpEndpoint(int port)
        {
            this.port = port;
        }
        public TcpEndpoint(string host, int port)
            : this(port)
        {
            this.host = host;
        }
        public Connection Connect()
        {
            try
            {
                var socket = new System.Net.Sockets.TcpClient(host, port);
                Connection connection = new TcpConnection(socket);
                return connection;
            }
            catch (System.IO.IOException e)
            {
                throw new FastConnectionException(e);
            }
            catch (System.Exception e)
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
                    var temp_tcpListener = new System.Net.Sockets.TcpListener(System.Net.IPAddress.Parse("0.0.0.0"), port);

                    temp_tcpListener.Start();
                    serverSocket = temp_tcpListener;
                }
                while (true)
                {
                    var socket = serverSocket.AcceptTcpClient();
                    connectionListener.OnConnect(new TcpConnection(socket));
                }
            }
            catch (System.IO.IOException e)
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
                catch (System.IO.IOException)
                {
                }
            serverSocket = null;
        }
    }
}