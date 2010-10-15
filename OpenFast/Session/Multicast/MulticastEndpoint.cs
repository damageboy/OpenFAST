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
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace OpenFAST.Session.Multicast
{
    public sealed class MulticastEndpoint : IEndpoint
    {
        private readonly string _group;
        private readonly int _port;

        public MulticastEndpoint(int port, string group)
        {
            _port = port;
            _group = group;
        }

        #region Endpoint Members

        public ConnectionListener ConnectionListener
        {
            set { throw new NotSupportedException(); }
        }

        public void Accept()
        {
            throw new NotSupportedException();
        }

        public void Close()
        {
        }

        public IConnection Connect()
        {
            try
            {
                var socket = new UdpClient(_port);
                IPAddress groupAddress = Dns.GetHostEntry(_group).AddressList[0];
                socket.JoinMulticastGroup(groupAddress);
                return new MulticastConnection(socket, groupAddress);
            }
            catch (IOException e)
            {
                throw new FastConnectionException(e);
            }
        }

        #endregion
    }
}