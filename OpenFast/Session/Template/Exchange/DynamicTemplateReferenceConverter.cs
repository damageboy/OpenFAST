using System;
using GroupValue = OpenFAST.GroupValue;
using SessionControlProtocol_1_1 = OpenFAST.Session.SessionControlProtocol_1_1;
using DynamicTemplateReference = OpenFAST.Template.DynamicTemplateReference;
using Field = OpenFAST.Template.Field;
using Group = OpenFAST.Template.Group;
using TemplateRegistry = OpenFAST.Template.TemplateRegistry;

namespace OpenFAST.Session.Template.Exchange
{
	public class DynamicTemplateReferenceConverter : FieldInstructionConverter
	{
		virtual public Group[] TemplateExchangeTemplates
		{
			get
			{
				return new Group[]{SessionControlProtocol_1_1.DYN_TEMP_REF_INSTR};
			}
			
		}
		
		public virtual Field Convert(GroupValue groupValue, TemplateRegistry templateRegistry, ConversionContext context)
		{
			return DynamicTemplateReference.INSTANCE;
		}
		
		public virtual GroupValue Convert(Field field, ConversionContext context)
		{
			return SessionControlProtocol_1_1.DYN_TEMP_REF_MESSAGE;
		}
		
		public virtual bool ShouldConvert(Field field)
		{
			return field.GetType().Equals(typeof(DynamicTemplateReference));
		}
	}
}