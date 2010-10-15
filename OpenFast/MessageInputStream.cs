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
    public sealed class MessageInputStream : MessageStream
    {
        private readonly Context context;
        private readonly FastDecoder decoder;

        private readonly List<MessageHandler> handlers =
            new List<MessageHandler>();

        private readonly Stream in_Renamed;

        private readonly Dictionary<MessageTemplate, MessageHandler> templateHandlers =
            new Dictionary<MessageTemplate, MessageHandler>();

        private MessageBlockReader blockReader = MessageBlockReader_Fields.NULL;

        public MessageInputStream(Stream inputStream) : this(inputStream, new Context())
        {
        }

        public MessageInputStream(Stream inputStream, Context context)
        {
            in_Renamed = inputStream;
            this.context = context;
            decoder = new FastDecoder(context, in_Renamed);
        }

        public Stream UnderlyingStream
        {
            get { return in_Renamed; }
        }

        public Context Context
        {
            get { return context; }
        }

        public MessageBlockReader BlockReader
        {
            set { blockReader = value; }
        }

        #region MessageStream Members

        public void Close()
        {
            try
            {
                in_Renamed.Close();
            }
            catch (IOException e)
            {
                throw new RuntimeException(e);
            }
        }

        public void AddMessageHandler(MessageTemplate template, MessageHandler handler)
        {
            templateHandlers[template] = handler;
        }

        public void AddMessageHandler(MessageHandler handler)
        {
            handlers.Add(handler);
        }

        public TemplateRegistry GetTemplateRegistry()
        {
            return context.TemplateRegistry;
        }

        #endregion

        public Message ReadMessage()
        {
            if (context.TraceEnabled)
                context.StartTrace();

            bool keepReading = blockReader.ReadBlock(in_Renamed);

            if (!keepReading)
                return null;

            Message message = decoder.ReadMessage();

            if (message == null)
            {
                return null;
            }

            blockReader.MessageRead(in_Renamed, message);

            foreach (MessageHandler t in handlers)
            {
                t.HandleMessage(message, context, decoder);
            }

            MessageHandler handler;
            if (templateHandlers.TryGetValue(message.Template, out handler))
            {
                handler.HandleMessage(message, context, decoder);

                return ReadMessage();
            }

            return message;
        }

        public void RegisterTemplate(int templateId, MessageTemplate template)
        {
            context.RegisterTemplate(templateId, template);
        }

        public void SetTemplateRegistry(TemplateRegistry registry)
        {
            context.TemplateRegistry = registry;
        }

        //public void  AddTemplateRegisteredListener(TemplateRegisteredListener templateRegisteredListener)
        //{
        //}

        public void Reset()
        {
            decoder.Reset();
        }
    }
}