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
using System.Threading;
using OpenFAST.Error;

namespace OpenFAST.Session
{
    public sealed class FastServer : ConnectionListener
    {
        private readonly IEndpoint endpoint;
        private readonly string serverName;
        private readonly ISessionProtocol sessionProtocol;
        private IErrorHandler errorHandler = ErrorHandler_Fields.DEFAULT;
        private bool listening;
        private SupportClass.ThreadClass serverThread;
        private ISessionHandler sessionHandler = SessionHandlerFields.Null;

        public FastServer(string serverName, ISessionProtocol sessionProtocol, IEndpoint endpoint)
        {
            if (endpoint == null || sessionProtocol == null)
            {
                throw new NullReferenceException();
            }
            this.endpoint = endpoint;
            this.sessionProtocol = sessionProtocol;
            this.serverName = serverName;
            endpoint.ConnectionListener = this;
        }

        public IErrorHandler ErrorHandler
        {
            // ************* OPTIONAL DEPENDENCY SETTERS **************
            set
            {
                if (value == null)
                {
                    throw new NullReferenceException();
                }

                errorHandler = value;
            }
        }

        public ISessionHandler SessionHandler
        {
            set { sessionHandler = value; }
        }

        public void Listen()
        {
            listening = true;
            if (serverThread == null)
            {
                IThreadRunnable runnable = new FastServerThread(this);
                serverThread = new SupportClass.ThreadClass(new ThreadStart(runnable.Run), "FastServer");
            }
            serverThread.Start();
        }

        public void Close()
        {
            listening = false;
            endpoint.Close();
        }

        public override void OnConnect(IConnection connection)
        {
            Session session = sessionProtocol.OnNewConnection(serverName, connection);
            sessionHandler.NewSession(session);
        }

        #region Nested type: FastServerThread

        private class FastServerThread : IThreadRunnable
        {
            private FastServer enclosingInstance;

            public FastServerThread(FastServer enclosingInstance)
            {
                InitBlock(enclosingInstance);
            }

            public FastServer Enclosing_Instance
            {
                get { return enclosingInstance; }
            }

            #region IThreadRunnable Members

            public virtual void Run()
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
                        Thread.Sleep(new TimeSpan((Int64) 10000*20));
                    }
                    catch (ThreadInterruptedException)
                    {
                    }
                }
            }

            #endregion

            private void InitBlock(FastServer internalInstance)
            {
                enclosingInstance = internalInstance;
            }
        }

        #endregion
    }
}