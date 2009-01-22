using System;

namespace OpenFAST.Session
{
	public class LocalEndpoint : Endpoint
	{
		virtual public ConnectionListener ConnectionListener
		{
			set
			{
				this.listener = value;
			}
			
		}
		
		private LocalEndpoint server;
		private ConnectionListener listener;
		private System.Collections.IList connections;
		
		public LocalEndpoint()
		{
			connections = new System.Collections.ArrayList(3);
		}
		
		public LocalEndpoint(LocalEndpoint server)
		{
			this.server = server;
		}
		
		public virtual void  Accept()
		{
			if (!(connections.Count == 0))
			{
				lock (this)
				{
					System.Object tempObject;
					tempObject = connections[0];
					connections.RemoveAt(0);
					Connection connection = (Connection) tempObject;
					listener.OnConnect(connection);
				}
			}
		}
		
		public virtual Connection Connect()
		{
			LocalConnection localConnection = new LocalConnection(server, this);
			LocalConnection remoteConnection = new LocalConnection(localConnection);
			lock (server)
			{
				server.connections.Add(remoteConnection);
			}
			return localConnection;
		}
		
		public virtual void  Close()
		{
		}
	}
}