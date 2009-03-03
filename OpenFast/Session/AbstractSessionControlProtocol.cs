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
using Message = OpenFAST.Message;
using Field = OpenFAST.Template.Field;
using MessageTemplate = OpenFAST.Template.MessageTemplate;
using OpenFAST.Template;

namespace OpenFAST.Session
{
	public abstract class AbstractSessionControlProtocol : SessionProtocol
	{
		[Serializable]
		public class RESETMessageBase:Message
		{
            internal RESETMessageBase(OpenFAST.Template.MessageTemplate Param1)
                : base(Param1)
			{
			}
			private const long serialVersionUID = 1L;
		}
		virtual public Message ResetMessage
		{
			get
			{
				return RESET;
			}
			
		}
		public abstract OpenFAST.Message CloseMessage{get;}
		internal const int FAST_RESET_TEMPLATE_ID = 120;

        internal static readonly MessageTemplate FAST_RESET_TEMPLATE = new MessageTemplate("Reset", new Field[]{});
		
		internal static readonly Message RESET;
		public abstract OpenFAST.Message CreateTemplateDefinitionMessage(OpenFAST.Template.MessageTemplate param1);
		public abstract OpenFAST.Session.Session OnNewConnection(string param1, OpenFAST.Session.Connection param2);
        public abstract OpenFAST.Session.Session Connect(string param1, OpenFAST.Session.Connection param2, TemplateRegistry inboundRegistry, TemplateRegistry outboundRegistry, MessageListener messageListener, SessionListener sessionListener);
		public abstract void  HandleMessage(OpenFAST.Session.Session param1, OpenFAST.Message param2);
		public abstract void  OnError(OpenFAST.Session.Session param1, OpenFAST.Error.ErrorCode param2, string param3);
		public abstract bool SupportsTemplateExchange();
		public abstract void  ConfigureSession(OpenFAST.Session.Session param1);
		public abstract OpenFAST.Message CreateTemplateDeclarationMessage(OpenFAST.Template.MessageTemplate param1, int param2);
		public abstract bool IsProtocolMessage(OpenFAST.Message param1);
		static AbstractSessionControlProtocol()
		{
			RESET = new RESETMessageBase(FAST_RESET_TEMPLATE);
		}
	}
}