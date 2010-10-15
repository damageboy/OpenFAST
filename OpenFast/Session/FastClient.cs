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
using OpenFAST.Template;

namespace OpenFAST.Session
{
    public sealed class FastClient
    {
        private readonly string clientName;
        private readonly IEndpoint endpoint;

        private readonly ISessionListener sessionListener = SessionListenerFields.Null;
        private readonly ISessionProtocol sessionProtocol;
        private ITemplateRegistry inboundRegistry = TemplateRegistryFields.NULL;
        private IMessageListener messageListener = MessageListener_Fields.NULL;
        private ITemplateRegistry outboundRegistry = TemplateRegistryFields.NULL;

        public FastClient(string clientName, ISessionProtocol sessionProtocol, IEndpoint endpoint)
        {
            this.clientName = clientName;
            this.sessionProtocol = sessionProtocol;
            this.endpoint = endpoint;
        }

        public IMessageListener MessageListener
        {
            set { messageListener = value; }
        }

        public ITemplateRegistry InboundTemplateRegistry
        {
            set { inboundRegistry = value; }
            get { return inboundRegistry; }
        }

        public ITemplateRegistry OutboundTemplateRegistry
        {
            set { outboundRegistry = value; }
            get { return outboundRegistry; }
        }

        public Session Connect()
        {
            IConnection connection = endpoint.Connect();
            Session session = sessionProtocol.Connect(clientName, connection, inboundRegistry, outboundRegistry,
                                                      messageListener, sessionListener);
            return session;
        }
    }
}