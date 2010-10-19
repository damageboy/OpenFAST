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
using OpenFAST.Template;

namespace OpenFAST.Session
{
    public sealed class FastClient
    {
        private readonly string _clientName;
        private readonly IEndpoint _endpoint;

        private readonly ISessionListener _sessionListener = SessionListenerFields.Null;
        private readonly ISessionProtocol _sessionProtocol;
        private ITemplateRegistry _inboundRegistry = TemplateRegistryFields.Null;
        private IMessageListener _messageListener = MessageListenerFields.Null;
        private ITemplateRegistry _outboundRegistry = TemplateRegistryFields.Null;

        public FastClient(string clientName, ISessionProtocol sessionProtocol, IEndpoint endpoint)
        {
            _clientName = clientName;
            _sessionProtocol = sessionProtocol;
            _endpoint = endpoint;
        }

        public IMessageListener MessageListener
        {
            set { _messageListener = value; }
        }

        public ITemplateRegistry InboundTemplateRegistry
        {
            set { _inboundRegistry = value; }
            get { return _inboundRegistry; }
        }

        public ITemplateRegistry OutboundTemplateRegistry
        {
            set { _outboundRegistry = value; }
            get { return _outboundRegistry; }
        }

        public Session Connect()
        {
            IConnection connection = _endpoint.Connect();
            Session session = _sessionProtocol.Connect(_clientName, connection, _inboundRegistry, _outboundRegistry,
                                                       _messageListener, _sessionListener);
            return session;
        }
    }
}