using System;
using Message = OpenFAST.Message;
using ErrorCode = OpenFAST.Error.ErrorCode;
using MessageTemplate = OpenFAST.Template.MessageTemplate;

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
		Session Connect(string senderName, Connection connection);
		Session OnNewConnection(string serverName, Connection connection);
		void  OnError(Session session, ErrorCode code, string message);
		bool IsProtocolMessage(Message message);
		void  HandleMessage(Session session, Message message);

		bool SupportsTemplateExchange();
		Message CreateTemplateDefinitionMessage(MessageTemplate messageTemplate);
		Message CreateTemplateDeclarationMessage(MessageTemplate messageTemplate, int templateId);
	}
}