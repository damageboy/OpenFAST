using System;
using QName = OpenFAST.QName;
using DynamicTemplateReference = OpenFAST.Template.DynamicTemplateReference;
using Field = OpenFAST.Template.Field;
using StaticTemplateReference = OpenFAST.Template.StaticTemplateReference;

namespace OpenFAST.Template.Loader
{
	public class TemplateRefParser : FieldParser
	{
		public virtual Field Parse(System.Xml.XmlElement element, ParsingContext context)
		{
			if (element.HasAttribute("name"))
			{
				QName templateName;
				if (element.HasAttribute("templateNs"))
					templateName = new QName(element.GetAttribute("name"), element.GetAttribute("templateNs"));
				else
					templateName = new QName(element.GetAttribute("name"), "");
				
				if (context.TemplateRegistry.IsDefined(templateName))
					return new StaticTemplateReference(context.TemplateRegistry.get_Renamed(templateName));
				else
				{
					context.ErrorHandler.Error(OpenFAST.Error.FastConstants.D8_TEMPLATE_NOT_EXIST, "The template \"" + templateName + "\" was not found.");
					return null;
				}
			}
			else
			{
				return DynamicTemplateReference.INSTANCE;
			}
		}
		
		public virtual bool CanParse(System.Xml.XmlElement element, ParsingContext context)
		{
			return "templateRef".Equals(element.Name);
		}
	}
}