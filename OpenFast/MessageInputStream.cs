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
using System.Collections.Generic;
using System.IO;
using OpenFAST.Codec;
using OpenFAST.Template;

namespace OpenFAST
{
    public sealed class MessageInputStream : IMessageStream
    {
        private readonly Context _context;
        private readonly FastDecoder _decoder;
        private readonly List<IMessageHandler> _handlers = new List<IMessageHandler>();
        private readonly Stream _inStream;

        private readonly Dictionary<MessageTemplate, IMessageHandler> _templateHandlers =
            new Dictionary<MessageTemplate, IMessageHandler>();

        private IMessageBlockReader _blockReader = MessageBlockReaderFields.Null;

        public MessageInputStream(Stream inputStream) : this(inputStream, new Context())
        {
        }

        public MessageInputStream(Stream inputStream, Context context)
        {
            _inStream = inputStream;
            _context = context;
            _decoder = new FastDecoder(context, _inStream);
        }

        public Stream UnderlyingStream
        {
            get { return _inStream; }
        }

        public Context Context
        {
            get { return _context; }
        }

        public IMessageBlockReader BlockReader
        {
            set { _blockReader = value; }
        }

        #region IMessageStream Members

        public void Close()
        {
            try
            {
                _inStream.Close();
            }
            catch (IOException e)
            {
                throw new RuntimeException(e);
            }
        }

        public void AddMessageHandler(MessageTemplate template, IMessageHandler handler)
        {
            _templateHandlers[template] = handler;
        }

        public void AddMessageHandler(IMessageHandler handler)
        {
            _handlers.Add(handler);
        }

        public ITemplateRegistry TemplateRegistry
        {
            get { return _context.TemplateRegistry; }
            set { _context.TemplateRegistry = value; }
        }

        #endregion

        public Message ReadMessage()
        {
            if (_context.TraceEnabled)
                _context.StartTrace();

            bool keepReading = _blockReader.ReadBlock(_inStream);

            if (!keepReading)
                return null;

            Message message = _decoder.ReadMessage();

            if (message == null)
            {
                return null;
            }

            _blockReader.MessageRead(_inStream, message);

            foreach (IMessageHandler t in _handlers)
            {
                t.HandleMessage(message, _context, _decoder);
            }

            IMessageHandler handler;
            if (_templateHandlers.TryGetValue(message.Template, out handler))
            {
                handler.HandleMessage(message, _context, _decoder);

                return ReadMessage();
            }

            return message;
        }

        public void RegisterTemplate(int templateId, MessageTemplate template)
        {
            _context.RegisterTemplate(templateId, template);
        }

        //public void  AddTemplateRegisteredListener(TemplateRegisteredListener templateRegisteredListener)
        //{
        //}

        public void Reset()
        {
            _decoder.Reset();
        }
    }
}