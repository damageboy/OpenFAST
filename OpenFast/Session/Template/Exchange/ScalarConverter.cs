using System;
using FieldValue = OpenFAST.FieldValue;
using GroupValue = OpenFAST.GroupValue;
using Message = OpenFAST.Message;
using QName = OpenFAST.QName;
using ScalarValue = OpenFAST.ScalarValue;
using SessionControlProtocol_1_1 = OpenFAST.Session.SessionControlProtocol_1_1;
using Field = OpenFAST.Template.Field;
using Group = OpenFAST.Template.Group;
using MessageTemplate = OpenFAST.Template.MessageTemplate;
using Scalar = OpenFAST.Template.Scalar;
using TemplateRegistry = OpenFAST.Template.TemplateRegistry;
using Operator = OpenFAST.Template.operator_Renamed.Operator;
using Type = OpenFAST.Template.Type.FASTType;

namespace OpenFAST.Session.Template.Exchange
{
	public class ScalarConverter:AbstractFieldInstructionConverter
	{
		override public Group[] TemplateExchangeTemplates
		{
			get
			{
				return SupportClass.ICollectionSupport.ToArray<Group>(new SupportClass.HashSetSupport(TEMPLATE_TYPE_MAP.Keys));
			}
			
		}
		
		private System.Collections.IDictionary TYPE_TEMPLATE_MAP = new System.Collections.Hashtable();
		private System.Collections.IDictionary TEMPLATE_TYPE_MAP = new System.Collections.Hashtable();
		
		public ScalarConverter()
		{
			TYPE_TEMPLATE_MAP[Type.I32] = SessionControlProtocol_1_1.INT32_INSTR;
			TYPE_TEMPLATE_MAP[Type.U32] = SessionControlProtocol_1_1.UINT32_INSTR;
			TYPE_TEMPLATE_MAP[Type.I64] = SessionControlProtocol_1_1.INT64_INSTR;
			TYPE_TEMPLATE_MAP[Type.U64] = SessionControlProtocol_1_1.UINT64_INSTR;
			TYPE_TEMPLATE_MAP[Type.DECIMAL] = SessionControlProtocol_1_1.DECIMAL_INSTR;
			TYPE_TEMPLATE_MAP[Type.UNICODE] = SessionControlProtocol_1_1.UNICODE_INSTR;
			TYPE_TEMPLATE_MAP[Type.ASCII] = SessionControlProtocol_1_1.ASCII_INSTR;
			TYPE_TEMPLATE_MAP[Type.STRING] = SessionControlProtocol_1_1.ASCII_INSTR;
			TYPE_TEMPLATE_MAP[Type.BYTE_VECTOR] = SessionControlProtocol_1_1.BYTE_VECTOR_INSTR;
			
			TEMPLATE_TYPE_MAP[SessionControlProtocol_1_1.INT32_INSTR] = Type.I32;
			TEMPLATE_TYPE_MAP[SessionControlProtocol_1_1.UINT32_INSTR] = Type.U32;
			TEMPLATE_TYPE_MAP[SessionControlProtocol_1_1.INT64_INSTR] = Type.I64;
			TEMPLATE_TYPE_MAP[SessionControlProtocol_1_1.UINT64_INSTR] = Type.U64;
			TEMPLATE_TYPE_MAP[SessionControlProtocol_1_1.DECIMAL_INSTR] = Type.DECIMAL;
			TEMPLATE_TYPE_MAP[SessionControlProtocol_1_1.UNICODE_INSTR] = Type.UNICODE;
			TEMPLATE_TYPE_MAP[SessionControlProtocol_1_1.ASCII_INSTR] = Type.ASCII;
			TEMPLATE_TYPE_MAP[SessionControlProtocol_1_1.BYTE_VECTOR_INSTR] = Type.BYTE_VECTOR;
		}
		
		public override Field Convert(GroupValue fieldDef, TemplateRegistry templateRegistry, ConversionContext context)
		{
			Type type = (Type) TEMPLATE_TYPE_MAP[fieldDef.GetGroup()];
			bool optional = fieldDef.GetBool("Optional");
			ScalarValue initialValue = ScalarValue.UNDEFINED;
			if (fieldDef.IsDefined("InitialValue"))
				initialValue = (ScalarValue) fieldDef.GetValue("InitialValue");
			
			if (fieldDef.IsDefined("Operator"))
			{
				GroupValue operatorGroup = fieldDef.GetGroup("Operator").GetGroup(0);
				Operator operator_Renamed = GetOperator(operatorGroup.GetGroup());
				Scalar scalar = new Scalar(fieldDef.GetString("Name"), type, operator_Renamed, initialValue, optional);
				if (operatorGroup.IsDefined("Dictionary"))
					scalar.Dictionary = operatorGroup.GetString("Dictionary");
				if (operatorGroup.IsDefined("Key"))
				{
					string name = operatorGroup.GetGroup("Key").GetString("Name");
					string ns = operatorGroup.GetGroup("Key").GetString("Ns");
					scalar.Key = new QName(name, ns);
				}
				return scalar;
			}
			else
			{
				return new Scalar(fieldDef.GetString("Name"), type, Operator.NONE, initialValue, optional);
			}
		}
		
		public override GroupValue Convert(Field field, ConversionContext context)
		{
			Scalar scalar = (Scalar) field;
			MessageTemplate scalarTemplate = (MessageTemplate) TYPE_TEMPLATE_MAP[scalar.Type];
			Message scalarMsg = new Message(scalarTemplate);
			SetNameAndId(scalar, scalarMsg);
			scalarMsg.SetInteger("Optional", scalar.Optional?1:0);
			if (!scalar.Operator.Equals(Operator.NONE))
				scalarMsg.SetFieldValue("Operator", new GroupValue(scalarTemplate.GetGroup("Operator"), new FieldValue[]{CreateOperator(scalar)}));
			if (!scalar.DefaultValue.Undefined)
				scalarMsg.SetFieldValue("InitialValue", scalar.DefaultValue);
			return scalarMsg;
		}
		
		public override bool ShouldConvert(Field field)
		{
			return field.GetType().Equals(typeof(Scalar));
		}
	}
}