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
using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using OpenFAST.Error;
using OpenFAST.Template;

namespace OpenFAST.Session
{
    public class Session : IErrorHandler
    {
        private readonly IConnection _connection;
        private readonly ISessionProtocol _protocol;
        private IErrorHandler _errorHandler = ErrorHandlerFields.Default;
        private MessageInputStream _inStream;
        private bool _listening;
        private SupportClass.ThreadClass _listeningThread;
        private IMessageListener _messageListener;
        private MessageOutputStream _outStream;
        private ISessionListener _sessionListener = SessionListenerFields.Null;

        public Session(IConnection connection, ISessionProtocol protocol, ITemplateRegistry inboundRegistry,
                       ITemplateRegistry outboundRegistry)
        {
            var inContext = new Context();
            inContext.TemplateRegistry.RegisterAll(inboundRegistry);
            var outContext = new Context();
            outContext.TemplateRegistry.RegisterAll(outboundRegistry);
            inContext.ErrorHandler = this;

            _connection = connection;
            _protocol = protocol;
            try
            {
                _inStream = new MessageInputStream(connection.InputStream.BaseStream, inContext);
                _outStream = new MessageOutputStream(connection.OutputStream.BaseStream, outContext);
            }
            catch (IOException e)
            {
                _errorHandler.Error(null, "Error occurred in connection.", e);
                throw new IllegalStateException(e);
            }

            protocol.ConfigureSession(this);
        }

        public virtual IClient Client { get; set; }

        public virtual IErrorHandler ErrorHandler
        {
            get { return _errorHandler; }
            set
            {
                if (value == null)
                    _errorHandler = ErrorHandlerFields.Null;
                _errorHandler = value;
            }
        }

        public virtual IConnection Connection
        {
            get { return _connection; }
        }

        public virtual IMessageListener MessageHandler
        {
            set
            {
                _messageListener = value;
                IsListening = true;
            }
        }

        public virtual bool IsListening
        {
            get { return _listening; }
            set
            {
                _listening = value;
                if (value)
                    ListenForMessages();
            }
        }

        public virtual ISessionListener SessionListener
        {
            set { _sessionListener = value; }
        }

        public MessageInputStream MessageInputStream
        {
            get { return _inStream; }
            set { _inStream = value; }
        }

        public MessageOutputStream MessageOutputStream
        {
            get { return _outStream; }
            set { _outStream = value; }
        }

        #region IErrorHandler Members

        public virtual void Error(ErrorCode code, string message)
        {
            if (code.Equals(FastConstants.D9_TEMPLATE_NOT_REGISTERED))
            {
                code = SessionConstants.TEMPLATE_NOT_SUPPORTED;
                message = "Template Not Supported";
            }
            _protocol.OnError(this, code, message);
            _errorHandler.Error(code, message);
        }

        public virtual void Error(ErrorCode code, string message, Exception t)
        {
            _protocol.OnError(this, code, message);
            _errorHandler.Error(code, message, t);
        }

        #endregion

        public virtual void Close()
        {
            _listening = false;
            _outStream.WriteMessage(_protocol.CloseMessage);
            _inStream.Close();
            _outStream.Close();
            _connection.Close();
        }

        // RESPONDER
        public virtual void Close(ErrorCode alertCode)
        {
            _listening = false;
            _inStream.Close();
            _outStream.Close();
            _sessionListener.OnClose();
        }

        public virtual void Reset()
        {
            _outStream.Reset();
            _inStream.Reset();
            _outStream.WriteMessage(_protocol.ResetMessage);
        }

        private void ListenForMessages()
        {
            if (_listeningThread == null)
            {
                IThreadRunnable messageReader = new SessionThread(this);
                _listeningThread = new SupportClass.ThreadClass(new ThreadStart(messageReader.Run),
                                                                "FAST Session Message Reader");
            }
            if (_listeningThread.IsAlive)
                return;
            _listeningThread.Start();
        }

        public virtual void SendTemplates(ITemplateRegistry registry)
        {
            if (!_protocol.SupportsTemplateExchange())
            {
                throw new NotSupportedException("The procotol " + _protocol +
                                                " does not support template exchange.");
            }
            foreach (MessageTemplate template in registry.Templates)
            {
                _outStream.WriteMessage(_protocol.CreateTemplateDefinitionMessage(template));

                int templateId = registry.GetId(template);

                _outStream.WriteMessage(_protocol.CreateTemplateDeclarationMessage(template, templateId));

                // BUG? double check if IsRegister() done on the same object as RegisterTemplate
                if (!_outStream.GetTemplateRegistry().IsRegistered(template))
                    _outStream.RegisterTemplate(templateId, template);
            }
        }

        public virtual void AddDynamicTemplateDefinition(MessageTemplate template)
        {
            _inStream.GetTemplateRegistry().Define(template);
            _outStream.GetTemplateRegistry().Define(template);
        }

        public virtual void RegisterDynamicTemplate(QName templateName, int id)
        {
            if (!_inStream.GetTemplateRegistry().TryRegister(id, templateName))
                throw new ArgumentOutOfRangeException("templateName", templateName,
                                                      "Template is not defined in the input stream.");

            if (!_outStream.GetTemplateRegistry().TryRegister(id, templateName))
                throw new ArgumentOutOfRangeException("templateName", templateName,
                                                      "Template is not defined in the output stream.");
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
                while (Enclosing_Instance._listening)
                {
                    try
                    {
                        Message message = Enclosing_Instance.MessageInputStream.ReadMessage();

                        if (message == null)
                        {
                            Enclosing_Instance._listening = false;
                            break;
                        }
                        if (Enclosing_Instance._protocol.IsProtocolMessage(message))
                        {
                            Enclosing_Instance._protocol.HandleMessage(Enclosing_Instance, message);
                        }
                        else if (Enclosing_Instance._messageListener != null)
                        {
                            Enclosing_Instance._messageListener.OnMessage(enclosingInstance, message);
                        }
                        else
                        {
                            throw new InvalidOperationException(
                                "Received non-protocol message without a message listener.");
                        }
                    }
                    catch (Exception e)
                    {
                        Exception cause = e.InnerException;

                        if (cause != null && cause.GetType().Equals(typeof (SocketException)) &&
                            cause.Message.Equals("Socket closed"))
                        {
                            Enclosing_Instance._listening = false;
                        }
                        else if (e is FastException)
                        {
                            var fastException = ((FastException) e);
                            Enclosing_Instance._errorHandler.Error(fastException.Code, fastException.Message, e);
                        }
                        else
                        {
                            Enclosing_Instance._errorHandler.Error(FastConstants.GENERAL_ERROR, e.Message,
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