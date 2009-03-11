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
namespace OpenFAST.Session
{
	public class LocalEndpoint : Endpoint
	{
		virtual public ConnectionListener ConnectionListener
		{
			set
			{
				listener = value;
			}
			
		}
		
		private readonly LocalEndpoint server;
		private ConnectionListener listener;
		private readonly System.Collections.IList connections;
		
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
				    object tempObject = connections[0];
					connections.RemoveAt(0);
					var connection = (Connection) tempObject;
					listener.OnConnect(connection);
				}
			}
		}
		
		public virtual Connection Connect()
		{
			var localConnection = new LocalConnection();
			var remoteConnection = new LocalConnection(localConnection);
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