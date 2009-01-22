using System;
using QName = OpenFAST.QName;
using Field = OpenFAST.Template.Field;
using MessageTemplate = OpenFAST.Template.MessageTemplate;

namespace OpenFAST.Template.Loader
{
	public class TemplateParser:GroupParser
	{
		
		private bool loadTemplateIdFromAuxId;
		
		public TemplateParser(bool loadTemplateIdFromAuxId)
		{
			this.loadTemplateIdFromAuxId = loadTemplateIdFromAuxId;
		}
		
		public override Field Parse(System.Xml.XmlElement templateElement, bool optional, ParsingContext context)
		{
			MessageTemplate messageTemplate = new MessageTemplate(getTemplateName(templateElement, context), ParseFields(templateElement, context));
			ParseMore(templateElement, messageTemplate, context);
			if (loadTemplateIdFromAuxId && templateElement.HasAttribute("id"))
			{
				try
				{
					int templateId = System.Int32.Parse(templateElement.GetAttribute("id"));
					context.TemplateRegistry.Register(templateId, messageTemplate);
				}
				catch (System.FormatException)
				{
					context.TemplateRegistry.Define(messageTemplate);
				}
			}
			else
				context.TemplateRegistry.Define(messageTemplate);
			return messageTemplate;
		}
		
		private QName getTemplateName(System.Xml.XmlElement templateElement, ParsingContext context)
		{
			return new QName(templateElement.GetAttribute("name"), context.TemplateNamespace);
		}
	}
}