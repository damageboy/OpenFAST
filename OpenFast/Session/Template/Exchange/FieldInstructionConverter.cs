using System;
using GroupValue = OpenFAST.GroupValue;
using Field = OpenFAST.Template.Field;
using Group = OpenFAST.Template.Group;
using TemplateRegistry = OpenFAST.Template.TemplateRegistry;

namespace OpenFAST.Session.Template.Exchange
{
	public interface FieldInstructionConverter
	{
		Group[] TemplateExchangeTemplates
		{
			get;
			
		}
		bool ShouldConvert(Field field);
		Field Convert(GroupValue fieldDef, TemplateRegistry templateRegistry, ConversionContext context);
		GroupValue Convert(Field field, ConversionContext context);
	}
}