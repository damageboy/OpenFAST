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