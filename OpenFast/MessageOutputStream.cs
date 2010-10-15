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
using OpenFAST.Error;
using OpenFAST.Template;

namespace OpenFAST
{
    public sealed class MessageOutputStream : MessageStream
    {
        private readonly Context context;
        private readonly FastEncoder encoder;

        private readonly List<MessageHandler> handlers =
            new List<MessageHandler>();

        private readonly Stream out_Renamed;

        private readonly Dictionary<MessageTemplate, MessageHandler> templateHandlers =
            new Dictionary<MessageTemplate, MessageHandler>();


        public MessageOutputStream(Stream outputStream) : this(outputStream, new Context())
        {
        }

        public MessageOutputStream(Stream outputStream, Context context)
        {
            out_Renamed = outputStream;
            encoder = new FastEncoder(context);
            this.context = context;
        }

        public Stream UnderlyingStream
        {
            get { return out_Renamed; }
        }

        public Context Context
        {
            get { return context; }
        }

        #region MessageStream Members

        public void Close()
        {
            try
            {
                out_Renamed.Close();
            }
            catch (IOException e)
            {
                Global.HandleError(FastConstants.IO_ERROR, "An error occurred while closing output stream.", e);
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

        public void WriteMessage(Message message)
        {
            WriteMessage(message, false);
        }

        public void WriteMessage(Message message, bool flush)
        {
            try
            {
                if (context.TraceEnabled)
                    context.StartTrace();

                foreach (MessageHandler t in handlers)
                {
                    t.HandleMessage(message, context, encoder);
                }

                MessageHandler handler;
                if (templateHandlers.TryGetValue(message.Template, out handler))
                {
                    handler.HandleMessage(message, context, encoder);
                }

                byte[] data = encoder.Encode(message);

                if ((data == null) || (data.Length == 0))
                {
                    return;
                }

                byte[] temp_byteArray = data;
                out_Renamed.Write(temp_byteArray, 0, temp_byteArray.Length);
                if (flush)
                    out_Renamed.Flush();
            }
            catch (IOException e)
            {
                Global.HandleError(FastConstants.IO_ERROR, "An IO error occurred while writing message " + message,
                                   e);
            }
        }

        public void Reset()
        {
            encoder.Reset();
        }

        public void RegisterTemplate(int templateId, MessageTemplate template)
        {
            encoder.RegisterTemplate(templateId, template);
        }

        public void SetTemplateRegistry(TemplateRegistry registry)
        {
            context.TemplateRegistry = registry;
        }
    }
}