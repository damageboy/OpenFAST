using System;
using Context = OpenFAST.Context;
using Message = OpenFAST.Message;
using MessageTemplate = OpenFAST.Template.MessageTemplate;
using TemplateRegisteredListener = OpenFAST.Template.TemplateRegisteredListener;

namespace OpenFAST.Codec
{
	public sealed class FastEncoder : Coder
	{
		private Context context;
	
		public FastEncoder(Context context)
		{
			this.context = context;
		}
		
		public byte[] Encode(Message message)
		{
			MessageTemplate template = message.Template;
			context.NewMessage(template);
			return template.Encode(message, context);
		}
		
		public void  Reset()
		{
			context.Reset();
		}
		
		public void  RegisterTemplate(int templateId, MessageTemplate template)
		{
			context.RegisterTemplate(templateId, template);
		}
		
	}
}