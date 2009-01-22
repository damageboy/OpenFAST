using System;

namespace OpenFAST.Session
{
	public sealed class FastClient
	{
		private string clientName;
		private Endpoint endpoint;
		private SessionProtocol sessionProtocol;
		
		public FastClient(string clientName, SessionProtocol sessionProtocol, Endpoint endpoint)
		{
			this.clientName = clientName;
			this.sessionProtocol = sessionProtocol;
			this.endpoint = endpoint;
		}
		public Session Connect()
		{
			Connection connection = endpoint.Connect();
			Session session = sessionProtocol.Connect(clientName, connection);
			return session;
		}
	}
}