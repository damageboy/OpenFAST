using System;
using Context = OpenFAST.Context;
using Message = OpenFAST.Message;
using MessageHandler = OpenFAST.MessageHandler;
using ScalarValue = OpenFAST.ScalarValue;
using Coder = OpenFAST.Codec.Coder;
using ErrorCode = OpenFAST.Error.ErrorCode;
using Field = OpenFAST.Template.Field;
using MessageTemplate = OpenFAST.Template.MessageTemplate;
using Scalar = OpenFAST.Template.Scalar;
using TemplateRegistry = OpenFAST.Template.TemplateRegistry;
using Operator = OpenFAST.Template.operator_Renamed.Operator;
using Type = OpenFAST.Template.Type.FASTType;

namespace OpenFAST.Session
{
	public class SessionControlProtocol_1_0:AbstractSessionControlProtocol
	{
		public class RESETMessageHandler : MessageHandler
		{
			public virtual void  HandleMessage(Message readMessage, Context context, Coder coder)
			{
				coder.Reset();
			}
		}
		override public Message CloseMessage
		{
			get
			{
				return CreateFastAlertMessage(OpenFAST.Session.SessionConstants.CLOSE);
			}
			
		}
		internal const int FAST_HELLO_TEMPLATE_ID = 16000;
		internal const int FAST_ALERT_TEMPLATE_ID = 16001;
		
		public override Session OnNewConnection(string serverName, Connection connection)
		{
			Session session = new Session(connection, this);
			Message message = session.MessageInputStream.readMessage();
			session.MessageOutputStream.WriteMessage(CreateHelloMessage(serverName));
			string clientName = message.GetString(1);
			session.Client = new BasicClient(clientName, "unknown");
			return session;
		}
		public override Session Connect(string senderName, Connection connection)
		{
			Session session = new Session(connection, this);
			session.MessageOutputStream.WriteMessage(CreateHelloMessage(senderName));
			Message message = session.MessageInputStream.readMessage();
			string serverName = message.GetString(1);
			session.Client = new BasicClient(serverName, "unknown");
			return session;
		}
		public override void  OnError(Session session, ErrorCode code, string message)
		{
			session.MessageOutputStream.WriteMessage(CreateFastAlertMessage(code));
		}
		public virtual void  RegisterSessionTemplates(TemplateRegistry registry)
		{
			registry.Register(FAST_HELLO_TEMPLATE_ID, FAST_HELLO_TEMPLATE);
			registry.Register(FAST_ALERT_TEMPLATE_ID, FAST_ALERT_TEMPLATE);
			registry.Register(FAST_RESET_TEMPLATE_ID, FAST_RESET_TEMPLATE);
		}
		public override void  ConfigureSession(Session session)
		{
			RegisterSessionTemplates(session.MessageInputStream.GetTemplateRegistry());
			RegisterSessionTemplates(session.MessageOutputStream.GetTemplateRegistry());
			session.MessageInputStream.AddMessageHandler(FAST_RESET_TEMPLATE, RESET_HANDLER);
			session.MessageOutputStream.AddMessageHandler(FAST_RESET_TEMPLATE, RESET_HANDLER);
		}
		public static Message CreateFastAlertMessage(ErrorCode code)
		{
			Message alert = new Message(FAST_ALERT_TEMPLATE);
			alert.SetInteger(1, code.Severity.Code);
			alert.SetInteger(2, code.Code);
			alert.SetString(4, code.Description);
			return alert;
		}
		public static Message CreateHelloMessage(string name)
		{
			Message message = new Message(FAST_HELLO_TEMPLATE);
			message.SetString(1, name);
			return message;
		}
		
		public static readonly MessageTemplate FAST_ALERT_TEMPLATE;
		public static readonly MessageTemplate FAST_HELLO_TEMPLATE;
		private static readonly MessageHandler RESET_HANDLER;
		
		public override void  HandleMessage(Session session, Message message)
		{
			if (message.Template.Equals(FAST_ALERT_TEMPLATE))
			{
				ErrorCode alertCode = ErrorCode.GetAlertCode(message);
				if (alertCode.Equals(OpenFAST.Session.SessionConstants.CLOSE))
				{
					session.Close(alertCode);
				}
				else
				{
					session.ErrorHandler.Error(alertCode, message.GetString(4));
				}
			}
		}
		public override bool IsProtocolMessage(Message message)
		{
			if (message == null)
				return false;
			return (message.Template.Equals(FAST_ALERT_TEMPLATE)) || (message.Template.Equals(FAST_HELLO_TEMPLATE)) || (message.Template.Equals(FAST_RESET_TEMPLATE));
		}
		public override bool SupportsTemplateExchange()
		{
			return false;
		}
		public override Message CreateTemplateDeclarationMessage(MessageTemplate messageTemplate, int templateId)
		{
			return null;
		}
		public override Message CreateTemplateDefinitionMessage(MessageTemplate messageTemplate)
		{
			return null;
		}
		static SessionControlProtocol_1_0()
		{
			FAST_ALERT_TEMPLATE = new MessageTemplate("", new Field[]{new Scalar("Severity", Type.U32, Operator.NONE, ScalarValue.UNDEFINED, false), new Scalar("Code", Type.U32, Operator.NONE, ScalarValue.UNDEFINED, false), new Scalar("Value", Type.U32, Operator.NONE, ScalarValue.UNDEFINED, true), new Scalar("Description", Type.ASCII, Operator.NONE, ScalarValue.UNDEFINED, false)});
			FAST_HELLO_TEMPLATE = new MessageTemplate("", new Field[]{new Scalar("SenderName", Type.ASCII, Operator.NONE, ScalarValue.UNDEFINED, false)});
			RESET_HANDLER = new RESETMessageHandler();
		}
	}
}