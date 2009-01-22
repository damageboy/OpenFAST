using System;
using MessageTemplate = OpenFAST.Template.MessageTemplate;
using TemplateRegistry = OpenFAST.Template.TemplateRegistry;

namespace OpenFAST.Template.Loader
{
	public interface MessageTemplateLoader
	{
		TemplateRegistry TemplateRegistry
		{
			get;
			
			set;
			
		}
		MessageTemplate[] Load(System.IO.Stream source);
	}
}