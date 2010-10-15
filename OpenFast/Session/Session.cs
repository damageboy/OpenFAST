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
using System.Net.Sockets;
using System.Threading;
using OpenFAST.Error;
using OpenFAST.Template;

namespace OpenFAST.Session
{
    public class Session : ErrorHandler
    {
        private readonly Connection connection;
        private readonly SessionProtocol protocol;
        private ErrorHandler errorHandler = ErrorHandler_Fields.DEFAULT;
        private MessageInputStream in_stream;
        private bool listening;
        private SupportClass.ThreadClass listeningThread;
        private MessageListener messageListener;
        private MessageOutputStream out_stream;
        private SessionListener sessionListener = SessionListener_Fields.NULL;

        public Session(Connection connection, SessionProtocol protocol, TemplateRegistry inboundRegistry,
                       TemplateRegistry outboundRegistry)
        {
            var inContext = new Context();
            inContext.TemplateRegistry.RegisterAll(inboundRegistry);
            var outContext = new Context();
            outContext.TemplateRegistry.RegisterAll(outboundRegistry);
            inContext.ErrorHandler = this;

            this.connection = connection;
            this.protocol = protocol;
            try
            {
                in_stream = new MessageInputStream(connection.InputStream.BaseStream, inContext);
                out_stream = new MessageOutputStream(connection.OutputStream.BaseStream, outContext);
            }
            catch (IOException e)
            {
                errorHandler.Error(null, "Error occurred in connection.", e);
                throw new IllegalStateException(e);
            }

            protocol.ConfigureSession(this);
        }

        public virtual Client Client { get; set; }

        public virtual ErrorHandler ErrorHandler
        {
            get { return errorHandler; }

            set
            {
                if (value == null)
                {
                    errorHandler = ErrorHandler_Fields.NULL;
                }

                errorHandler = value;
            }
        }

        public virtual Connection Connection
        {
            get { return connection; }
        }

        public virtual MessageListener MessageHandler
        {
            set
            {
                messageListener = value;
                Listening = true;
            }
        }

        public virtual bool Listening
        {
            set
            {
                listening = value;
                if (value)
                    ListenForMessages();
            }
        }

        public virtual SessionListener SessionListener
        {
            set { sessionListener = value; }
        }

        public MessageInputStream MessageInputStream
        {
            get { return in_stream; }
            set { in_stream = value; }
        }

        public MessageOutputStream MessageOutputStream
        {
            get { return out_stream; }
            set { out_stream = value; }
        }

        #region ErrorHandler Members

        public virtual void Error(ErrorCode code, string message)
        {
            if (code.Equals(FastConstants.D9_TEMPLATE_NOT_REGISTERED))
            {
                code = SessionConstants.TEMPLATE_NOT_SUPPORTED;
                message = "Template Not Supported";
            }
            protocol.OnError(this, code, message);
            errorHandler.Error(code, message);
        }

        public virtual void Error(ErrorCode code, string message, Exception t)
        {
            protocol.OnError(this, code, message);
            errorHandler.Error(code, message, t);
        }

        #endregion

        public virtual void Close()
        {
            listening = false;
            out_stream.WriteMessage(protocol.CloseMessage);
            in_stream.Close();
            out_stream.Close();
        }

        // RESPONDER
        public virtual void Close(ErrorCode alertCode)
        {
            listening = false;
            in_stream.Close();
            out_stream.Close();
            sessionListener.OnClose();
        }

        public virtual void Reset()
        {
            out_stream.Reset();
            in_stream.Reset();
            out_stream.WriteMessage(protocol.ResetMessage);
        }

        private void ListenForMessages()
        {
            if (listeningThread == null)
            {
                IThreadRunnable messageReader = new SessionThread(this);
                listeningThread = new SupportClass.ThreadClass(new ThreadStart(messageReader.Run),
                                                               "FAST Session Message Reader");
            }
            if (listeningThread.IsAlive)
                return;
            listeningThread.Start();
        }

        public virtual void SendTemplates(TemplateRegistry registry)
        {
            if (!protocol.SupportsTemplateExchange())
            {
                throw new NotSupportedException("The procotol " + protocol +
                                                " does not support template exchange.");
            }
            MessageTemplate[] templates = registry.Templates;
            for (int i = 0; i < templates.Length; i++)
            {
                MessageTemplate template = templates[i];
                out_stream.WriteMessage(protocol.CreateTemplateDefinitionMessage(template));
                out_stream.WriteMessage(protocol.CreateTemplateDeclarationMessage(template, registry.GetId(template)));
                if (!out_stream.GetTemplateRegistry().IsRegistered(template))
                    out_stream.RegisterTemplate(registry.GetId(template), template);
            }
        }

        public virtual void AddDynamicTemplateDefinition(MessageTemplate template)
        {
            in_stream.GetTemplateRegistry().Define(template);
            out_stream.GetTemplateRegistry().Define(template);
        }

        public virtual void RegisterDynamicTemplate(QName templateName, int id)
        {
            if (!in_stream.GetTemplateRegistry().IsDefined(templateName))
            {
                throw new SystemException("Template " + templateName + " has not been defined.");
            }
            in_stream.GetTemplateRegistry().Register(id, templateName);
            if (!out_stream.GetTemplateRegistry().IsDefined(templateName))
            {
                throw new SystemException("Template " + templateName + " has not been defined.");
            }
            out_stream.GetTemplateRegistry().Register(id, templateName);
        }

        #region Nested type: SessionThread

        private class SessionThread : IThreadRunnable
        {
            private Session enclosingInstance;

            public SessionThread(Session enclosingInstance)
            {
                InitBlock(enclosingInstance);
            }

            public Session Enclosing_Instance
            {
                get { return enclosingInstance; }
            }

            #region IThreadRunnable Members

            public virtual void Run()
            {
                while (Enclosing_Instance.listening)
                {
                    try
                    {
                        Message message = Enclosing_Instance.MessageInputStream.ReadMessage();

                        if (message == null)
                        {
                            Enclosing_Instance.listening = false;
                            break;
                        }
                        if (Enclosing_Instance.protocol.IsProtocolMessage(message))
                        {
                            Enclosing_Instance.protocol.HandleMessage(Enclosing_Instance, message);
                        }
                        else if (Enclosing_Instance.messageListener != null)
                        {
                            Enclosing_Instance.messageListener.OnMessage(enclosingInstance, message);
                        }
                        else
                        {
                            throw new SystemException("Received non-protocol message without a message listener.");
                        }
                    }
                    catch (Exception e)
                    {
                        Exception cause = e.InnerException;

                        if (cause != null && cause.GetType().Equals(typeof (SocketException)) &&
                            cause.Message.Equals("Socket closed"))
                        {
                            Enclosing_Instance.listening = false;
                        }
                        else if (e is FastException)
                        {
                            var fastException = ((FastException) e);
                            Enclosing_Instance.errorHandler.Error(fastException.Code, fastException.Message, e);
                        }
                        else
                        {
                            Enclosing_Instance.errorHandler.Error(FastConstants.GENERAL_ERROR, e.Message,
                                                                  e);
                        }
                    }
                }
            }

            #endregion

            private void InitBlock(Session internalInstance)
            {
                enclosingInstance = internalInstance;
            }
        }

        #endregion
    }
}