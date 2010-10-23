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
                Yuri Astrakhan <FirstName><LastName>@gmail.com
*/
using System;
using System.Threading;
using OpenFAST.Error;

namespace OpenFAST.Session
{
    public sealed class FastServer : ConnectionListener
    {
        private readonly IEndpoint _endpoint;
        private readonly string _serverName;
        private readonly ISessionProtocol _sessionProtocol;
        private IErrorHandler _errorHandler = ErrorHandlerFields.Default;
        private bool _listening;
        private Thread _serverThread;
        private ISessionHandler _sessionHandler = SessionHandlerFields.Null;

        public FastServer(string serverName, ISessionProtocol sessionProtocol, IEndpoint endpoint)
        {
            if (sessionProtocol == null) throw new ArgumentNullException("sessionProtocol");
            if (endpoint == null) throw new ArgumentNullException("endpoint");

            _endpoint = endpoint;
            _sessionProtocol = sessionProtocol;
            _serverName = serverName;
            endpoint.ConnectionListener = this;
        }

        public IErrorHandler ErrorHandler
        {
            // ************* OPTIONAL DEPENDENCY SETTERS **************
            set
            {
                if (value == null) throw new ArgumentNullException("value");
                _errorHandler = value;
            }
        }

        public ISessionHandler SessionHandler
        {
            set { _sessionHandler = value; }
        }

        public void Listen()
        {
            _listening = true;
            if (_serverThread == null)
            {
                _serverThread = new Thread(
                    () =>
                        {
                            while (_listening)
                            {
                                try
                                {
                                    _endpoint.Accept();
                                }
                                catch (FastConnectionException e)
                                {
                                    _errorHandler.OnError(e, DynError.Undefined, null);
                                }
                                try
                                {
                                    Thread.Sleep(new TimeSpan((Int64) 10000*20));
                                }
                                catch (ThreadInterruptedException)
                                {
                                }
                            }
                        }) {Name = "FastServer"};
            }
            _serverThread.Start();
        }

        public void Close()
        {
            _listening = false;
            _endpoint.Close();
        }

        public override void OnConnect(IConnection connection)
        {
            Session session = _sessionProtocol.OnNewConnection(_serverName, connection);
            _sessionHandler.NewSession(session);
        }
    }
}