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
    public sealed class MessageOutputStream : IMessageStream
    {
        private readonly Context _context;
        private readonly FastEncoder _encoder;
        private readonly List<IMessageHandler> _handlers = new List<IMessageHandler>();
        private readonly Stream _outStream;

        private readonly Dictionary<MessageTemplate, IMessageHandler> _templateHandlers =
            new Dictionary<MessageTemplate, IMessageHandler>();


        public MessageOutputStream(Stream outputStream) : this(outputStream, new Context())
        {
        }

        public MessageOutputStream(Stream outputStream, Context context)
        {
            _outStream = outputStream;
            _encoder = new FastEncoder(context);
            _context = context;
        }

        public Stream UnderlyingStream
        {
            get { return _outStream; }
        }

        public Context Context
        {
            get { return _context; }
        }

        #region IMessageStream Members

        public void Close()
        {
            try
            {
                _outStream.Close();
            }
            catch (IOException e)
            {
                Global.HandleError(FastConstants.IO_ERROR, "An error occurred while closing output stream.", e);
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

        public ITemplateRegistry GetTemplateRegistry()
        {
            return _context.TemplateRegistry;
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
                if (_context.TraceEnabled)
                    _context.StartTrace();

                foreach (IMessageHandler t in _handlers)
                {
                    t.HandleMessage(message, _context, _encoder);
                }

                IMessageHandler handler;
                if (_templateHandlers.TryGetValue(message.Template, out handler))
                {
                    handler.HandleMessage(message, _context, _encoder);
                }

                byte[] data = _encoder.Encode(message);

                if (data == null || data.Length == 0)
                    return;

                byte[] tmp = data;
                _outStream.Write(tmp, 0, tmp.Length);
                if (flush)
                    _outStream.Flush();
            }
            catch (IOException e)
            {
                Global.HandleError(
                    FastConstants.IO_ERROR, "An IO error occurred while writing message " + message, e);
            }
        }

        public void Reset()
        {
            _encoder.Reset();
        }

        public void RegisterTemplate(int templateId, MessageTemplate template)
        {
            _encoder.RegisterTemplate(templateId, template);
        }

        public void SetTemplateRegistry(ITemplateRegistry registry)
        {
            _context.TemplateRegistry = registry;
        }
    }
}