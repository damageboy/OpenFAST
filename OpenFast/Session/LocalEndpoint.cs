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
using System.Collections.Generic;

namespace OpenFAST.Session
{
    public class LocalEndpoint : IEndpoint
    {
        private readonly List<LocalConnection> _connections;
        private readonly LocalEndpoint _server;
        private ConnectionListener _listener;

        public LocalEndpoint()
        {
            _connections = new List<LocalConnection>(3);
        }

        public LocalEndpoint(LocalEndpoint server)
        {
            _server = server;
        }

        #region IEndpoint Members

        public virtual ConnectionListener ConnectionListener
        {
            set { _listener = value; }
        }

        public virtual void Accept()
        {
            if (_connections.Count != 0)
            {
                lock (this)
                {
                    LocalConnection c = _connections[0];
                    _connections.RemoveAt(0);
                    _listener.OnConnect(c);
                }
            }
        }

        public virtual IConnection Connect()
        {
            var localConnection = new LocalConnection();
            var remoteConnection = new LocalConnection(localConnection);
            lock (_server)
            {
                _server._connections.Add(remoteConnection);
            }
            return localConnection;
        }

        public virtual void Close()
        {
        }

        #endregion
    }
}