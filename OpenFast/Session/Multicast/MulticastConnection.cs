using System;
using Connection = OpenFAST.Session.Connection;

namespace OpenFAST.Session.Multicast
{
	public sealed class MulticastConnection : Connection
	{
        System.IO.StreamReader in_stream;

        public System.IO.StreamReader InputStream
		{
			get
			{
                return in_stream;
			}
			
		}
		public System.IO.StreamWriter OutputStream
		{
			get
			{
				throw new System.NotSupportedException("Multicast sending not currently supported.");
			}
			
		}
		private System.Net.Sockets.UdpClient socket;
		private System.Net.IPAddress group;
		
		public MulticastConnection(System.Net.Sockets.UdpClient socket, System.Net.IPAddress group)
		{
			this.socket = socket;
			this.group = group;
            in_stream = new System.IO.StreamReader(new MulticastInputStream(socket));
		}
		
		public void  Close()
		{
			try
			{
				socket.DropMulticastGroup((System.Net.IPAddress) group);
				socket.Close();
			}
			catch (System.IO.IOException)
			{
			}
		}
	}
}