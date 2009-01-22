using System;
using Connection = OpenFAST.Session.Connection;

namespace OpenFAST.Session.Tcp
{
	sealed class TcpConnection : Connection
	{
        System.IO.StreamReader in_stream ;
        System.IO.StreamWriter out_stream ;
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
                return out_stream;
			}
			
		}

        private System.Net.Sockets.TcpClient socket;
		
		public TcpConnection(System.Net.Sockets.TcpClient socket)
		{
			if (socket == null)
				throw new System.NullReferenceException();
			this.socket = socket;
            in_stream = new System.IO.StreamReader(socket.GetStream());
            out_stream = new System.IO.StreamWriter(socket.GetStream());

		}
		public void  Close()
		{
			try
			{
				socket.Close();
			}
			catch (System.IO.IOException)
			{
			}
		}
	}
}