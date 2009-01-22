using System;
using FastDecoder = OpenFAST.Codec.FastDecoder;
using MessageTemplate = OpenFAST.Template.MessageTemplate;
using TemplateRegisteredListener = OpenFAST.Template.TemplateRegisteredListener;
using TemplateRegistry = OpenFAST.Template.TemplateRegistry;
using OpenFAST;

namespace OpenFAST
{
	public sealed class MessageInputStream : MessageStream
	{
		public System.IO.Stream UnderlyingStream
		{
			get
			{
				return in_Renamed;
			}
			
		}
		public Context Context
		{
			get
			{
				return context;
			}
			
		}
		public MessageBlockReader BlockReader
		{
			set
			{
				this.blockReader = value;
			}
			
		}
		private System.IO.Stream in_Renamed;
		private FastDecoder decoder;
		private Context context;
        private System.Collections.Generic.Dictionary<MessageTemplate, MessageHandler> templateHandlers = new System.Collections.Generic.Dictionary<MessageTemplate, MessageHandler>();
        private System.Collections.Generic.List<MessageHandler> handlers = new System.Collections.Generic.List<MessageHandler>();
		private MessageBlockReader blockReader = OpenFAST.MessageBlockReader_Fields.NULL;
		
		public MessageInputStream(System.IO.Stream inputStream):this(inputStream, new Context())
		{
		}
		
		public MessageInputStream(System.IO.Stream inputStream, Context context)
		{
			this.in_Renamed = inputStream;
			this.context = context;
			this.decoder = new FastDecoder(context, in_Renamed);
		}

		public Message readMessage()
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
			
			if (!(handlers.Count == 0))
			{
				for (int i = 0; i < handlers.Count; i++)
				{
					handlers[i].HandleMessage(message, context, decoder);
				}
			}
			if (templateHandlers.ContainsKey(message.Template))
			{
                templateHandlers[message.Template].HandleMessage(message, context, decoder);
				
				return readMessage();
			}
			
			return message;
		}
		
		public void  RegisterTemplate(int templateId, MessageTemplate template)
		{
			context.RegisterTemplate(templateId, template);
		}
		
		public void  Close()
		{
			try
			{
				in_Renamed.Close();
			}
			catch (System.IO.IOException e)
			{
				throw new RuntimeException(e);
			}
		}
		
		public void  AddMessageHandler(MessageTemplate template, MessageHandler handler)
		{
			templateHandlers[template] = handler;
		}
		
		public void  AddMessageHandler(MessageHandler handler)
		{
			handlers.Add(handler);
		}
		
		public void  SetTemplateRegistry(TemplateRegistry registry)
		{
			context.TemplateRegistry = registry;
		}
		
		public TemplateRegistry GetTemplateRegistry()
		{
			return context.TemplateRegistry;
		}
		
		public void  AddTemplateRegisteredListener(TemplateRegisteredListener templateRegisteredListener)
		{
		}
		
		public void  Reset()
		{
			decoder.Reset();
		}
	}
}