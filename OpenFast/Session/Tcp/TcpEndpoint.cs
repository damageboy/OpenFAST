using System;
using Connection = OpenFAST.Session.Connection;
using Endpoint = OpenFAST.Session.Endpoint;
using FastConnectionException = OpenFAST.Session.FastConnectionException;

namespace OpenFAST.Session.Tcp
{
    public sealed class TcpEndpoint : Endpoint
    {
        public ConnectionListener ConnectionListener
        {
            set
            {
                this.connectionListener = value;
            }

        }

        private int port;
        private string host;
        private ConnectionListener connectionListener = OpenFAST.Session.ConnectionListener.NULL;
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
                System.Net.Sockets.TcpClient socket = new System.Net.Sockets.TcpClient(host, port);
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
                    System.Net.Sockets.TcpListener temp_tcpListener;
                    //commented by shariq
                    //temp_tcpListener = new System.Net.Sockets.TcpListener(System.Net.Dns.GetHostByName(System.Net.Dns.GetHostName()).AddressList[0], port);
                    //listen on all adapters
                    temp_tcpListener = new System.Net.Sockets.TcpListener(System.Net.IPAddress.Parse("0.0.0.0"), port);

                    temp_tcpListener.Start();
                    serverSocket = temp_tcpListener;
                }
                while (true)
                {
                    System.Net.Sockets.TcpClient socket = serverSocket.AcceptTcpClient();
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