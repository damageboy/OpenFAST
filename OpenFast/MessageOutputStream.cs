using System;
using FastEncoder = OpenFAST.Codec.FastEncoder;
using MessageTemplate = OpenFAST.Template.MessageTemplate;
using TemplateRegistry = OpenFAST.Template.TemplateRegistry;

namespace OpenFAST
{
	public sealed class MessageOutputStream : MessageStream
	{
		public System.IO.Stream UnderlyingStream
		{
			get
			{
				return out_Renamed;
			}
			
		}
		public Context Context
		{
			get
			{
				return context;
			}
			
		}
		private System.IO.Stream out_Renamed;
		private FastEncoder encoder;
		private Context context;
        private System.Collections.Generic.Dictionary<MessageTemplate, MessageHandler> templateHandlers = new System.Collections.Generic.Dictionary<MessageTemplate, MessageHandler>();
        private System.Collections.Generic.List<MessageHandler> handlers = new System.Collections.Generic.List<MessageHandler>();

		
		public MessageOutputStream(System.IO.Stream outputStream):this(outputStream, new Context())
		{
		}
		
		public MessageOutputStream(System.IO.Stream outputStream, Context context)
		{
			this.out_Renamed = outputStream;
			this.encoder = new FastEncoder(context);
			this.context = context;
		}
		
		public void  WriteMessage(Message message)
		{
			WriteMessage(message, false);
		}
		
		public void  WriteMessage(Message message, bool flush)
		{
			try
			{
				if (context.TraceEnabled)
					context.StartTrace();
				
				if (!(handlers.Count == 0))
				{
					for (int i = 0; i < handlers.Count; i++)
					{
						handlers[i].HandleMessage(message, context, encoder);
					}
				}
				if (templateHandlers.ContainsKey(message.Template))
				{
					templateHandlers[message.Template].HandleMessage(message, context, encoder);
				}
				
				byte[] data = encoder.Encode(message);
				
				if ((data == null) || (data.Length == 0))
				{
					return ;
				}
				
				byte[] temp_byteArray;
				temp_byteArray = data;
				out_Renamed.Write(temp_byteArray, 0, temp_byteArray.Length);
				if (flush)
					out_Renamed.Flush();
			}
			catch (System.IO.IOException e)
			{
				Global.HandleError(OpenFAST.Error.FastConstants.IO_ERROR, "An IO error occurred while writing message " + message, e);
			}
		}
		
		public void  Reset()
		{
			encoder.Reset();
		}
		
		public void  RegisterTemplate(int templateId, MessageTemplate template)
		{
			encoder.RegisterTemplate(templateId, template);
		}
		
		public void  Close()
		{
			try
			{
				out_Renamed.Close();
			}
			catch (System.IO.IOException e)
			{
				Global.HandleError(OpenFAST.Error.FastConstants.IO_ERROR, "An error occurred while closing output stream.", e);
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
	}
}