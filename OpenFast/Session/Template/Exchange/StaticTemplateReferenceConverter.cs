using System;
using GroupValue = OpenFAST.GroupValue;
using Message = OpenFAST.Message;
using QName = OpenFAST.QName;
using SessionControlProtocol_1_1 = OpenFAST.Session.SessionControlProtocol_1_1;
using Field = OpenFAST.Template.Field;
using Group = OpenFAST.Template.Group;
using StaticTemplateReference = OpenFAST.Template.StaticTemplateReference;
using TemplateRegistry = OpenFAST.Template.TemplateRegistry;

namespace OpenFAST.Session.Template.Exchange
{
	public class StaticTemplateReferenceConverter:AbstractFieldInstructionConverter
	{
		override public Group[] TemplateExchangeTemplates
		{
			get
			{
				return new Group[]{SessionControlProtocol_1_1.STAT_TEMP_REF_INSTR};
			}
			
		}
		
		public override Field Convert(GroupValue fieldDef, TemplateRegistry templateRegistry, ConversionContext context)
		{
			QName name = new QName(fieldDef.GetString("Name"), fieldDef.GetString("Ns"));
			if (!templateRegistry.IsDefined(name))
			{
				throw new System.SystemException("Referenced template " + name + " not defined.");
			}
			return new StaticTemplateReference(templateRegistry.get_Renamed(name));
		}
		
		public override GroupValue Convert(Field field, ConversionContext context)
		{
			Message strDef = new Message(SessionControlProtocol_1_1.STAT_TEMP_REF_INSTR);
			SetNameAndId(field, strDef);
			return strDef;
		}
		
		public override bool ShouldConvert(Field field)
		{
			return field.GetType().Equals(typeof(StaticTemplateReference));
		}
	}
}