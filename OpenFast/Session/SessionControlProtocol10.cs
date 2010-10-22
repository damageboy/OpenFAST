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
using OpenFAST.Codec;
using OpenFAST.Error;
using OpenFAST.Template;
using OpenFAST.Template.Operator;
using Type = OpenFAST.Template.Type.FASTType;

namespace OpenFAST.Session
{
    public class SessionControlProtocol10 : AbstractSessionControlProtocol
    {
        private const int ResetTemplateId = 120;
        private const int HelloTemplateId = 16000;
        private const int AlertTemplateId = 16001;

        private static readonly MessageTemplate AlertTemplate;
        private static readonly MessageTemplate HelloTemplate;
        private static readonly IMessageHandler ResetHandler;

        static SessionControlProtocol10()
        {
            AlertTemplate = new MessageTemplate(
                "",
                new Field[]
                    {
                        new Scalar("Severity", Type.U32, Operator.None, ScalarValue.Undefined, false),
                        new Scalar("Code", Type.U32, Operator.None, ScalarValue.Undefined, false),
                        new Scalar("Value", Type.U32, Operator.None, ScalarValue.Undefined, true),
                        new Scalar("Description", Type.Ascii, Operator.None, ScalarValue.Undefined, false)
                    });
            HelloTemplate = new MessageTemplate(
                "",
                new Field[]
                    {
                        new Scalar("SenderName", Type.Ascii, Operator.None, ScalarValue.Undefined, false)
                    });
            ResetHandler = new ResetMessageHandler();
        }

        public override Message CloseMessage
        {
            get { return CreateFastAlertMessage(SessionConstants.Close); }
        }

        public override Session OnNewConnection(string serverName, IConnection connection)
        {
            var session = new Session(connection, this, TemplateRegistryFields.Null, TemplateRegistryFields.Null);
            Message message = session.MessageInputStream.ReadMessage();
            session.MessageOutputStream.WriteMessage(CreateHelloMessage(serverName));
            string clientName = message.GetString(1);
            session.Client = new BasicClient(clientName, "unknown");
            return session;
        }

        public override Session Connect(string senderName, IConnection connection, ITemplateRegistry inboundRegistry,
                                        ITemplateRegistry outboundRegistry, IMessageListener messageListener,
                                        ISessionListener sessionListener)
        {
            var session = new Session(connection, this, TemplateRegistryFields.Null, TemplateRegistryFields.Null);
            session.MessageOutputStream.WriteMessage(CreateHelloMessage(senderName));
            Message message = session.MessageInputStream.ReadMessage();
            string serverName = message.GetString(1);
            session.Client = new BasicClient(serverName, "unknown");
            return session;
        }

        public override void OnError(Session session, ErrorCode code, string message)
        {
            session.MessageOutputStream.WriteMessage(CreateFastAlertMessage(code));
        }

        public virtual void RegisterSessionTemplates(ITemplateRegistry registry)
        {
            registry.Register(HelloTemplateId, HelloTemplate);
            registry.Register(AlertTemplateId, AlertTemplate);
            registry.Register(ResetTemplateId, FastResetTemplate);
        }

        public override void ConfigureSession(Session session)
        {
            RegisterSessionTemplates(session.MessageInputStream.GetTemplateRegistry());
            RegisterSessionTemplates(session.MessageOutputStream.GetTemplateRegistry());
            session.MessageInputStream.AddMessageHandler(FastResetTemplate, ResetHandler);
            session.MessageOutputStream.AddMessageHandler(FastResetTemplate, ResetHandler);
        }

        public static Message CreateFastAlertMessage(ErrorCode code)
        {
            var alert = new Message(AlertTemplate);
            alert.SetInteger(1, (int) code.Severity);
            alert.SetInteger(2, code.Code);
            alert.SetString(4, code.Description);
            return alert;
        }

        public static Message CreateHelloMessage(string name)
        {
            var message = new Message(HelloTemplate);
            message.SetString(1, name);
            return message;
        }

        public override void HandleMessage(Session session, Message message)
        {
            if (message.Template.Equals(AlertTemplate))
            {
                ErrorCode alertCode = ErrorCode.GetAlertCode(message.GetInt(2));
                if (alertCode.Equals(SessionConstants.Close))
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
            return (message.Template.Equals(AlertTemplate)) || (message.Template.Equals(HelloTemplate)) ||
                   (message.Template.Equals(FastResetTemplate));
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

        #region Nested type: RESETMessageHandler

        public class ResetMessageHandler : IMessageHandler
        {
            #region IMessageHandler Members

            public virtual void HandleMessage(Message readMessage, Context context, ICoder coder)
            {
                coder.Reset();
            }

            #endregion
        }

        #endregion
    }
}