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
using ErrorCode = OpenFAST.Error.ErrorCode;
using MessageTemplate = OpenFAST.Template.MessageTemplate;
using OpenFAST.Template;

namespace OpenFAST.Session
{
	public interface SessionProtocol
	{
		Message ResetMessage
		{
			get;
			
		}
		Message CloseMessage
		{
			get;
			
		}
		void  ConfigureSession(Session session);
        Session Connect(string senderName, Connection connection, TemplateRegistry inboundRegistry, TemplateRegistry outboundRegistry, MessageListener messageListener, SessionListener sessionListener);
		Session OnNewConnection(string serverName, Connection connection);
		void  OnError(Session session, ErrorCode code, string message);
		bool IsProtocolMessage(Message message);
		void  HandleMessage(Session session, Message message);

		bool SupportsTemplateExchange();
		Message CreateTemplateDefinitionMessage(MessageTemplate messageTemplate);
		Message CreateTemplateDeclarationMessage(MessageTemplate messageTemplate, int templateId);
	}
}