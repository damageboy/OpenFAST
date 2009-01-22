using System;
using Field = OpenFAST.Template.Field;
using Group = OpenFAST.Template.Group;

namespace OpenFAST.Session.Template.Exchange
{
	public class ConversionContext
	{
        private System.Collections.Generic.Dictionary<Group, FieldInstructionConverter> converterTemplateMap = new System.Collections.Generic.Dictionary<Group, FieldInstructionConverter>();
        private System.Collections.Generic.List<FieldInstructionConverter> converters = new System.Collections.Generic.List<FieldInstructionConverter>();
		
		public virtual void  AddFieldInstructionConverter(FieldInstructionConverter converter)
		{
			Group[] templateExchangeTemplates = converter.TemplateExchangeTemplates;
			for (int i = 0; i < templateExchangeTemplates.Length; i++)
			{
				converterTemplateMap[templateExchangeTemplates[i]] = converter;
			}
			converters.Add(converter);
		}
		
		public virtual FieldInstructionConverter GetConverter(Group group)
		{
			return (FieldInstructionConverter) converterTemplateMap[group];
		}
		
		public virtual FieldInstructionConverter GetConverter(Field field)
		{
			for (int i = converters.Count - 1; i >= 0; i--)
			{
				FieldInstructionConverter converter = (FieldInstructionConverter) converters[i];
				if (converter.ShouldConvert(field))
					return converter;
			}
			throw new System.SystemException("No valid converter found for the field: " + field);
		}
	}
}