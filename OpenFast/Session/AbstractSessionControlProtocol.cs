using System;
using Message = OpenFAST.Message;
using Field = OpenFAST.Template.Field;
using MessageTemplate = OpenFAST.Template.MessageTemplate;

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
		public abstract OpenFAST.Session.Session Connect(string param1, OpenFAST.Session.Connection param2);
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