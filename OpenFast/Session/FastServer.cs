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
using ErrorHandler = OpenFAST.Error.ErrorHandler;

namespace OpenFAST.Session
{
	public sealed class FastServer : ConnectionListener
	{
		private class FastServerThread : IThreadRunnable
		{
			public FastServerThread(FastServer enclosingInstance)
			{
				InitBlock(enclosingInstance);
			}
			private void  InitBlock(FastServer enclosingInstance)
			{
				this.enclosingInstance = enclosingInstance;
			}
			private FastServer enclosingInstance;
			public FastServer Enclosing_Instance
			{
				get
				{
					return enclosingInstance;
				}
				
			}
			public virtual void  Run()
			{
				while (Enclosing_Instance.listening)
				{
					try
					{
						Enclosing_Instance.endpoint.Accept();
					}
					catch (FastConnectionException e)
					{
						Enclosing_Instance.errorHandler.Error(null, null, e);
					}
					try
					{
						System.Threading.Thread.Sleep(new System.TimeSpan((System.Int64) 10000 * 20));
					}
					catch (System.Threading.ThreadInterruptedException)
					{
					}
				}
			}
		}
		public ErrorHandler ErrorHandler
		{
			// ************* OPTIONAL DEPENDENCY SETTERS **************
			set
			{
				if (value == null)
				{
					throw new System.NullReferenceException();
				}
				
				this.errorHandler = value;
			}
			
		}
		public SessionHandler SessionHandler
		{
			set
			{
				this.sessionHandler = value;
			}
			
		}
		private ErrorHandler errorHandler = OpenFAST.Error.ErrorHandler_Fields.DEFAULT;
		private SessionHandler sessionHandler = OpenFAST.Session.SessionHandler_Fields.NULL;
		private bool listening;
		
		private SessionProtocol sessionProtocol;
		private Endpoint endpoint;
		private string serverName;
		private SupportClass.ThreadClass serverThread;
		
		public FastServer(string serverName, SessionProtocol sessionProtocol, Endpoint endpoint)
		{
			if (endpoint == null || sessionProtocol == null)
			{
				throw new System.NullReferenceException();
			}
			this.endpoint = endpoint;
			this.sessionProtocol = sessionProtocol;
			this.serverName = serverName;
			endpoint.ConnectionListener = this;
		}
		
		public void  Listen()
		{
			listening = true;
			if (serverThread == null)
			{
				IThreadRunnable runnable = new FastServerThread(this);
				serverThread = new SupportClass.ThreadClass(new System.Threading.ThreadStart(runnable.Run), "FastServer");
			}
			serverThread.Start();
		}
		
		public void  Close()
		{
			listening = false;
			endpoint.Close();
		}
		
		public override void  OnConnect(Connection connection)
		{
			Session session = sessionProtocol.OnNewConnection(serverName, connection);
			this.sessionHandler.NewSession(session);
		}
	}
}