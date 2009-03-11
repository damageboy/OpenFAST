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
		private readonly Endpoint endpoint;
		private readonly SessionProtocol sessionProtocol;
		
		public FastClient(string clientName, SessionProtocol sessionProtocol, Endpoint endpoint)
		{
			this.clientName = clientName;
			this.sessionProtocol = sessionProtocol;
			this.endpoint = endpoint;
		}
		public Session Connect()
		{
			var connection = endpoint.Connect();
            var session = sessionProtocol.Connect(clientName, connection, inboundRegistry, outboundRegistry, messageListener, sessionListener);
			return session;
		}
        private readonly SessionListener sessionListener = SessionListener_Fields.NULL;
        private TemplateRegistry inboundRegistry = TemplateRegistry_Fields.NULL;
        private TemplateRegistry outboundRegistry = TemplateRegistry_Fields.NULL;
        private MessageListener messageListener = MessageListener_Fields.NULL;
        public MessageListener MessageListener
        {
            set
            {
                messageListener = value;
            }
        }
        public TemplateRegistry InboundTemplateRegistry
        {
            set
            {
                inboundRegistry = value;
            }
            get
            {
                return inboundRegistry;
            }
        }
        public TemplateRegistry OutboundTemplateRegistry
        {
            set
            {
                outboundRegistry = value;
            }
            get
            {
                return outboundRegistry;
            }
        }
	}
}