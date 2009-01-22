using System;
using FieldValue = OpenFAST.FieldValue;
using GroupValue = OpenFAST.GroupValue;
using Message = OpenFAST.Message;
using SequenceValue = OpenFAST.SequenceValue;
using SessionControlProtocol_1_1 = OpenFAST.Session.SessionControlProtocol_1_1;
using Field = OpenFAST.Template.Field;
using Group = OpenFAST.Template.Group;
using MessageTemplate = OpenFAST.Template.MessageTemplate;
using TemplateRegistry = OpenFAST.Template.TemplateRegistry;

namespace OpenFAST.Session.Template.Exchange
{
	public class GroupConverter:AbstractFieldInstructionConverter
	{
		override public Group[] TemplateExchangeTemplates
		{
			get
			{
				return new Group[]{SessionControlProtocol_1_1.GROUP_INSTR};
			}
			
		}
		
		public override Field Convert(GroupValue fieldDef, TemplateRegistry templateRegistry, ConversionContext context)
		{
			string name = fieldDef.GetString("Name");
			Field[] fields = ParseFieldInstructions(fieldDef, templateRegistry, context);
			bool optional = fieldDef.GetBool("Optional");
			return new Group(name, fields, optional);
		}
		
		public override GroupValue Convert(Field field, ConversionContext context)
		{
			Group group = (Group) field;
			Message groupMsg = Convert(group, new Message(SessionControlProtocol_1_1.GROUP_INSTR), context);
			groupMsg.SetBool("Optional", field.Optional);
			return groupMsg;
		}
		
		public override bool ShouldConvert(Field field)
		{
			return field.GetType().Equals(typeof(Group));
		}
		
		public static Message Convert(Group group, Message groupMsg, ConversionContext context)
		{
			SetNameAndId(group, groupMsg);
			SequenceValue instructions = new SequenceValue(SessionControlProtocol_1_1.TEMPLATE_DEFINITION.GetSequence("Instructions"));
			int i = group is MessageTemplate?1:0;
			Field[] fields = group.FieldDefinitions;
			for (; i < fields.Length; i++)
			{
				Field field = fields[i];
				FieldInstructionConverter converter = context.GetConverter(field);
				if (converter == null)
					throw new System.SystemException("No converter found for type " + field.GetType());
				FieldValue value_Renamed = converter.Convert(field, context);
				instructions.Add(new FieldValue[]{value_Renamed});
			}
			groupMsg.SetFieldValue("Instructions", instructions);
			return groupMsg;
		}
		
		public static Field[] ParseFieldInstructions(GroupValue groupDef, TemplateRegistry registry, ConversionContext context)
		{
			SequenceValue instructions = groupDef.GetSequence("Instructions");
			Field[] fields = new Field[instructions.Length];
			for (int i = 0; i < fields.Length; i++)
			{
				GroupValue fieldDef = instructions[i].GetGroup(0);
				FieldInstructionConverter converter = context.GetConverter(fieldDef.GetGroup());
				if (converter == null)
				{
					throw new System.SystemException("Encountered unknown group " + fieldDef.GetGroup() + "while processing field instructions " + groupDef.GetGroup());
				}
				fields[i] = converter.Convert(fieldDef, registry, context);
			}
			return fields;
		}
	}
}