using System;
using Connection = OpenFAST.Session.Connection;
using Endpoint = OpenFAST.Session.Endpoint;
using FastConnectionException = OpenFAST.Session.FastConnectionException;

namespace OpenFAST.Session.Multicast
{
	public sealed class MulticastEndpoint : Endpoint
	{
		public ConnectionListener ConnectionListener
		{
			set
			{
				throw new System.NotSupportedException();
			}
			
		}
		private int port;
		private string group;
		public MulticastEndpoint(int port, string group)
		{
			this.port = port;
			this.group = group;
		}
		public void  Accept()
		{
			throw new System.NotSupportedException();
		}
		public void  Close()
		{
		}
		public Connection Connect()
		{
			try
			{
				System.Net.Sockets.UdpClient socket = new System.Net.Sockets.UdpClient(port);
				System.Net.IPAddress groupAddress = System.Net.Dns.GetHostEntry(group).AddressList[0];
				socket.JoinMulticastGroup((System.Net.IPAddress) groupAddress);
				return new MulticastConnection(socket, groupAddress);
			}
			catch (System.IO.IOException e)
			{
				throw new FastConnectionException(e);
			}
		}
	}
}