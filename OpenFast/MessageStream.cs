using System;
using MessageTemplate = OpenFAST.Template.MessageTemplate;
using TemplateRegistry = OpenFAST.Template.TemplateRegistry;

namespace OpenFAST
{
	
	public interface MessageStream
	{
		void  AddMessageHandler(MessageTemplate template, MessageHandler handler);
		void  AddMessageHandler(MessageHandler handler);
		void  Close();
		TemplateRegistry GetTemplateRegistry();
	}
}